using System.Collections.Generic;
using System.Diagnostics;
using MidiSequencer.Properties;
using NAudio.Midi;

namespace MidiSequencer
{
    /// <summary>
    ///     Class used for interfacing with MIDI input devices
    /// </summary>
    public class MidiInInterface : MidiInterface
    {
        private MidiIn _connection;

        /// <summary>
        ///     Stores unhandled received events.
        /// </summary>
        public List<MidiEvent> EventPool { get; } = new List<MidiEvent>();

        /// <summary>
        ///     Stores amount of notes that are currently on.
        /// </summary>
        public int NoteCount { get; private set; }

        /// <inheritdoc />
        public override void ConnectToDevice(int deviceId)
        {
            try
            {
                _connection = new MidiIn(deviceId);
                _connection.MessageReceived += Connection_MessageReceived;
                Device = deviceId;
                _connection.Start();
            }
            catch
            {
                Debug.WriteLine(Resources.Cant_connect_to_midi_in_device + deviceId);
            }
        }

        private void Connection_MessageReceived(object sender, MidiInMessageEventArgs e)
        {
            switch (e.MidiEvent.CommandCode)
            {
                case MidiCommandCode.NoteOn:
                {
                    if (Channel == 0 || Channel == e.MidiEvent.Channel) EventPool.Add(e.MidiEvent);

                    NoteCount++;
                    break;
                }
                case MidiCommandCode.NoteOff:
                    NoteCount--;
                    break;
            }
        }

        /// <summary>Gets connected MIDI input devices</summary>
        /// <returns>Array of MidiInCapabilities</returns>
        public static MidiInCapabilities[] GetMidiInDevices()
        {
            var devices = new MidiInCapabilities[MidiIn.NumberOfDevices];

            for (var device = 0; device < MidiIn.NumberOfDevices; device++) devices[device] = MidiIn.DeviceInfo(device);

            return devices;
        }
    }
}