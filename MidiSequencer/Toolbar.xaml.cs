using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using NAudio.Midi;

namespace MidiSequencer
{
    /// <summary>
    ///     Toolbar control used to edit individual sequences.
    /// </summary>
    public partial class Toolbar : UserControl
    {
        private readonly List<string> _inChannels = new List<string>
            {"all", "1", "2", "3", "4", "5", "6", "7", "8", "9", "10", "11", "12", "13", "14", "15", "16"};

        private readonly List<string> _notes = new List<string>
            {"C", "C# / Db", "D", "D# / Eb", "E", "F", "F# / Gb", "G", "G# / Ab", "A", "A# / Bb", "B"};

        private readonly List<string> _octaves = new List<string>
            {"-1", "0", "1", "2", "3", "4", "5", "6", "7", "8", "9"};

        private readonly List<string> _outChannels = new List<string>
            {"1", "2", "3", "4", "5", "6", "7", "8", "9", "10", "11", "12", "13", "14", "15", "16"};

        private MidiInCapabilities[] _midiInDevices;
        private MidiOutCapabilities[] _midiOutDevices;

        private Sequence _targetSequence;

        /// <summary>
        ///     Initializes control.
        /// </summary>
        public Toolbar()
        {
            InitializeComponent();

            InChannelForm.ItemsSource = _inChannels;
            InChannelForm.SelectedIndex = 0;
            OutChannelForm.ItemsSource = _outChannels;
            OutChannelForm.SelectedIndex = 0;

            InNote.ItemsSource = _notes;
            InNote.SelectedIndex = 0;
            InOct.ItemsSource = _octaves;
            InOct.SelectedIndex = 5;
            OutNote.ItemsSource = _notes;
            OutNote.SelectedIndex = 0;
            OutOct.ItemsSource = _octaves;
            OutOct.SelectedIndex = 5;

            SequenceSettingsBox.Visibility = Visibility.Hidden;
            MidiInBox.Visibility = Visibility.Hidden;
            MidiOutBox.Visibility = Visibility.Hidden;
            DrumSequence.Visibility = Visibility.Hidden;

            AttachEvents();
        }

        private void OutChannelForm_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _targetSequence.Output.Channel = int.Parse(OutChannelForm.SelectedItem.ToString());
        }

        private void OutDeviceForm_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _targetSequence.Output.ConnectToDevice(OutDeviceForm.SelectedIndex);
        }

        private void OutDeviceForm_DropDownOpened(object sender, EventArgs e)
        {
            _midiOutDevices = null;
            _midiOutDevices = MidiOutInterface.GetMidiOutDevices();
            OutDeviceForm.ItemsSource = _midiOutDevices.Select(device => device.ProductName);
        }

        private void InChannelForm_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var channel = InChannelForm.SelectedItem.ToString();
            _targetSequence.Input.Channel = channel.Equals("all") ? 0 : int.Parse(channel);
        }

        private void InDeviceForm_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _targetSequence.Input.ConnectToDevice(InDeviceForm.SelectedIndex);
        }

        private void InDeviceForm_DropDownOpened(object sender, EventArgs e)
        {
            _midiInDevices = null;
            _midiInDevices = MidiInInterface.GetMidiInDevices();
            InDeviceForm.ItemsSource = _midiInDevices.Select(device => device.ProductName);
        }

        private void OnBeatsOrBarsChanged(object sender, TextChangedEventArgs e)
        {
            var beats = 1;
            var bars = 1;
            try
            {
                beats = int.Parse(Beats.Text);
                bars = int.Parse(Bars.Text);
            }
            catch (Exception)
            {
                // ignored, only fired due to bad user input.
            }

            _targetSequence?.SetSteps(beats, bars);
        }

        private void AttachEvents()
        {
            Beats.TextChanged += OnBeatsOrBarsChanged;
            Bars.TextChanged += OnBeatsOrBarsChanged;
            GateTime.TextChanged += GateTime_TextChanged;
            InDeviceForm.DropDownOpened += InDeviceForm_DropDownOpened;
            InDeviceForm.SelectionChanged += InDeviceForm_SelectionChanged;
            InChannelForm.SelectionChanged += InChannelForm_SelectionChanged;
            OutDeviceForm.DropDownOpened += OutDeviceForm_DropDownOpened;
            OutDeviceForm.SelectionChanged += OutDeviceForm_SelectionChanged;
            OutChannelForm.SelectionChanged += OutChannelForm_SelectionChanged;
            InNote.SelectionChanged += InNoteOrOctChanged;
            InOct.SelectionChanged += InNoteOrOctChanged;
            OutNote.SelectionChanged += OutNoteOrOctChanged;
            OutOct.SelectionChanged += OutNoteOrOctChanged;
            DetectInput.Click += DetectInput_Click;
        }

        private async void DetectInput_Click(object sender, RoutedEventArgs e)
        {
            _targetSequence.Input.EventPool.Clear();
            ((TextBlock) DetectInput.Content).Text = "Waiting for input...";
            while (_targetSequence.Input.EventPool.Count == 0) await Task.Delay(50);

            InNote.SelectedIndex = ((NoteOnEvent) _targetSequence.Input.EventPool.Last()).NoteNumber % 12;
            InOct.SelectedIndex = ((NoteOnEvent) _targetSequence.Input.EventPool.Last()).NoteNumber / 12;
            _targetSequence.Input.EventPool.Clear();

            ((TextBlock) DetectInput.Content).Text = "Input detected!";
            await Task.Delay(1000);
            ((TextBlock) DetectInput.Content).Text = "Automatically detect input note";
        }

        private void OutNoteOrOctChanged(object sender, SelectionChangedEventArgs e)
        {
            ((DrumSequence) _targetSequence).OutNote = OutNote.SelectedIndex + OutOct.SelectedIndex * 12;
        }

        private void InNoteOrOctChanged(object sender, SelectionChangedEventArgs e)
        {
            ((DrumSequence) _targetSequence).InNote = InNote.SelectedIndex + InOct.SelectedIndex * 12;
        }

        private void GateTime_TextChanged(object sender, TextChangedEventArgs e)
        {
            var gate = 1;
            try
            {
                gate = int.Parse(GateTime.Text);
            }
            catch (Exception)
            {
                // ignored, only fired due to bad user input.
            }

            _targetSequence.Gate = gate;
        }

        private void DetachEvents()
        {
            Beats.TextChanged -= OnBeatsOrBarsChanged;
            Bars.TextChanged -= OnBeatsOrBarsChanged;
            GateTime.TextChanged -= GateTime_TextChanged;
            InDeviceForm.DropDownOpened -= InDeviceForm_DropDownOpened;
            InDeviceForm.SelectionChanged -= InDeviceForm_SelectionChanged;
            InChannelForm.SelectionChanged -= InChannelForm_SelectionChanged;
            OutDeviceForm.DropDownOpened -= OutDeviceForm_DropDownOpened;
            OutDeviceForm.SelectionChanged -= OutDeviceForm_SelectionChanged;
            OutChannelForm.SelectionChanged -= OutChannelForm_SelectionChanged;
            InNote.SelectionChanged -= InNoteOrOctChanged;
            InOct.SelectionChanged -= InNoteOrOctChanged;
            OutNote.SelectionChanged -= OutNoteOrOctChanged;
            OutOct.SelectionChanged -= OutNoteOrOctChanged;
            DetectInput.Click -= DetectInput_Click;
        }

        /// <summary>
        ///     Opens toolbar for passed sequence.
        /// </summary>
        /// <param name="sequence">Sequence for which to open toolbar.</param>
        public void SetSequence(Sequence sequence)
        {
            //to prevent event firing
            DetachEvents();

            _targetSequence?.SetHighlight(false);
            _targetSequence = sequence;
            _targetSequence?.SetHighlight(true);

            Beats.Text = _targetSequence?.Beats.ToString() ?? "";
            Bars.Text = _targetSequence?.Bars.ToString() ?? "";
            GateTime.Text = _targetSequence?.Gate.ToString() ?? "";

            if (_targetSequence != null)
            {
                InChannelForm.SelectedIndex = _targetSequence.Input.Channel;
                OutChannelForm.SelectedIndex = _targetSequence.Output.Channel - 1;

                InDeviceForm.SelectedIndex = _targetSequence.Input.GetDevice() ?? -1;
                InDeviceForm_DropDownOpened(null, null);
                OutDeviceForm.SelectedIndex = _targetSequence.Output.GetDevice() ?? -1;
                OutDeviceForm_DropDownOpened(null, null);

                var color = "Default";
                switch (_targetSequence)
                {
                    case ClockSequence _:
                        SequenceSettingsBox.Visibility = Visibility.Visible;
                        GateTimePanel.Visibility = Visibility.Hidden;
                        MidiInBox.Visibility = Visibility.Hidden;
                        MidiOutBox.Visibility = Visibility.Hidden;
                        DrumSequence.Visibility = Visibility.Hidden;
                        break;
                    case DrumSequence drumSequence:
                        color = "Red";
                        SequenceSettingsBox.Visibility = Visibility.Visible;
                        GateTimePanel.Visibility = Visibility.Visible;
                        MidiInBox.Visibility = Visibility.Visible;
                        MidiOutBox.Visibility = Visibility.Visible;
                        DrumSequence.Visibility = Visibility.Visible;
                        InNote.SelectedIndex = drumSequence.InNote % 12;
                        InOct.SelectedIndex = drumSequence.InNote / 12;
                        OutNote.SelectedIndex = drumSequence.OutNote % 12;
                        OutOct.SelectedIndex = drumSequence.OutNote / 12;
                        break;
                    case MonoSequence _:
                        color = "Blue";
                        SequenceSettingsBox.Visibility = Visibility.Visible;
                        GateTimePanel.Visibility = Visibility.Visible;
                        MidiInBox.Visibility = Visibility.Visible;
                        MidiOutBox.Visibility = Visibility.Visible;
                        DrumSequence.Visibility = Visibility.Hidden;
                        break;
                    case PolySequence _:
                        color = "Yellow";
                        SequenceSettingsBox.Visibility = Visibility.Visible;
                        GateTimePanel.Visibility = Visibility.Visible;
                        MidiInBox.Visibility = Visibility.Visible;
                        MidiOutBox.Visibility = Visibility.Visible;
                        DrumSequence.Visibility = Visibility.Hidden;
                        break;
                    case null:
                        SequenceSettingsBox.Visibility = Visibility.Hidden;
                        GateTimePanel.Visibility = Visibility.Visible;
                        MidiInBox.Visibility = Visibility.Hidden;
                        MidiOutBox.Visibility = Visibility.Hidden;
                        DrumSequence.Visibility = Visibility.Hidden;
                        break;
                }

                var rd = new ResourceDictionary
                {
                    Source = new Uri($"/Styles/{color}.xaml", UriKind.Relative)
                };
                SequenceSettingsBox.Resources.MergedDictionaries[0] = rd;
                MidiInBox.Resources.MergedDictionaries[0] = rd;
                MidiOutBox.Resources.MergedDictionaries[0] = rd;
                DrumSequence.Resources.MergedDictionaries[0] = rd;
            }

            //re-attach event handlers
            AttachEvents();
        }
    }
}