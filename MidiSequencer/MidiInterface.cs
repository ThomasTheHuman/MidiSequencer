namespace MidiSequencer
{
    /// <summary>
    ///     Abstract class for MIDI interfacing.
    /// </summary>
    public abstract class MidiInterface
    {
        /// <summary>
        ///     ID of MIDI device.
        /// </summary>
        protected int? Device;

        /// <summary>
        ///     MIDI channel used.
        /// </summary>
        public int Channel { get; set; }

        /// <summary>
        ///     Connects to device of specified id.
        /// </summary>
        /// <param name="deviceId">ID of device to connect to.</param>
        public abstract void ConnectToDevice(int deviceId);

        /// <summary>
        ///     Creates simple object for serialization.
        /// </summary>
        /// <returns>Object for serialization.</returns>
        public MidiInterfaceEntity Serialize()
        {
            return new MidiInterfaceEntity
            {
                Channel = Channel,
                Device = Device
            };
        }

        /// <summary>
        ///     Returns device id.
        /// </summary>
        /// <returns>Device id to which interface is connected if it is connected or null if it's not.</returns>
        public int? GetDevice()
        {
            return Device;
        }
    }
}