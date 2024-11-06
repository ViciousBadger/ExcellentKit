using System.Collections.Generic;
using UnityEngine;

namespace ExcellentKit
{
    /// <summary>
    /// Emits all recieved signals with an (optional) delay.
    /// </summary>
    public class SignalPipeWithDelay : SignalPipe
    {
        [SerializeField]
        private float _delayInSeconds = 0;

        private sealed class Pulse
        {
            public float DelayTimer { get; set; }
            public Signal Signal { get; init; }
        }

        private enum PulseType
        {
            Activate,
            Deactivate
        }

        private readonly List<Pulse> _pulses = new();

        private void Update()
        {
            var pulsesToRemove = new List<Pulse>();

            foreach (var pulse in _pulses)
            {
                pulse.DelayTimer -= Time.deltaTime;
                if (pulse.DelayTimer <= 0f)
                {
                    pulsesToRemove.Add(pulse);
                    Emit(pulse.Signal);
                }
            }

            foreach (var pulse in pulsesToRemove)
            {
                _pulses.Remove(pulse);
            }
        }

        protected override void OnSignalRecieved(Signal signal)
        {
            _pulses.Add(new() { DelayTimer = _delayInSeconds, Signal = signal });
        }

        protected override string GetLabelTextForTarget(SignalReciever target)
        {
            return string.Format("Delay ({0}s)", _delayInSeconds);
        }
    }
}
