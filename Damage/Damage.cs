using System;
using UnityEngine;

namespace ExcellentKit
{
    [Serializable]
    public class DamageSource
    {
        [SerializeField]
        DamageType _damageType;

        [SerializeField]
        float _timeToDeath = 1f;

        public DamageType DamageType => _damageType;
        public float TimeToDeath => _timeToDeath;
    }

    public class DamageTypeTracker
    {
        public DamageType Type { get; private init; }
        public float Damage { get; set; }

        public DamageTypeTracker(DamageType type)
        {
            Type = type;
        }
    }
}
