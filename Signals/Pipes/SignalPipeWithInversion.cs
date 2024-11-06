using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

namespace ExcellentKit
{
    /// <summary>
    /// <para>
    /// Emits Deactivate signals when recieving Activate signals and vice versa. Emitted signals will have new IDs.
    /// </para>
    /// <para>
    /// It is ensured that each emitted Activate/Deactivate signal is still correctly "paired".
    /// </para>
    /// </summary>
    public class SignalPipeWithInversion : SignalPipe
    {
        [SerializeField]
        private bool _activateOnStart = false;

        private readonly Queue<uint> _activeSignals = new();

        [UsedImplicitly]
        private void Start()
        {
            if (_activateOnStart)
            {
                Activate(new Signal());
            }
        }

        protected override void OnSignalRecieved(Signal signal)
        {
            switch (signal.Type)
            {
                case SignalType.Activate:
                    Deactivate(signal);
                    break;
                case SignalType.Deactivate:
                    Activate(signal);
                    break;
            }
        }

        private void Activate(Signal original)
        {
            var newId = SignalId.Next();
            _activeSignals.Enqueue(newId);
            Emit(original with { Id = newId, Type = SignalType.Activate });
        }

        private void Deactivate(Signal original)
        {
            if (_activeSignals.Count > 0)
            {
                var takenId = _activeSignals.Dequeue();
                Emit(original with { Id = takenId, Type = SignalType.Deactivate });
            }
        }

        protected override string GetLabelTextForTarget(SignalReciever target)
        {
            return "Inverted";
        }
    }
}
