using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Xml.Serialization;
using Microsoft.Win32;

namespace MidiSequencer
{
    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        ///     Initializes window.
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
            AddDrumTrackButton.Click += AddSequenceHandler<DrumSequence>;
            AddMonoTrackButton.Click += AddSequenceHandler<MonoSequence>;
            AddPolyTrackButton.Click += AddSequenceHandler<PolySequence>;
            BpmBox.Text = "140";
            BpmBox.TextChanged += OnBpmChange;

            PlayButton.Click += PlayButton_Click;
            PauseButton.Click += PauseButton_Click;
            StopButton.Click += StopButton_Click;

            SaveButton.Click += SaveButton_Click;
            LoadButton.Click += LoadButton_Click;
        }

        private void LoadButton_Click(object sender, RoutedEventArgs e)
        {
            (Sequencer.ClockSequence as ClockSequence)?.Stop();

            var openFileDialog = new OpenFileDialog
            {
                Filter = "XML files (*.xml)|*.xml"
            };
            if (openFileDialog.ShowDialog() != true) return;
            var serializer = new XmlSerializer(typeof(SequencerEntity));

            var aFile = new FileStream(openFileDialog.FileName, FileMode.Open);
            var buffer = new byte[aFile.Length];
            aFile.Read(buffer, 0, (int) aFile.Length);
            var stream = new MemoryStream(buffer);

            try
            {
                var entity = (SequencerEntity) serializer.Deserialize(stream);
                Sequencer.Deserialize(entity);

                CurrentFileText.Text = openFileDialog.FileName;
            }
            catch (CorruptedFileException corruptedFileException)
            {
                MessageBox.Show(corruptedFileException.Message,
                    "Cannot load file", MessageBoxButton.OK, MessageBoxImage.Error);
                Debug.WriteLine(corruptedFileException.Message);
            }
            catch (Exception exception)
            {
                MessageBox.Show(Properties.Resources.Cannot_deserialize_specified_file__exception__
                                + exception.Message,
                    "Cannot load file", MessageBoxButton.OK, MessageBoxImage.Error);
                Debug.WriteLine(Properties.Resources.Cannot_deserialize_specified_file__exception__
                                + exception.Message);
            }
            finally
            {
                aFile.Close();
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            (Sequencer.ClockSequence as ClockSequence)?.Stop();

            var saveFileDialog = new SaveFileDialog
            {
                Filter = "XML files (*.xml)|*.xml"
            };
            if (saveFileDialog.ShowDialog() != true) return;
            Sequencer.Save(saveFileDialog.FileName);

            CurrentFileText.Text = saveFileDialog.FileName;
        }

        private void StopButton_Click(object sender, RoutedEventArgs e)
        {
            (Sequencer.ClockSequence as ClockSequence)?.Stop();
        }

        private void PauseButton_Click(object sender, RoutedEventArgs e)
        {
            (Sequencer.ClockSequence as ClockSequence)?.Pause();
        }

        private void PlayButton_Click(object sender, RoutedEventArgs e)
        {
            (Sequencer.ClockSequence as ClockSequence)?.Start();
        }

        private void AddSequenceHandler<T>(object sender, RoutedEventArgs e) where T : Sequence
        {
            Sequencer.AddSequence<T>();
        }

        /// <summary>
        ///     Opens toolbar for specified sequence.
        /// </summary>
        /// <param name="s">Sequence for which to open toolbar.</param>
        public void OpenToolbar(Sequence s)
        {
            SequenceToolbar.SetSequence(s);
        }

        private void OnBpmChange(object sender, TextChangedEventArgs e)
        {
            try
            {
                var bpm = int.Parse(BpmBox.Text);
                if (bpm > 300)
                {
                    BpmBox.Text = "300";
                    bpm = 300;
                }

                Sequencer.Bpm = bpm;
                (Sequencer.ClockSequence as ClockSequence)?.ReloadTimer();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        private void MainWindow_Closing(object sender, CancelEventArgs e)
        {
            Debug.WriteLine(Properties.Resources.Close_called__stopping_threads_);
            (Sequencer.ClockSequence as ClockSequence)?.Stop();
            Debug.WriteLine(Properties.Resources.Threads_stopped__closing_application_);
        }
    }
}