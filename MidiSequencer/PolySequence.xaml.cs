using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace MidiSequencer
{
    /// <summary>
    ///     Sequence type used for polyphonic instruments.
    /// </summary>
    public partial class PolySequence : Sequence
    {
        /// <inheritdoc />
        public PolySequence(Sequencer sequencer) : base(sequencer)
        {
            ReloadSteps<PolyStep>();

            InitializeComponent();
        }

        /// <inheritdoc />
        protected override void ReloadSteps()
        {
            ReloadSteps<PolyStep>();
        }

        /// <inheritdoc />
        protected override ColorPair Setup()
        {
            var colorPair = new ColorPair
            {
                On = FindResource("YellowBrush") as SolidColorBrush,
                Off = FindResource("DarkYellowBrush") as SolidColorBrush
            };
            return colorPair;
        }

        /// <inheritdoc />
        protected override async void RecordButton_Click(object sender, RoutedEventArgs e)
        {
            (Sequencer.ClockSequence as ClockSequence)?.Stop();
            Input.EventPool.Clear();
            Recording = !Recording;

            foreach (var step in Steps)
            {
                step.SetHighlight(true);
                while (Input.EventPool.Count == 0 && Recording && !Rest) await Task.Delay(25);

                if (!Recording)
                {
                    step.SetHighlight(false);
                    break;
                }

                if (Rest)
                {
                    Rest = false;
                    step.SetHighlight(false);
                }
                else
                {
                    while (Input.NoteCount != 0) await Task.Delay(25); //waits for all keys to be released

                    step.SetHighlight(false);
                    step.LoadMidi(this);
                }
            }

            Recording = false;
            Rest = false;
        }

        /// <inheritdoc />
        public override SequenceEntity Serialize()
        {
            return new PolySequenceEntity
            {
                Type = GetType().FullName,
                Gate = Gate,
                Beats = Beats,
                Bars = Bars,
                Steps = Steps.Select(step => step.Serialize() as PolyStepEntity).ToArray(),
                Input = Input.Serialize(),
                Output = Output.Serialize()
            };
        }

        /// <inheritdoc />
        public override void Deserialize(SequenceEntity entity)
        {
            if (entity == null) throw new CorruptedFileException();

            Bars = entity.Bars;
            Beats = entity.Beats;
            Gate = entity.Gate;
            ReloadSteps();

            Input.Channel = entity.Input.Channel;
            if (entity.Input.Device.HasValue) Input.ConnectToDevice(entity.Input.Device.Value);

            Output.Channel = entity.Output.Channel;
            if (entity.Output.Device.HasValue) Output.ConnectToDevice(entity.Output.Device.Value);

            var i = 0;
            foreach (var step in ((PolySequenceEntity) entity).Steps)
            {
                Steps[i].Deserialize(step);
                i++;
            }
        }
    }
}