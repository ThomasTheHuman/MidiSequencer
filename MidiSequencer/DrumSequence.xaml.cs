using System;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;

namespace MidiSequencer
{
    /// <summary>
    ///     Sequence type used for sequencing drums.
    /// </summary>
    public partial class DrumSequence : Sequence
    {
        /// <inheritdoc />
        public DrumSequence(Sequencer sequencer) : base(sequencer)
        {
            ReloadSteps<DrumStep>();

            InitializeComponent();
            StackPanel.Children.Remove(RecordButton);
            StackPanel.Children.Remove(RestButton);
            var rect = new Rectangle
            {
                Height = 30,
                Width = 120
            };
            StackPanel.Children.Insert(2, rect);
        }

        /// <summary>
        ///     Input note, default C4 (60)
        /// </summary>
        public int InNote { get; set; } = 60; //MIDI code for middle C (C4)

        /// <summary>
        ///     Output note, default C4 (60)
        /// </summary>
        public int OutNote { get; set; } = 60; //MIDI code for middle C (C4)

        /// <inheritdoc />
        protected override void ReloadSteps()
        {
            ReloadSteps<DrumStep>();
        }

        /// <inheritdoc />
        protected override ColorPair Setup()
        {
            var colorPair = new ColorPair
            {
                On = FindResource("RedBrush") as SolidColorBrush,
                Off = FindResource("DarkRedBrush") as SolidColorBrush
            };
            return colorPair;
        }

        /// <inheritdoc />
        protected override void RecordButton_Click(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public override SequenceEntity Serialize()
        {
            return new DrumSequenceEntity
            {
                Type = GetType().FullName,
                Gate = Gate,
                Beats = Beats,
                Bars = Bars,
                Steps = Steps.Select(step => step.Serialize() as DrumStepEntity).ToArray(),
                Input = Input.Serialize(),
                Output = Output.Serialize(),
                InNote = InNote,
                OutNote = OutNote
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

            InNote = ((DrumSequenceEntity) entity).InNote;
            OutNote = ((DrumSequenceEntity) entity).OutNote;

            Input.Channel = entity.Input.Channel;
            if (entity.Input.Device.HasValue) Input.ConnectToDevice(entity.Input.Device.Value);

            Output.Channel = entity.Output.Channel;
            if (entity.Output.Device.HasValue) Output.ConnectToDevice(entity.Output.Device.Value);

            var i = 0;
            foreach (var step in ((DrumSequenceEntity) entity).Steps)
            {
                Steps[i].Deserialize(step);
                i++;
            }
        }
    }
}