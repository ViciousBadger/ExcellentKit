using MemoryPack;
using UnityEngine;

namespace ExcellentGame
{
    [RequireComponent(typeof(SingleUseSignalPipe))]
    public class PersistentSingleUse : PersistentBehaviour<PersistentSingleUseData>
    {
        private SingleUseSignalPipe _singleUsePipe;

        protected override void Awake()
        {
            base.Awake();
            _singleUsePipe = GetComponent<SingleUseSignalPipe>();
        }

        protected override void Apply(PersistentSingleUseData savedData)
        {
            _singleUsePipe.HasBeenActivated = savedData.HasBeenActivated;
            _singleUsePipe.HasBeenDeactivated = savedData.HasBeenDeactivated;
        }

        protected override PersistentSingleUseData Persist()
        {
            return new PersistentSingleUseData
            {
                HasBeenActivated = _singleUsePipe.HasBeenActivated,
                HasBeenDeactivated = _singleUsePipe.HasBeenDeactivated,
            };
        }
    }

    [MemoryPackable]
    public readonly partial struct PersistentSingleUseData
    {
        public bool HasBeenActivated { get; init; }
        public bool HasBeenDeactivated { get; init; }
    }
}
