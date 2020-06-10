using System;

namespace MidiSequencer
{
    internal class CorruptedFileException : Exception
    {
        public CorruptedFileException() : base("Selected file was corrupted or is not a valid project file")
        {
        }
    }
}