using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using MathNet.Numerics;
using MidiSequencer.Properties;

namespace MidiSequencer
{
    internal class SequenceTimer
    {
        private readonly Sequencer _sequencer;
        private readonly HashSet<long> _sequencesLengthsSet = new HashSet<long>();
        private readonly List<Step> _steps;
        private readonly Timer _timer;
        private int _complexStepCount;
        private int _currentStep;

        public SequenceTimer(Sequencer sequencer, List<Step> steps)
        {
            _timer = new Timer();
            _timer.Elapsed += TickEvent;

            _sequencer = sequencer;
            _steps = steps;
        }

        public void SetTimer()
        {
            var sequenceLength = (double) 60000 / _sequencer.Bpm * _steps.Count;
            _sequencesLengthsSet.Clear();
            _sequencesLengthsSet.Add(_steps.Count);
            foreach (var sequence in _sequencer.Sequences) _sequencesLengthsSet.Add(sequence.Beats * sequence.Bars);

            _complexStepCount = (int) Euclid.LeastCommonMultiple(_sequencesLengthsSet.ToArray());

            _timer.Interval = sequenceLength / _complexStepCount;
        }

        private void TickEvent(object o, EventArgs e)
        {
            if (_currentStep >= _complexStepCount) _currentStep = 0;

            foreach (var sequence in _sequencer.Sequences)
            {
                var interval = _complexStepCount / sequence.Steps.Count;
                if (_currentStep != 0 && _currentStep % interval != 0) continue;
                var stepIndex = _currentStep / interval;
                var thisStep = interval == 0 ? 0 : stepIndex;

                PlayNote(thisStep, sequence);
            }

            _currentStep++;
        }

        public void Start()
        {
            foreach (var sequence in _sequencer.Sequences) sequence.StopRecording();

            _timer.Start();
        }

        public void Stop()
        {
            foreach (var sequence in _sequencer.Sequences)
            foreach (var step in sequence.Steps)
            {
                step.SetHighlight(false);
                step.NoteOff(sequence);
            }

            _currentStep = 0;
            _timer.Stop();
        }

        public void Pause()
        {
            _timer.Stop();
        }

        private static async void PlayNote(int step, Sequence sequence)
        {
            try
            {
                Application.Current?.Dispatcher.Invoke(() =>
                {
                    if (sequence.On != true) return;
                    sequence.Steps[step]?.SetHighlight(true);
                    sequence.Steps[step]?.NoteOn(sequence);
                });

                await Task.Delay(sequence.Gate);

                Application.Current?.Dispatcher.Invoke(() =>
                {
                    sequence.Steps[step]?.NoteOff(sequence);
                    sequence.Steps[step]?.SetHighlight(false);
                    if (sequence.HotRec == true && sequence.Input.EventPool.Count > 0)
                        sequence.Steps[step]?.LoadMidi(sequence);
                });
            }
            catch (Exception ex)
            {
                Debug.WriteLine(Resources.Cannot_play_note__exception__ + ex.Message);
            }
        }
    }
}