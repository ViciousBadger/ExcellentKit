using System.Collections.Generic;
using UnityEngine;

namespace ExcellentKit
{
    /// <summary>
    /// Emits only some recieved signals.
    /// </summary>
    public class SignalPipeWithDiceroll : SignalPipe
    {
        [SerializeField, Min(1)]
        private int _sides = 6;

        [SerializeField, Min(1)]
        private int _minSuccessValue = 6;

        private readonly HashSet<uint> _activeSignals = new();

        protected override void OnSignalRecieved(Signal signal)
        {
            switch (signal)
            {
                case ActivationSignal(uint id):
                    var roll = Random.Range(1, _sides + 1);
                    if (roll >= _minSuccessValue)
                    {
                        _activeSignals.Add(id);
                        Emit(signal);
                    }
                    break;
                case DeactivationSignal(uint id):
                    if (_activeSignals.Contains(id))
                    {
                        // This is to make sure we only deactivate signals that "succeeded" the diceroll.
                        _activeSignals.Remove(id);
                        Emit(signal);
                    }
                    break;
            }
        }

        protected override string GetLabelTextForTarget(SignalReciever target)
        {
            return string.Format("Diceroll\n{0} / {1}", GetSuccessChance(), _sides);
        }

        private int GetSuccessChance()
        {
            return _sides - _minSuccessValue + 1;
        }
    }
}
