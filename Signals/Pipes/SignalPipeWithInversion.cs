using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

namespace ExcellentKit
{
    /// <summary>
    /// <para>
    /// Emits Deactivate signals when recieving Activate signals and vice versa. Emitted signals will have new IDs, but the arguments are reused.
    /// </para>
    /// </summary>
    public class SignalPipeWithInversion : SignalPipe
    {
        [SerializeField]
        private bool _activateOnStart = false;

        private readonly Queue<uint> _emittedSignalIds = new();
        private readonly Dictionary<uint, SignalArgs> _activeRecievedSignals = new();

        [UsedImplicitly]
        private void Start()
        {
            if (_activateOnStart)
            {
                var newId = SignalId.Next();
                _emittedSignalIds.Enqueue(newId);
                Emit(new ActivationSignal(newId, new()));
            }
        }

        protected override void OnSignalRecieved(Signal signal)
        {
            switch (signal)
            {
                case ActivationSignal(uint id, SignalArgs args):
                    _activeRecievedSignals.Add(id, args);
                    if (_emittedSignalIds.Count > 0)
                    {
                        var takenId = _emittedSignalIds.Dequeue();
                        Emit(new DeactivationSignal(takenId));
                    }
                    break;
                case DeactivationSignal(uint id):
                    var newId = SignalId.Next();
                    _emittedSignalIds.Enqueue(newId);
                    if (_activeRecievedSignals.TryGetValue(id, out SignalArgs storedArgs))
                    {
                        Emit(new ActivationSignal(newId, storedArgs));
                    }
                    else
                    {
                        Emit(new ActivationSignal(newId, new()));
                    }
                    break;
            }
        }

        protected override string GetLabelTextForTarget(SignalReciever target)
        {
            return "Inverted";
        }
    }
}
