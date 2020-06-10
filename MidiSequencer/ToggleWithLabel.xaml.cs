using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace MidiSequencer
{
    /// <summary>
    ///     UserControl representing a labeled ToggleButton
    /// </summary>
    public partial class ToggleWithLabel : UserControl
    {
        private readonly ToggleButton _toggle;

        /// <summary>
        ///     Initializes Control.
        /// </summary>
        /// <param name="label">Label to be displayed below ToggleButton.</param>
        public ToggleWithLabel(string label)
        {
            var recordButtonContainer = new StackPanel
            {
                Orientation = Orientation.Vertical
            };
            Content = recordButtonContainer;

            _toggle = new ToggleButton {Height = 40, Width = 60};
            recordButtonContainer.Children.Add(_toggle);

            var buttonLabel = new TextBlock {Text = label, HorizontalAlignment = HorizontalAlignment.Center};
            recordButtonContainer.Children.Add(buttonLabel);

            InitializeComponent();
        }

        /// <summary>
        ///     True if toggle is in ON position.
        /// </summary>
        public bool? IsChecked => _toggle.IsChecked;

        /// <summary>
        ///     Sets value of toggle button irrespectively of current value.
        /// </summary>
        /// <param name="val">Value to which toggle is being set.</param>
        public void Set(bool val)
        {
            _toggle.IsChecked = val;
        }
    }
}