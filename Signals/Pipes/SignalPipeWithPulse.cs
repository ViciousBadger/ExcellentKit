using System.Collections.Generic;
using UnityEngine;

namespace ExcellentKit
{
    /// <summary>
    /// For each active recieved signal, this pipe will emit "pulses" of regular (or randomized) interval and length.
    /// </summary>
    public class SignalPipeWithPulse : SignalPipe
    {
        [SerializeField]
        private float _minPulseInterval = 9f;

        [SerializeField]
        private float _maxPulseInterval = 9f;

        [SerializeField]
        private float _minPulseLength = 1f;

        [SerializeField]
        private float _maxPulseLength = 1f;

        [SerializeField]
        private bool _emitInitialPulse = false;

        private sealed class PulseEmitter
        {
            public ActivationSignal SourceSignal { get; init; }
            public float Timer { get; set; }
            public uint? EmittedSignalId { get; set; }
        }

        private readonly Dictionary<uint, PulseEmitter> _activePulses = new();

        private void Update()
        {
            foreach (var emitter in _activePulses.Values)
            {
                emitter.Timer -= Time.deltaTime;
                if (emitter.Timer <= 0f)
                {
                    if (emitter.EmittedSignalId is uint)
                    {
                        DeactivatePulseEmitter(emitter);
                    }
                    else
                    {
                        ActivatePulseEmitter(emitter);
                    }
                }
            }
        }

        private void ActivatePulseEmitter(PulseEmitter emitter)
        {
            var newId = SignalId.Next();
            Emit(new ActivationSignal(newId, emitter.SourceSignal.Args with { }));
            emitter.EmittedSignalId = newId;
            emitter.Timer = Random.Range(_minPulseLength, _maxPulseLength);
        }

        private void DeactivatePulseEmitter(PulseEmitter emitter)
        {
            Emit(new DeactivationSignal((uint)emitter.EmittedSignalId));
            emitter.EmittedSignalId = null;
            emitter.Timer = Random.Range(_minPulseInterval, _maxPulseInterval);
        }

        protected override void OnSignalRecieved(Signal signal)
        {
            switch (signal)
            {
                case ActivationSignal(uint id):
                    var newEmitter = new PulseEmitter() { SourceSignal = (ActivationSignal)signal };
                    if (_emitInitialPulse)
                    {
                        ActivatePulseEmitter(newEmitter);
                    }
                    else
                    {
                        newEmitter.Timer = Random.Range(_minPulseInterval, _maxPulseInterval);
                    }
                    _activePulses.Add(id, newEmitter);
                    break;
                case DeactivationSignal(uint id):
                    if (_activePulses.TryGetValue(id, out PulseEmitter emitter))
                    {
                        if (emitter.EmittedSignalId is uint emittedId)
                        {
                            Emit(new DeactivationSignal(emittedId));
                        }
                        _activePulses.Remove(id);
                    }
                    break;
            }
        }

        protected override string GetLabelTextForTarget(SignalReciever target)
        {
            return string.Format(
                "Pulse\nInteral: {0}s-{1}s\nLength: {2}s-{3}s",
                _minPulseInterval,
                _maxPulseInterval,
                _minPulseLength,
                _maxPulseLength
            );
        }
    }
}
