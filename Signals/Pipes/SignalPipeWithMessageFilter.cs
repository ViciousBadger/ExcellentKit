using System.Collections.Generic;
using UnityEngine;

namespace ExcellentKit
{
    /// <summary>
    /// Emits recieved signals only if their message matches a given string.
    /// </summary>
    public class SignalPipeWithMessageFilter : SignalPipe
    {
        [SerializeField]
        private string _requiredMessage;

        private readonly HashSet<uint> _activeSignals = new();

        protected override void OnSignalRecieved(Signal signal)
        {
            switch (signal)
            {
                case ActivationSignal(uint id, SignalArgs args):
                    if (args.Message == _requiredMessage)
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
            return string.Format("Message filter: {0}", _requiredMessage);
        }
    }
}
