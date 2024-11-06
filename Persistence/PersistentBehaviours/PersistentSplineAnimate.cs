using System.Security.Permissions;
using MemoryPack;
using UnityEngine;
using UnityEngine.Splines;

namespace ExcellentKit
{
    [RequireComponent(typeof(SplineAnimate))]
    public class PersistentSplineAnimate : PersistentBehaviour<PersistentSplineAnimateData>
    {
        private SplineAnimate _splineAnimate;

        protected override void Awake()
        {
            base.Awake();
            _splineAnimate = GetComponent<SplineAnimate>();
        }

        protected override void Apply(PersistentSplineAnimateData savedData)
        {
            if (savedData.IsPlaying && !_splineAnimate.IsPlaying)
            {
                _splineAnimate.Play();
            }
            if (!savedData.IsPlaying && _splineAnimate.IsPlaying)
            {
                _splineAnimate.Pause();
            }

            _splineAnimate.NormalizedTime = savedData.NormalizedTime;
            _splineAnimate.MaxSpeed = savedData.MaxSpeed;
        }

        protected override PersistentSplineAnimateData Persist()
        {
            return new()
            {
                IsPlaying = _splineAnimate.IsPlaying,
                NormalizedTime = _splineAnimate.NormalizedTime,
                MaxSpeed = _splineAnimate.MaxSpeed
            };
        }
    }

    [MemoryPackable]
    public readonly partial struct PersistentSplineAnimateData
    {
        public bool IsPlaying { get; init; }
        public float NormalizedTime { get; init; }
        public float MaxSpeed { get; init; }
    }
}
