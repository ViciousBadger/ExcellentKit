using Alchemy.Inspector;
using MemoryPack;
using UnityEngine;

namespace ExcellentGame
{
    [RequireComponent(typeof(AudioSource))]
    public class PersistentAudioSource : PersistentBehaviour<PersistentAudioSourceData>
    {
        private AudioSource _audioSource;

        protected override void Awake()
        {
            base.Awake();
            _audioSource = GetComponent<AudioSource>();
        }

        protected override void Apply(PersistentAudioSourceData savedData)
        {
            _audioSource.volume = savedData.Volume;
            _audioSource.pitch = savedData.Pitch;
            _audioSource.time = savedData.Time;
            _audioSource.loop = savedData.Loop;
            if (savedData.IsPlaying)
            {
                _audioSource.Play();
            }
            else
            {
                _audioSource.Pause();
            }
        }

        protected override PersistentAudioSourceData Persist()
        {
            return new PersistentAudioSourceData
            {
                Volume = _audioSource.volume,
                Pitch = _audioSource.pitch,
                Time = _audioSource.time,
                Loop = _audioSource.loop,
                IsPlaying = _audioSource.isPlaying,
            };
        }
    }

    [MemoryPackable]
    public readonly partial struct PersistentAudioSourceData
    {
        public float Volume { get; init; }
        public float Pitch { get; init; }
        public float Time { get; init; }
        public bool Loop { get; init; }
        public bool IsPlaying { get; init; }
    }
}
