using LitMotion;
using UnityEngine;

namespace ExcellentKit
{
    [RequireComponent(typeof(Light))]
    public class SignalToLightColor : GatedSignalBehaviour
    {
        [SerializeField]
        private Color _targetColor = Color.white;

        [SerializeField]
        private float _changeTime = 1f;

        private Light _light;
        private Color _originalColor;
        private MotionHandle _activeHandle;

        protected override void Awake()
        {
            base.Awake();
            _light = GetComponent<Light>();
            _originalColor = _light.color;
        }

        protected override void OnSignalRecieved(Signal signal)
        {
            if (_activeHandle.IsActive())
            {
                _activeHandle.Cancel();
            }
            base.OnSignalRecieved(signal);
        }

        protected override void OnActivate(ActivationSignal signal)
        {
            _activeHandle = LMotion
                .Create(_originalColor, _targetColor, _changeTime)
                .Bind(x => _light.color = x);
        }

        protected override void OnDeactivate(DeactivationSignal signal)
        {
            _activeHandle = LMotion
                .Create(_targetColor, _originalColor, _changeTime)
                .Bind(x => _light.color = x);
        }

        public void OnDrawGizmos()
        {
            GizmosExtra.ColorPaletteSignalBehaviour();
            GizmosExtra.DrawLabel(
                transform.position,
                string.Format(
                    "Light color\nR{0} G{1} B{2} A{3}",
                    _targetColor.r,
                    _targetColor.g,
                    _targetColor.b,
                    _targetColor.a
                )
            );
        }
    }
}
