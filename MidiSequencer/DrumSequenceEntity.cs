using System.Xml.Serialization;

namespace MidiSequencer
{
    /// <inheritdoc />
    [XmlRoot(ElementName = "drum-sequence")]
    public class DrumSequenceEntity : SequenceEntity
    {
        /// <summary>
        ///     MIDI code for note that is used for input.
        /// </summary>
        public int InNote { get; set; }

        /// <summary>
        ///     MIDI code for note that is used for output.
        /// </summary>
        public int OutNote { get; set; }

        /// <summary>
        ///     Array of steps.
        /// </summary>
        public DrumStepEntity[] Steps { get; set; }
    }
}