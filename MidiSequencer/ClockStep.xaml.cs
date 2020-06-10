using System;
using System.Windows.Media;

namespace MidiSequencer
{
    /// <summary>
    ///     type of Step used by ClockSequence.
    /// </summary>
    public partial class ClockStep : Step
    {
        /// <summary>
        ///     Initializes control.
        /// </summary>
        public ClockStep()
        {
            InitializeComponent();
            Button.Click -= Toggle;
            Button.Height = 30;
            Highlight.Height = 30;
            Container.Height = 30;
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

        /// <inheritdoc />
        public override void LoadMidi(Sequence sequence)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public override void NoteOn(Sequence sequence)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public override void NoteOff(Sequence sequence)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public override StepEntity Serialize()
        {
            return new ClockStepEntity();
        }

        /// <inheritdoc />
        public override void Deserialize(StepEntity step)
        {
            throw new NotImplementedException();
        }
    }
}