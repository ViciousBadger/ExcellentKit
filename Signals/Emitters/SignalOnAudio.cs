using JetBrains.Annotations;
using UnityEngine;

namespace ExcellentKit
{
    /// <summary>
    /// Emits Activate when an audio source starts playing and Deactivate when it stops.
    /// </summary>
    [RequireComponent(typeof(AudioSource))]
    public class SignalOnAudioStop : SingleTrackingSignalEmitter
    {
        private AudioSource _audioSource;
        private bool _wasPlaying = false;

        protected void Awake()
        {
            _audioSource = GetComponent<AudioSource>();
        }

        [UsedImplicitly]
        private void Update()
        {
            if (!_wasPlaying && _audioSource.isPlaying)
            {
                ActivateTracked(new SignalArgs());
                _wasPlaying = true;
            }
            if (_wasPlaying && !_audioSource.isPlaying)
            {
                DeactivateTracked();
                _wasPlaying = false;
            }
        }

        protected override string GetLabelTextForTarget(SignalReciever target)
        {
            return "On audio stop";
        }
    }
}
