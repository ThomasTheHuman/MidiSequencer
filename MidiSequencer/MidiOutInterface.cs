using System.Diagnostics;
using MidiSequencer.Properties;
using NAudio.Midi;

namespace MidiSequencer
{
    /// <summary>
    ///     Class used for interfacing with MIDI output devices
    /// </summary>
    public class MidiOutInterface : MidiInterface
    {
        private MidiOut _connection;

        /// <summary>
        ///     Sets channel to 1, because default 0 is reserved for all channels.
        /// </summary>
        public MidiOutInterface()
        {
            Channel = 1;
        }

        /// <inheritdoc />
        public override void ConnectToDevice(int deviceId)
        {
            try
            {
                _connection = new MidiOut(deviceId);
                Device = deviceId;
            }
            catch
            {
                Debug.WriteLine(Resources.Cant_connect_to_midi_out_device + deviceId);
            }
        }

        /// <summary>
        ///     Sends MIDI event to connected device.
        /// </summary>
        /// <param name="e">MIDI event.</param>
        public void SendEvent(MidiEvent e)
        {
            e.Channel = Channel;
            _connection?.Send(e.GetAsShortMessage());
        }

        /// <summary>Gets connected MIDI output devices</summary>
        /// <returns>Array of MidiOutCapabilities</returns>
        public static MidiOutCapabilities[] GetMidiOutDevices()
        {
            var devices = new MidiOutCapabilities[MidiOut.NumberOfDevices];
            for (var device = 0; device < MidiOut.NumberOfDevices; device++)
                devices[device] = MidiOut.DeviceInfo(device);

            return devices;
        }
    }
}