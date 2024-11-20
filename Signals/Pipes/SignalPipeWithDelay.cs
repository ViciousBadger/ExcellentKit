using System.Collections.Generic;
using UnityEngine;

namespace ExcellentKit
{
    /// <summary>
    /// Emits all recieved signals with a configurable delay.
    /// </summary>
    public class SignalPipeWithDelay : SignalPipe
    {
        [SerializeField]
        private float _delay = 0;

        private sealed class DelayedSignal
        {
            public float DelayTimer { get; set; }
            public Signal Signal { get; init; }
        }

        private readonly List<DelayedSignal> _delayedSignals = new();

        private void Update()
        {
            var delayedSignalsToRemove = new List<DelayedSignal>();

            foreach (var delayedSignal in _delayedSignals)
            {
                delayedSignal.DelayTimer -= Time.deltaTime;
                if (delayedSignal.DelayTimer <= 0f)
                {
                    delayedSignalsToRemove.Add(delayedSignal);
                    Emit(delayedSignal.Signal);
                }
            }

            foreach (var pulse in delayedSignalsToRemove)
            {
                _delayedSignals.Remove(pulse);
            }
        }

        protected override void OnSignalRecieved(Signal signal)
        {
            _delayedSignals.Add(new() { DelayTimer = _delay, Signal = signal });
        }

        protected override string GetLabelTextForTarget(SignalReciever target)
        {
            return string.Format("Delay ({0}s)", _delay);
        }
    }
}
