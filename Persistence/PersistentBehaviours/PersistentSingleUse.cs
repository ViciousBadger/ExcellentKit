// NOTE: In case the signal is active WHILE it is persisted,
// when it is applied again it will act as if it was also deactivated.
// May solve this some day, but for now, dont do anything super important on deactivation of persistent signals..

using MemoryPack;
using UnityEngine;

namespace ExcellentKit
{
    [RequireComponent(typeof(SignalPipeWithSingleUse))]
    public class PersistentSingleUse : PersistentBehaviour<PersistentSingleUseData>
    {
        private SignalPipeWithSingleUse _singleUsePipe;

        protected override void Awake()
        {
            base.Awake();
            _singleUsePipe = GetComponent<SignalPipeWithSingleUse>();
        }

        protected override void Apply(PersistentSingleUseData savedData)
        {
            _singleUsePipe.Used = savedData.Used;
        }

        protected override PersistentSingleUseData Persist()
        {
            return new PersistentSingleUseData { Used = _singleUsePipe.Used };
        }
    }

    [MemoryPackable]
    public readonly partial struct PersistentSingleUseData
    {
        public readonly bool Used { get; init; }
    }
}
