using System.Collections.Generic;
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
            _singleUsePipe.UsedSignalTypes = new HashSet<SignalType>(savedData.UsedSignalTypes);
        }

        protected override PersistentSingleUseData Persist()
        {
            return new PersistentSingleUseData
            {
                UsedSignalTypes = new HashSet<SignalType>(_singleUsePipe.UsedSignalTypes)
            };
        }
    }

    [MemoryPackable]
    public readonly partial struct PersistentSingleUseData
    {
        public readonly HashSet<SignalType> UsedSignalTypes { get; init; }
    }
}
