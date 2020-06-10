using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using MaterialDesignThemes.Wpf;

namespace MidiSequencer
{
    /// <summary>
    ///     Base UserControl for sequence's steps.
    /// </summary>
    public abstract class Step : UserControl
    {
        private readonly ColorPair _colorPair;

        /// <summary>
        ///     Step's button that this class is wrapped around.
        /// </summary>
        protected readonly Button Button;

        /// <summary>
        ///     Container that holds Button and Container.
        /// </summary>
        protected readonly Grid Container;

        /// <summary>
        ///     Border used for highlighting step while recording or playback.
        /// </summary>
        protected readonly Border Highlight;

        /// <summary>
        ///     Tells if step is enabled or disabled.
        /// </summary>
        protected bool On;

        /// <summary>
        ///     Initializes Step.
        /// </summary>
        protected Step()
        {
            Padding = new Thickness(0);
            _colorPair = Setup();
            On = false;
            Button = new Button
            {
                Width = 60,
                Height = 60,
                Padding = new Thickness(0),
                Background = _colorPair.Off,
                Foreground = Brushes.White,
                BorderThickness = new Thickness(0),
                Style = TryFindResource("MaterialDesignFlatDarkBgButton") as Style
            };
            Button.Click += Toggle;
            ButtonAssist.SetCornerRadius(Button, new CornerRadius(0));

            Highlight = new Border
            {
                Width = 60,
                Height = 60,
                Padding = new Thickness(0),
                BorderBrush = _colorPair.Off,
                BorderThickness = new Thickness(2, 0, 0, 0)
            };

            Container = new Grid
            {
                Height = 60,
                Width = 60
            };
            Container.Children.Add(Button);
            Container.Children.Add(Highlight);

            Content = Container;
        }

        /// <summary>
        ///     Sets number that is displayed on step.
        /// </summary>
        /// <param name="displayNumber">Number to be set.</param>
        public void SetNumber(int displayNumber)
        {
            Button.Content = displayNumber;
        }

        /// <summary>
        ///     Sets width of step.
        /// </summary>
        /// <param name="width">Width to set.</param>
        public void SetWidth(double width)
        {
            Button.Width = width;
            Highlight.Width = width;
            Container.Width = width;
        }

        /// <summary>
        ///     Creates a new ColorPair of this type of step.
        /// </summary>
        /// <returns>ColorPair of this type of step.</returns>
        protected abstract ColorPair Setup();

        /// <summary>
        ///     Toggles step state between enabled and disabled.
        /// </summary>
        protected void Toggle()
        {
            On = !On;
            Button.Background = On ? _colorPair.On : _colorPair.Off;
        }

        /// <summary>
        ///     Toggles step state between enabled and disabled.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Toggle(object sender, RoutedEventArgs e)
        {
            Toggle();
        }

        /// <summary>
        ///     Sets highlight on or off.
        /// </summary>
        /// <param name="mode">Desired state of highlight.</param>
        public void SetHighlight(bool mode)
        {
            Highlight.BorderBrush = mode ? Brushes.White : _colorPair.Off;
        }

        /// <summary>
        ///     Loads midi from event pool to step.
        /// </summary>
        /// <param name="sequence">Parent sequence.</param>
        public abstract void LoadMidi(Sequence sequence);

        /// <summary>
        ///     Sends note on event.
        /// </summary>
        /// <param name="sequence">Parent sequence.</param>
        public abstract void NoteOn(Sequence sequence);

        /// <summary>
        ///     Sends note off event.
        /// </summary>
        /// <param name="sequence">Parent sequence.</param>
        public abstract void NoteOff(Sequence sequence);

        /// <summary>
        ///     Serializes this instance of Step.
        /// </summary>
        /// <returns>Serialized Step.</returns>
        public abstract StepEntity Serialize();

        /// <summary>
        ///     Deserializes passed entity by applying it's values to this instance of Step.
        /// </summary>
        /// <param name="step">Serialized Step.</param>
        public abstract void Deserialize(StepEntity step);
    }
}