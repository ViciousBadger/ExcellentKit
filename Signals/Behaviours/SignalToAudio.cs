using System;
using System.Text;
using LitMotion;
using UnityEngine;
using Random = UnityEngine.Random;

namespace ExcellentKit
{
    [RequireComponent(typeof(AudioSource))]
    public class SignalToAudio : SignalBehaviour
    {
        private AudioSource _audioSource;

        public enum ActivateAction
        {
            AlwaysPlay,
            PlayIfStopped,
            NeverPlay
        }

        public enum DeactivateAction
        {
            Stop,
            Pause,
            KeepPlaying
        }

        [Serializable]
        public class FadeOptions
        {
            [SerializeField]
            private bool _enabled = false;

            [SerializeField]
            private float _fadeTime = 0f;

            public bool Enabled => _enabled;
            public float FadeTime => _fadeTime;
        }

        [Serializable]
        public class RandomPitchOptions
        {
            [SerializeField]
            private bool _enabled = false;

            [SerializeField]
            private float _minRandomPitch = 1f;

            [SerializeField]
            private float _maxRandomPitch = 1f;

            public bool Enabled => _enabled;
            public float MinRandomPitch => _minRandomPitch;
            public float MaxRandomPitch => _maxRandomPitch;
        }

        [Serializable]
        public class VolumeByStrengthOptions
        {
            [SerializeField]
            private bool _enabled = false;

            [SerializeField]
            private float _minSignalStrength = 0.0f;

            [SerializeField]
            private float _minSignalVolume = 0.0f;

            [SerializeField]
            private float _maxSignalStrength = 1.0f;

            [SerializeField]
            private float _maxSignalVolume = 1.0f;

            public bool Enabled => _enabled;
            public float MinSignalStrength => _minSignalStrength;
            public float MinSignalVolume => _minSignalVolume;
            public float MaxSignalStrength => _maxSignalStrength;
            public float MaxSignalVolume => _maxSignalVolume;
        }

        [SerializeField]
        private ActivateAction _onActivate = ActivateAction.AlwaysPlay;

        [SerializeField]
        private DeactivateAction _onDeactivate = DeactivateAction.KeepPlaying;

        [SerializeField]
        private FadeOptions _fadeInOnActivate = new();

        [SerializeField]
        private FadeOptions _fadeOutOnDeactivate = new();

        [SerializeField]
        private RandomPitchOptions _randomPitch = new();

        [SerializeField]
        private VolumeByStrengthOptions _adjustVolumeBySignalStrength = new();

        private MotionHandle _activeVolumeHandle;
        private float _targetVolume;

        protected override void Awake()
        {
            base.Awake();
            _audioSource = GetComponent<AudioSource>();
            _targetVolume = _audioSource.volume;
        }

        protected override void OnSignalRecieved(Signal signal)
        {
            switch (signal.Type)
            {
                case SignalType.Activate:
                    HandleActivate(signal);
                    break;
                case SignalType.Deactivate:
                    HandleDeactivate();
                    break;
            }
        }

        private void HandleActivate(Signal signal)
        {
            bool shouldPlay = _onActivate switch
            {
                ActivateAction.AlwaysPlay => true,
                ActivateAction.PlayIfStopped => !_audioSource.isPlaying,
                ActivateAction.NeverPlay => false,
                _ => throw new System.NotImplementedException(),
            };

            if (shouldPlay)
            {
                _audioSource.Play();
            }

            if (_randomPitch.Enabled)
            {
                _audioSource.pitch = Random.Range(
                    _randomPitch.MinRandomPitch,
                    _randomPitch.MaxRandomPitch
                );
            }

            if (_adjustVolumeBySignalStrength.Enabled)
            {
                _targetVolume = signal.Strength.Remap(
                    _adjustVolumeBySignalStrength.MinSignalStrength,
                    _adjustVolumeBySignalStrength.MaxSignalStrength,
                    _adjustVolumeBySignalStrength.MinSignalVolume,
                    _adjustVolumeBySignalStrength.MaxSignalVolume
                );
            }

            if (_fadeInOnActivate.Enabled)
            {
                _audioSource.volume = 0f;
                if (_activeVolumeHandle.IsActive())
                {
                    _activeVolumeHandle.Cancel();
                }
                _activeVolumeHandle = LMotion
                    .Create(0f, _targetVolume, _fadeInOnActivate.FadeTime)
                    .Bind(v => _audioSource.volume = v);
            }
            else
            {
                _audioSource.volume = _targetVolume;
            }
        }

        private void HandleDeactivate()
        {
            if (_fadeOutOnDeactivate.Enabled)
            {
                if (_activeVolumeHandle.IsActive())
                {
                    _activeVolumeHandle.Cancel();
                }
                _activeVolumeHandle = LMotion
                    .Create(_audioSource.volume, 0f, _fadeOutOnDeactivate.FadeTime)
                    .WithOnComplete(() => StopModeAction())
                    .Bind(v => _audioSource.volume = v);
            }
            else
            {
                StopModeAction();
            }
        }

        private void StopModeAction()
        {
            switch (_onDeactivate)
            {
                case DeactivateAction.Stop:
                    _audioSource.Stop();
                    break;
                case DeactivateAction.Pause:
                    _audioSource.Pause();
                    break;
                case DeactivateAction.KeepPlaying:
                    break;
            }
        }

        public void OnDrawGizmos()
        {
            GizmosExtra.ColorPaletteSignalBehaviour();

            var str = new StringBuilder();
            str.AppendLine("Audio");
            str.AppendLine(string.Format("{0}/{1}", _onActivate, _onDeactivate));

            if (_fadeInOnActivate.Enabled)
            {
                str.AppendLine(string.Format("Fade in: {0}s", _fadeInOnActivate.FadeTime));
            }

            if (_fadeOutOnDeactivate.Enabled)
            {
                str.AppendLine(string.Format("Fade out: {0}s", _fadeOutOnDeactivate.FadeTime));
            }

            if (_randomPitch.Enabled)
            {
                str.AppendLine(
                    string.Format(
                        "Random pitch: {0}-{1}",
                        _randomPitch.MinRandomPitch,
                        _randomPitch.MaxRandomPitch
                    )
                );
            }

            if (_adjustVolumeBySignalStrength.Enabled)
            {
                str.AppendLine(
                    string.Format(
                        "Volume by strength: {0}-{1} to {2}-{3}",
                        _adjustVolumeBySignalStrength.MinSignalStrength,
                        _adjustVolumeBySignalStrength.MaxSignalStrength,
                        _adjustVolumeBySignalStrength.MinSignalVolume,
                        _adjustVolumeBySignalStrength.MaxSignalVolume
                    )
                );
            }

            // Remove last line break
            str.Length--;

            GizmosExtra.DrawLabel(transform.position, str.ToString());
        }
    }
}
