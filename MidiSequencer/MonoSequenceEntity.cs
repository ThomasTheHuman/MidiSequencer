using System.Xml.Serialization;

namespace MidiSequencer
{
    /// <inheritdoc />
    [XmlRoot(ElementName = "mono-sequence")]
    public class MonoSequenceEntity : SequenceEntity
    {
        /// <summary>
        ///     Array of steps.
        /// </summary>
        public MonoStepEntity[] Steps { get; set; }
    }
}