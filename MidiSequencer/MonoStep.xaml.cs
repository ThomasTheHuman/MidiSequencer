using System.Linq;
using System.Windows.Media;
using NAudio.Midi;

namespace MidiSequencer
{
    /// <summary>
    ///     type of Step used by MonoSequence.
    /// </summary>
    public partial class MonoStep : Step
    {
        /// <summary>
        ///     Initializes control.
        /// </summary>
        public MonoStep()
        {
            InitializeComponent();
        }

        private MidiEvent MidiEvent { get; set; }

        /// <inheritdoc />
        protected override ColorPair Setup()
        {
            var colorPair = new ColorPair
            {
                On = FindResource("BlueBrush") as SolidColorBrush,
                Off = FindResource("DarkBlueBrush") as SolidColorBrush
            };
            return colorPair;
        }

        /// <inheritdoc />
        public override void LoadMidi(Sequence sequence)
        {
            var eventPool = sequence.Input.EventPool;
            if (eventPool.Count == 0) return;
            if (!On) Toggle();

            MidiEvent = eventPool.Last().Clone();
            eventPool.Clear();
        }

        /// <inheritdoc />
        public override void NoteOn(Sequence sequence)
        {
            if (MidiEvent != null && On) sequence.Output.SendEvent(MidiEvent);
        }

        /// <inheritdoc />
        public override void NoteOff(Sequence sequence)
        {
            if (MidiEvent != null && On)
                sequence.Output.SendEvent(new NoteEvent(0, 1,
                    MidiCommandCode.NoteOff, ((NoteEvent) MidiEvent).NoteNumber, 0));
        }

        /// <inheritdoc />
        public override StepEntity Serialize()
        {
            return new MonoStepEntity
            {
                On = On,
                Note = (MidiEvent as NoteEvent)?.NoteNumber
            };
        }

        /// <inheritdoc />
        public override void Deserialize(StepEntity step)
        {
            if (step == null) throw new CorruptedFileException();

            var note = (step as MonoStepEntity)?.Note;
            if (note != null)
                MidiEvent = new NoteEvent(0, 1,
                    MidiCommandCode.NoteOn, note.Value, 100);
            if (((MonoStepEntity) step).On) Toggle();
        }
    }
}