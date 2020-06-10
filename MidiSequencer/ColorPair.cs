using System.Windows.Media;

namespace MidiSequencer
{
    /// <summary>
    ///     Holds pair of SolidColorBrush.
    /// </summary>
    public struct ColorPair
    {
        /// <summary>
        ///     SolidColorBrush used for highlighted elements.
        /// </summary>
        public SolidColorBrush On;

        /// <summary>
        ///     SolidColorBrush used for not elements that are not highlighted.
        /// </summary>
        public SolidColorBrush Off;
    }
}