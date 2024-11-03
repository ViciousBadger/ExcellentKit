using MemoryPack;
using UnityEngine;

namespace ExcellentGame
{
    public class PersistentActive : PersistentBehaviour<PersistentActiveData>
    {
        protected override void Apply(PersistentActiveData savedData)
        {
            gameObject.SetActive(savedData.IsActive);
        }

        protected override PersistentActiveData Persist()
        {
            return new PersistentActiveData { IsActive = gameObject.activeSelf };
        }
    }

    [MemoryPackable]
    public readonly partial struct PersistentActiveData
    {
        public bool IsActive { get; init; }
    }
}
