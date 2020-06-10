namespace MidiSequencer
{
    /// <summary>
    ///     Class used for serialization of MidiInterface.
    /// </summary>
    public struct MidiInterfaceEntity
    {
        /// <summary>
        ///     MIDI channel.
        /// </summary>
        public int Channel { get; set; }

        /// <summary>
        ///     ID of MIDI device.
        /// </summary>
        public int? Device { get; set; }
    }
}