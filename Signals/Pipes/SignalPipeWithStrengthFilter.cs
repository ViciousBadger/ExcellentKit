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

        protected override void OnSignalRecieved(Signal signal)
        {
            if (signal.Strength >= _minRequiredStrength && signal.Strength <= _maxRequiredStrength)
            {
                Emit(signal);
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
