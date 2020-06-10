namespace MidiSequencer
{
    /// <inheritdoc />
    public class PolyStepEntity : StepEntity
    {
        /// <summary>
        ///     State of step.
        /// </summary>
        public bool On { get; set; }

        /// <summary>
        ///     Array of notes recorded to step.
        /// </summary>
        public int?[] Notes { get; set; }
    }
}