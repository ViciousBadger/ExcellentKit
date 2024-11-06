using System.Collections.Generic;
using UnityEngine;

namespace ExcellentKit
{
    /// <summary>
    /// <para>
    /// Emits Activate/Deactivate signals when an object is considered "visible" or "invisible". Mostly useful for optimization.
    /// </para>
    /// <para>
    /// GameObjects are usually visible if they have a mesh renderer and are inside a camera frustrum, unless hidden by the scene's occlusion culling.
    /// </para>
    /// </summary>
    public class SignalOnVisible : SignalEmitter
    {
        private readonly Queue<uint> _activeSignals = new();

        private void OnBecameVisible()
        {
            var newId = SignalId.Next();
            _activeSignals.Enqueue(newId);
            Emit(new Signal() { Id = newId, Type = SignalType.Activate });
        }

        private void OnBecameInvisible()
        {
            if (_activeSignals.TryDequeue(out uint lastId))
            {
                Emit(new Signal() { Id = lastId, Type = SignalType.Deactivate });
            }
            else
            {
                Debug.LogWarning(
                    "SignalOnVisible became invisible without first becoming visible.."
                );
            }
        }

        protected override string GetLabelTextForTarget(SignalReciever target)
        {
            return "On visible";
        }
    }
}
