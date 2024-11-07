using UnityEngine;

namespace ExcellentKit
{
    /// <summary>
    /// Emits recieved signals, but with an assigned strength
    /// </summary>
    public class SignalPipeWithStrength : SignalPipe
    {
        [SerializeField]
        private float _strength;

        protected override string GetLabelTextForTarget(SignalReciever target)
        {
            return string.Format("With strength: {0}", _strength);
        }

        protected override void OnSignalRecieved(Signal signal)
        {
            Emit(signal with { Strength = _strength });
        }
    }
}