using System;
using UnityEngine;
using UnityEngine.Events;

namespace ExcellentKit
{
    public abstract class DamageEffect : MonoBehaviour
    {
        public DamageTypeTracker Tracker { get; set; }
    }
}
