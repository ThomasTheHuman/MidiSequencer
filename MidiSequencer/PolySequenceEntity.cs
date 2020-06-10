using System.Xml.Serialization;

namespace MidiSequencer
{
    /// <inheritdoc />
    [XmlRoot(ElementName = "poly-sequence")]
    public class PolySequenceEntity : SequenceEntity
    {
        /// <summary>
        ///     Array of steps.
        /// </summary>
        public PolyStepEntity[] Steps { get; set; }
    }
}