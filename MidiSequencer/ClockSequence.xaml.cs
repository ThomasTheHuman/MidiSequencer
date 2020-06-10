using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;

namespace MidiSequencer
{
    /// <summary>
    ///     Clock sequence UserControl
    /// </summary>
    public partial class ClockSequence : Sequence
    {
        private readonly SequenceTimer _timer;

        /// <inheritdoc />
        public ClockSequence(Sequencer sequencer) : base(sequencer)
        {
            ReloadSteps<ClockStep>();

            _timer = new SequenceTimer(sequencer, Steps);
            _timer.SetTimer();

            InitializeComponent();
            StackPanel.Children.Remove(OnButton);
            StackPanel.Children.Remove(HotRecButton);
            StackPanel.Children.Remove(RecordButton);
            StackPanel.Children.Remove(RestButton);
            var rect = new Rectangle
            {
                Height = 30,
                Width = 240
            };
            StackPanel.Children.Insert(0, rect);
            EditButton.Height = 30;
            StackPanel.Children.Remove(DeleteButton);
            DeleteButton = null;
        }

        /// <inheritdoc />
        protected override void ReloadSteps()
        {
            ReloadSteps<ClockStep>();
        }

        /// <inheritdoc />
        protected override ColorPair Setup()
        {
            var colorPair = new ColorPair
            {
                On = FindResource("PlatinumBrush") as SolidColorBrush,
                Off = Brushes.Transparent
            };
            return colorPair;
        }

        /// <summary>
        ///     Reloads timer, used when sequencer settings such as tempo changed.
        /// </summary>
        public void ReloadTimer()
        {
            _timer.SetTimer();
        }

        /// <summary>
        ///     Starts sequencer.
        /// </summary>
        public void Start()
        {
            _timer.Start();
        }

        /// <summary>
        ///     Pauses sequencer.
        /// </summary>
        public void Pause()
        {
            _timer.Pause();
        }

        /// <summary>
        ///     Stops sequencer.
        /// </summary>
        public void Stop()
        {
            _timer.Stop();
        }

        /// <inheritdoc />
        protected override void RecordButton_Click(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public override SequenceEntity Serialize()
        {
            return new ClockSequenceEntity
            {
                Type = GetType().FullName,
                Gate = Gate,
                Beats = Beats,
                Bars = Bars,
                Input = Input.Serialize(),
                Output = Output.Serialize()
            };
        }

        /// <inheritdoc />
        public override void Deserialize(SequenceEntity entity)
        {
            if (entity == null) throw new CorruptedFileException();

            Stop();
            Bars = entity.Bars;
            Beats = entity.Beats;
            ReloadSteps();
            _timer.SetTimer();
        }
    }
}