using MemoryPack;
using UnityEngine;

namespace ExcellentGame
{
    /// <summary>
    /// Base class for a MonoBehaviour that can be "persisted" by storing data in a GameState.
    /// </summary>
    public abstract class PersistentBehaviour<TState> : PersistentBehaviour
        where TState : struct, IMemoryPackable<TState>
    {
        protected abstract TState Persist();
        protected abstract void Apply(TState persistedState);

        public override byte[] PersistAndSerialize()
        {
            return MemoryPackSerializer.Serialize(Persist());
        }

        public override void DeserializeAndApply(byte[] rawBytes)
        {
            try
            {
                var deserializedState = MemoryPackSerializer.Deserialize<TState>(rawBytes);
                Apply(deserializedState);
            }
            catch (MemoryPackSerializationException ex)
            {
                Debug.LogError(
                    string.Format(
                        "Failed to deserialize {0} on persistent behaviour with identifier {1}:\n{2}",
                        GetType().Name,
                        ObjectIdentifier.IdString,
                        ex.ToString()
                    )
                );
            }
        }
    }

    /// <summary>
    /// Abstract base for the generic version of PersistentBehaviour.
    /// </summary>
    [RequireComponent(typeof(PersistentObjectIdentifier))]
    public abstract class PersistentBehaviour : MonoBehaviour
    {
        public PersistentObjectIdentifier ObjectIdentifier { get; private set; }

        protected virtual void Awake()
        {
            ObjectIdentifier = GetComponent<PersistentObjectIdentifier>();
        }

        public Hash128 ComputeIdentifier()
        {
            return Hash128.Compute(GetType().Name);
        }

        public abstract byte[] PersistAndSerialize();
        public abstract void DeserializeAndApply(byte[] rawBytes);
    }
}
