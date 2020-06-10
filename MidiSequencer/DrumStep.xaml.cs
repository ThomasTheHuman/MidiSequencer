using System.Linq;
using System.Windows.Media;
using NAudio.Midi;

namespace MidiSequencer
{
    /// <summary>
    ///     type of Step used by DrumSequence.
    /// </summary>
    public partial class DrumStep : Step
    {
        /// <summary>
        ///     Initializes control.
        /// </summary>
        public DrumStep()
        {
            InitializeComponent();
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
        public override void LoadMidi(Sequence sequence)
        {
            var eventPool = sequence.Input.EventPool;
            if (eventPool.All(e =>
                ((NoteOnEvent) e).NoteNumber != ((DrumSequence) sequence).InNote)) return;
            eventPool.Clear();
            if (!On) Toggle();
        }

        /// <inheritdoc />
        public override void NoteOn(Sequence sequence)
        {
            if (!On) return;
            var midiEvent = new NoteEvent(0, sequence.Output.Channel,
                MidiCommandCode.NoteOn, ((DrumSequence) sequence).OutNote, 100);
            sequence.Output.SendEvent(midiEvent);
        }

        /// <inheritdoc />
        public override void NoteOff(Sequence sequence)
        {
            if (!On) return;
            var midiEvent = new NoteEvent(0, sequence.Output.Channel,
                MidiCommandCode.NoteOff, ((DrumSequence) sequence).OutNote, 100);
            sequence.Output.SendEvent(midiEvent);
        }

        /// <inheritdoc />
        public override StepEntity Serialize()
        {
            return new DrumStepEntity
            {
                On = On
            };
        }

        /// <inheritdoc />
        public override void Deserialize(StepEntity step)
        {
            if (step == null) throw new CorruptedFileException();

            if (((DrumStepEntity) step).On) Toggle();
        }
    }
}