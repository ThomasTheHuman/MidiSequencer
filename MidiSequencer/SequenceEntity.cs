namespace MidiSequencer
{
    /// <summary>
    ///     Serialized version of Sequence, used for saving and loading from XML file.
    /// </summary>
    public class SequenceEntity
    {
        /// <summary>
        ///     Type of sequence this entity corresponds to.
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        ///     Gate time in milliseconds.
        /// </summary>
        public int Gate { get; set; }

        /// <summary>
        ///     Amount of steps in bar.
        /// </summary>
        public int Beats { get; set; }

        /// <summary>
        ///     Amount og bars in sequence.
        /// </summary>
        public int Bars { get; set; }

        /// <summary>
        ///     Information about connected midi input device.
        /// </summary>
        public MidiInterfaceEntity Input { get; set; }

        /// <summary>
        ///     Information about connected midi output device.
        /// </summary>
        public MidiInterfaceEntity Output { get; set; }
    }
}