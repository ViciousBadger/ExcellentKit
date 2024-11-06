using LitMotion;
using UnityEngine;

namespace ExcellentKit
{
    public class SignalToAmbientLight : SignalBehaviour
    {
        [SerializeField]
        private Color _targetColor = new(0f, 0f, 0f, 1f);

        [SerializeField]
        private float _changeTime = 1f;

        private Color _originalColor;
        private MotionHandle _activeHandle;

        protected override void Awake()
        {
            base.Awake();
            _originalColor = RenderSettings.ambientLight;
        }

        protected override void OnSignalRecieved(Signal signal)
        {
            if (_activeHandle.IsActive())
            {
                _activeHandle.Cancel();
            }

            switch (signal.Type)
            {
                case SignalType.Activate:
                    _activeHandle = LMotion
                        .Create(_originalColor, _targetColor, _changeTime)
                        .Bind(x => RenderSettings.ambientLight = x);
                    break;
                case SignalType.Deactivate:
                    _activeHandle = LMotion
                        .Create(_targetColor, _originalColor, _changeTime)
                        .Bind(x => RenderSettings.ambientLight = x);
                    break;
            }
        }

        public void OnDrawGizmos()
        {
            GizmosExtra.ColorPaletteSignalBehaviour();
            GizmosExtra.DrawLabel(
                transform.position,
                string.Format(
                    "Ambient light\nR{0} G{1} B{2} A{3}",
                    _targetColor.r,
                    _targetColor.g,
                    _targetColor.b,
                    _targetColor.a
                )
            );
        }
    }
}
