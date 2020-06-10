using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Effects;
using MaterialDesignThemes.Wpf;

namespace MidiSequencer
{
    /// <summary>
    ///     UserControl that is a base class for sequences.
    /// </summary>
    public abstract class Sequence : UserControl
    {
        private readonly Border _highlightBorder;

        private readonly StackPanel _stepsContainer;

        /// <summary>
        ///     Button that highlights sequence and opens it's toolbar.
        /// </summary>
        protected readonly Button EditButton;

        /// <summary>
        ///     Toggle button that when turned on records all inputs while sequence is playing.
        /// </summary>
        protected readonly ToggleWithLabel HotRecButton;

        /// <summary>
        ///     Used to interact with MIDI input devices.
        /// </summary>
        public readonly MidiInInterface Input = new MidiInInterface();

        /// <summary>
        ///     Toggle button that when turned off disables all MIDI output of sequence.
        /// </summary>
        protected readonly ToggleWithLabel OnButton;

        /// <summary>
        ///     Used to interact with MIDI output devices.
        /// </summary>
        public readonly MidiOutInterface Output = new MidiOutInterface();

        /// <summary>
        ///     Button used to turn on recording.
        /// </summary>
        protected readonly Button RecordButton;

        /// <summary>
        ///     Rest button used for recording sequence.
        /// </summary>
        protected readonly Button RestButton;

        /// <summary>
        ///     Sequencer that hold this instance of Sequence.
        /// </summary>
        protected readonly Sequencer Sequencer;

        /// <summary>
        ///     UI container holding elements of sequence.
        /// </summary>
        protected readonly VirtualizingStackPanel StackPanel;

        /// <summary>
        ///     Button that removes sequence.
        /// </summary>
        protected Button DeleteButton;

        /// <summary>
        ///     Field telling if recording is enabled.
        /// </summary>
        protected bool Recording;

        /// <summary>
        ///     Field telling if rest button was clicked.
        /// </summary>
        protected bool Rest;

        /// <summary>
        ///     Initializes sequence.
        /// </summary>
        /// <param name="sequencer">Sequencer to which newly created sequence belongs.</param>
        protected Sequence(Sequencer sequencer)
        {
            Sequencer = sequencer;

            ColorPair = Setup();

            StackPanel = new VirtualizingStackPanel
            {
                Orientation = Orientation.Horizontal,
                Background = ColorPair.Off
            };
            _highlightBorder = new Border
            {
                BorderBrush = Brushes.Transparent,
                BorderThickness = new Thickness(2)
            };
            var grid = new Grid();
            grid.Children.Add(StackPanel);
            grid.Children.Add(_highlightBorder);
            Content = grid;

            OnButton = new ToggleWithLabel("ON");
            StackPanel.Children.Add(OnButton);
            OnButton.Set(true);

            HotRecButton = new ToggleWithLabel("REC");
            StackPanel.Children.Add(HotRecButton);

            RecordButton = new Button
            {
                Content = new PackIcon {Kind = PackIconKind.RecordCircle},
                Width = 60,
                Height = 60,
                Background = Brushes.Transparent,
                Style = TryFindResource("ButtonRevealStyle") as Style
            };
            RecordButton.Click += RecordButton_Click;
            ButtonAssist.SetCornerRadius(RecordButton, new CornerRadius(0));
            StackPanel.Children.Add(RecordButton);

            RestButton = new Button
            {
                Content = new PackIcon {Kind = PackIconKind.SkipNext},
                Width = 60,
                Height = 60,
                Background = Brushes.Transparent,
                Style = TryFindResource("ButtonRevealStyle") as Style
            };
            RestButton.Click += RestButton_Click;
            ButtonAssist.SetCornerRadius(RestButton, new CornerRadius(0));
            StackPanel.Children.Add(RestButton);

            StackPanel.Effect = new DropShadowEffect();
            _stepsContainer = new StackPanel
            {
                Orientation = Orientation.Horizontal
            };
            StackPanel.Children.Add(_stepsContainer);

            EditButton = new Button
            {
                Content = new PackIcon {Kind = PackIconKind.PlaylistEdit},
                Width = 60,
                Height = 60,
                Background = Brushes.Transparent,
                Style = TryFindResource("ButtonRevealStyle") as Style
            };
            EditButton.Click += OnEdit;
            ButtonAssist.SetCornerRadius(EditButton, new CornerRadius(0));
            StackPanel.Children.Add(EditButton);

            DeleteButton = new Button
            {
                Content = new PackIcon {Kind = PackIconKind.TrashCan},
                Width = 60,
                Height = 60,
                Background = Brushes.Transparent,
                Style = TryFindResource("ButtonRevealStyle") as Style
            };
            DeleteButton.Click += OnDelete;
            ButtonAssist.SetCornerRadius(DeleteButton, new CornerRadius(0));
            StackPanel.Children.Add(DeleteButton);

            if (GetType() != typeof(ClockSequence))
            {
                Beats = Sequencer.ClockSequence.Beats;
                Bars = Sequencer.ClockSequence.Bars;
            }

            StackPanel.SetValue(VirtualizingStackPanel.VirtualizationModeProperty, VirtualizationMode.Recycling);
        }

        /// <summary>
        ///     Property telling if hot recording is enabled.
        /// </summary>
        public bool? HotRec => HotRecButton.IsChecked;

        /// <summary>
        ///     Property telling if sequence is enabled.
        /// </summary>
        public bool? On => OnButton.IsChecked;

        /// <summary>
        ///     Gate time of midi events in milliseconds.
        /// </summary>
        public int Gate { get; set; } = 100;

        /// <summary>
        ///     Amount of steps in a single bar.
        /// </summary>
        public int Beats { get; protected set; } = 4;

        /// <summary>
        ///     Amount of bars in sequence.
        /// </summary>
        public int Bars { get; protected set; } = 2;

        /// <summary>
        ///     List of sequence's steps.
        /// </summary>
        public List<Step> Steps { get; } = new List<Step>();

        private ColorPair ColorPair { get; }

        private int SequenceLength()
        {
            return Beats * Bars;
        }

        private void RestButton_Click(object sender, RoutedEventArgs e)
        {
            if (Recording) Rest = true;
        }

        /// <summary>
        ///     Handles clicking record button.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected abstract void RecordButton_Click(object sender, RoutedEventArgs e);

        /// <summary>
        ///     Sets border highlight of sequence either on or off.
        /// </summary>
        /// <param name="highlighted">Desired state of highlight.</param>
        public void SetHighlight(bool highlighted)
        {
            _highlightBorder.BorderBrush = highlighted ? ColorPair.On : Brushes.Transparent;
        }

        private void OnEdit(object o, RoutedEventArgs e)
        {
            var win = (MainWindow) Window.GetWindow(this);
            win?.OpenToolbar(this);
        }

        private void OnDelete(object o, RoutedEventArgs e)
        {
            Sequencer.RemoveSequence(this);
        }

        private void RenderSteps()
        {
            _stepsContainer.Children.Clear();
            Steps.ForEach(step => { _stepsContainer.Children.Add(step); });
        }

        /// <summary>
        ///     Reloads steps if it's count changed.
        /// </summary>
        protected abstract void ReloadSteps();

        /// <summary>
        ///     Reloads steps if it's count changed.
        /// </summary>
        protected void ReloadSteps<T>() where T : new()
        {
            var stepDiff = Bars * Beats - Steps.Count;
            if (stepDiff > 0)
                for (var i = 0; i < stepDiff; i++)
                    Steps.Add(new T() as Step);
            else if (stepDiff < 0)
                for (var i = stepDiff; i < 0; i++)
                    Steps.RemoveAt(Steps.Count - 1);

            var beat = 1;
            Steps.ForEach(step =>
            {
                step.SetNumber(beat);
                beat++;
                if (beat > Beats) beat = 1;
            });

            if (GetType() != typeof(ClockSequence))
            {
                var desiredLength = Sequencer.ClockSequence.SequenceLength() * 60;
                var stepCount = Beats * Bars;
                var stepWidth = (double) desiredLength / stepCount;
                foreach (var step in Steps) step.SetWidth(stepWidth);
            }
            else
            {
                foreach (var sequence in Sequencer.Sequences) sequence.ReloadSteps();
            }

            RenderSteps();
        }

        /// <summary>
        ///     Sets amount of steps in sequence and reloads.
        /// </summary>
        /// <param name="beats">Amount of steps in a bar.</param>
        /// <param name="bars">Amount of bars in a sequence.</param>
        public void SetSteps(int beats, int bars)
        {
            Beats = beats;
            Bars = bars;
            ReloadSteps();
            ((ClockSequence) Sequencer.ClockSequence).ReloadTimer();
        }

        /// <summary>
        ///     Stops recording.
        /// </summary>
        public void StopRecording()
        {
            Recording = false;
        }

        /// <summary>
        ///     Creates a new ColorPair of this type of sequence.
        /// </summary>
        /// <returns>ColorPair of this type of sequence.</returns>
        protected abstract ColorPair Setup();

        /// <summary>
        ///     Serializes this instance of Sequence.
        /// </summary>
        /// <returns>Serialized sequence</returns>
        public abstract SequenceEntity Serialize();

        /// <summary>
        ///     Deserializes sequence by applying values from passed serialized entity to this instance of Sequence.
        /// </summary>
        /// <param name="entity">Serialized sequence</param>
        public abstract void Deserialize(SequenceEntity entity);
    }
}