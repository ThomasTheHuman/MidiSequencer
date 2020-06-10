using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Xml.Serialization;

namespace MidiSequencer
{
    /// <summary>
    ///     UserControl that acts as a container for sequences.
    /// </summary>
    public partial class Sequencer : UserControl
    {
        /// <summary>
        ///     Initializes component and renders clock sequence.
        /// </summary>
        public Sequencer()
        {
            InitializeComponent();
            ClockSequence = AddSequence<ClockSequence>();
        }

        /// <summary>
        ///     List of sequences in sequence pool.
        /// </summary>
        public List<Sequence> Sequences { get; } = new List<Sequence>();

        /// <summary>
        ///     Holds sequencer's clock sequence.
        /// </summary>
        public Sequence ClockSequence { get; }

        /// <summary>
        ///     Holds tempo of sequencer in beats per minute.
        /// </summary>
        public int Bpm { get; set; } = 140;

        /// <summary>
        ///     Applies values from passed entity. Used for loading data from file.
        /// </summary>
        /// <param name="entity">Object holding values to apply.</param>
        /// <exception cref="CorruptedFileException">Thrown when passed object is incomplete or null.</exception>
        public void Deserialize(SequencerEntity entity)
        {
            if (entity?.Sequences == null) throw new CorruptedFileException();

            while (Sequences.Count > 0) RemoveSequence(Sequences[0]);

            var win = (MainWindow) Window.GetWindow(this);

            if (win != null) win.BpmBox.Text = entity.Bpm.ToString();

            ClockSequence.Deserialize(entity.Clock);

            foreach (var sequenceEntity in entity.Sequences)
                (typeof(Sequencer).GetMethod("AddSequence")
                    ?.MakeGenericMethod(Type.GetType(sequenceEntity.Type))
                    .Invoke(this, null) as Sequence)?.Deserialize(sequenceEntity);

            (ClockSequence as ClockSequence)?.ReloadTimer();
        }

        /// <summary>
        ///     Adds sequence to sequencer.
        /// </summary>
        /// <typeparam name="T">Type of sequence.</typeparam>
        /// <returns>Newly created sequence.</returns>
        public Sequence AddSequence<T>() where T : Sequence
        {
            var newSequence =
                Activator.CreateInstance(typeof(T), this) as Sequence;
            SequencerPanel.Children.Add(newSequence ?? throw new Exception("Cannot create sequence"));
            if (typeof(T) != typeof(ClockSequence)) Sequences.Add(newSequence);

            return newSequence;
        }

        /// <summary>
        ///     Removes sequence from sequencer.
        /// </summary>
        /// <param name="sequence">Sequence to remove.</param>
        public void RemoveSequence(Sequence sequence)
        {
            Sequences.Remove(sequence);
            SequencerPanel.Children.Remove(sequence);
        }

        /// <summary>
        ///     Serializes and saves project to XML file.
        /// </summary>
        /// <param name="fileName">Name of file to create/overwrite.</param>
        public void Save(string fileName)
        {
            var entity = new SequencerEntity
            {
                Bpm = Bpm,
                Clock = ClockSequence.Serialize() as ClockSequenceEntity,
                Sequences = Sequences.Select(seq => seq.Serialize()).ToArray()
            };

            using (var stream = File.Create(fileName))
            {
                var serializer = new XmlSerializer(typeof(SequencerEntity));
                serializer.Serialize(stream, entity);
            }
        }
    }
}