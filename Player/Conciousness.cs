using System.Collections.Generic;
using LitMotion;
using UnityEditor;
using UnityEngine;

namespace ExcellentKit
{
    [RequireComponent(typeof(Camera))]
    public class Conciousness : MonoBehaviour
    {
        public static Conciousness Instance { get; private set; }

        private readonly List<IPossessable> _targetStack = new();
        private Vector3 _posOnTargetChange = Vector3.zero;
        private Quaternion _rotOnTargetChange = Quaternion.identity;
        private float _fovOnTargetChange = 0f;
        private float _targetChangeProgress = 0f;
        private Camera _cameraComponent;
        private MotionHandle _activeHandle;

        public IPossessable ActiveTarget
        {
            get { return _targetStack.Count > 0 ? _targetStack[^1] : null; }
        }

        private void Awake()
        {
            Instance = this;
            _cameraComponent = GetComponent<Camera>();
        }

        private void Start()
        {
            AudioListener.volume = 0.0f;
            CameraFade.StartAlphaFade(Color.black, true, 3.0f);
        }

        private void LateUpdate()
        {
            if (AudioListener.volume < 1f)
            {
                AudioListener.volume = Mathf.Min(AudioListener.volume + Time.deltaTime / 3f, 1f);
            }

            var activeTarget = ActiveTarget;

            if (activeTarget != null)
            {
                float st = Mathf.SmoothStep(0, 1, _targetChangeProgress);
                transform.SetPositionAndRotation(
                    Vector3.Lerp(_posOnTargetChange, activeTarget.GetCameraPosition(), st),
                    Quaternion.Slerp(_rotOnTargetChange, activeTarget.GetCameraRotation(), st)
                );

                float targetFOV = Mathf.Min(
                    172f,
                    // TODO: inject base fov?
                    72f * activeTarget.GetCameraFOVModifier()
                //GameMaster.GlobalState.Settings.BaseFOV * activeTarget.GetCameraFOVModifier()
                );
                _cameraComponent.fieldOfView = Mathf.Lerp(_fovOnTargetChange, targetFOV, st);
            }
        }

        public void Possess(IPossessable target, float targetChangeTime = 0f)
        {
            if (!_targetStack.Contains(target))
            {
                ActiveTarget?.OnPossessionEnd();
                _targetStack.Add(target);
            } // Otherwise we are probably "going back down" the progression stack.

            target.OnPossessionStart();

            _posOnTargetChange = transform.position;
            _rotOnTargetChange = transform.rotation;
            _fovOnTargetChange = _cameraComponent.fieldOfView;

            if (_activeHandle.IsActive())
            {
                _activeHandle.Cancel();
            }

            if (targetChangeTime > 0f)
            {
                _targetChangeProgress = 0f;
                _activeHandle = LMotion
                    .Create(0f, 1f, targetChangeTime)
                    .WithEase(Ease.InOutQuad)
                    .WithScheduler(MotionScheduler.UpdateIgnoreTimeScale)
                    .Bind(a => _targetChangeProgress = a);
            }
            else
            {
                _targetChangeProgress = 1f;
            }
        }

        public void EndPossession(IPossessable target, float targetChangeTime = 0f)
        {
            if (!_targetStack.Contains(target))
            {
                return;
            }
            bool wasTheActiveTarget = ActiveTarget == target;
            target.OnPossessionEnd();
            _targetStack.Remove(target);
            if (wasTheActiveTarget)
            {
                Possess(ActiveTarget, targetChangeTime);
            }
        }
    }

    public interface IPossessable
    {
        public abstract void OnPossessionStart();
        public abstract void OnPossessionEnd();
        public abstract Vector3 GetCameraPosition();
        public abstract Quaternion GetCameraRotation();
        public abstract float GetCameraFOVModifier();
    }
}
