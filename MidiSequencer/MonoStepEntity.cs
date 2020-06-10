namespace MidiSequencer
{
    /// <inheritdoc />
    public class MonoStepEntity : StepEntity
    {
        /// <summary>
        ///     State of step.
        /// </summary>
        public bool On { get; set; }

        /// <summary>
        ///     Note recorded on step, null if no note recorded.
        /// </summary>
        public int? Note { get; set; }
    }
}