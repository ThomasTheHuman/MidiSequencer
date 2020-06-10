using System.Collections.Generic;
using System.Linq;
using System.Windows.Media;
using NAudio.Midi;

namespace MidiSequencer
{
    /// <summary>
    ///     type of Step used by PolySequence.
    /// </summary>
    public partial class PolyStep : Step
    {
        private readonly List<MidiEvent> _midiEvents = new List<MidiEvent>();

        /// <summary>
        ///     Initializes control.
        /// </summary>
        public PolyStep()
        {
            InitializeComponent();
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
        public override void LoadMidi(Sequence sequence)
        {
            _midiEvents.Clear();
            foreach (var newEvent in sequence.Input.EventPool) _midiEvents.Add(newEvent.Clone());

            sequence.Input.EventPool.Clear();
            if (!On) Toggle();
        }

        /// <inheritdoc />
        public override void NoteOn(Sequence sequence)
        {
            if (!On) return;
            foreach (var e in _midiEvents) sequence.Output.SendEvent(e);
        }

        /// <inheritdoc />
        public override void NoteOff(Sequence sequence)
        {
            if (!On) return;
            foreach (var e in _midiEvents)
                sequence.Output.SendEvent(new NoteEvent(0, 1,
                    MidiCommandCode.NoteOff, ((NoteEvent) e).NoteNumber, 0));
        }

        /// <inheritdoc />
        public override StepEntity Serialize()
        {
            return new PolyStepEntity
            {
                On = On,
                Notes = _midiEvents.Select(e => (e as NoteEvent)?.NoteNumber).ToArray()
            };
        }

        /// <inheritdoc />
        public override void Deserialize(StepEntity step)
        {
            if (step == null) throw new CorruptedFileException();

            if (((PolyStepEntity) step).Notes.Length != 0)
                foreach (var note in ((PolyStepEntity) step).Notes)
                    if (note != null)
                        _midiEvents.Add(new NoteEvent(0, 1,
                            MidiCommandCode.NoteOn, note.Value, 100));

            if (((PolyStepEntity) step).On) Toggle();
        }
    }
}