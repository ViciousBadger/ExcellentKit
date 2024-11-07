using System.Collections.Generic;
using UnityEngine;

namespace ExcellentKit
{
    /// <summary>
    /// Emits recieved signals if their strength is within a certain range.
    /// </summary>
    public class SignalPipeWithStrengthFilter : SignalPipe
    {
        [SerializeField]
        private float _minRequiredStrength;

        [SerializeField]
        private float _maxRequiredStrength;

        private readonly HashSet<uint> _activeSignals = new();

        protected override void OnSignalRecieved(Signal signal)
        {
            switch (signal)
            {
                case ActivationSignal(uint id, SignalArgs args):
                    if (
                        args.Strength >= _minRequiredStrength
                        && args.Strength <= _maxRequiredStrength
                    )
                    {
                        _activeSignals.Add(id);
                        Emit(signal);
                    }
                    break;
                case DeactivationSignal(uint id):
                    if (_activeSignals.Contains(id))
                    {
                        _activeSignals.Remove(id);
                        Emit(signal);
                    }
                    break;
            }
        }

        protected override string GetLabelTextForTarget(SignalReciever target)
        {
            return string.Format(
                "Strength filter\n >={0}, <={1}",
                _minRequiredStrength,
                _maxRequiredStrength
            );
        }
    }
}
