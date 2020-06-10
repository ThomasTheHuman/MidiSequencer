using System.Xml.Serialization;

namespace MidiSequencer
{
    /// <summary>
    ///     Entity class used for serialization.
    /// </summary>
    public class SequencerEntity
    {
        /// <summary>
        ///     Tempo in beats per minute.
        /// </summary>
        public int Bpm { get; set; }

        /// <summary>
        ///     Serialized ClockSequence
        /// </summary>
        public ClockSequenceEntity Clock { get; set; }

        /// <summary>
        ///     Array of serialized sequences.
        /// </summary>
        [XmlArrayItem(typeof(MonoSequenceEntity), ElementName = "mono-sequence")]
        [XmlArrayItem(typeof(PolySequenceEntity), ElementName = "poly-sequence")]
        [XmlArrayItem(typeof(DrumSequenceEntity), ElementName = "drum-sequence")]
        public SequenceEntity[] Sequences { get; set; }
    }
}