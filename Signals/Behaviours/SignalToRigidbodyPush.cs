using UnityEngine;

namespace ExcellentKit
{
    public class SignalToRigidbodyPush : SignalBehaviour
    {
        public enum RigidbodyPushSpace
        {
            World,
            RelativeToRigidbody,
            RelativeToThisObject,
        }

        [SerializeField]
        private Rigidbody _target;

        [SerializeField]
        private RigidbodyPushSpace _mode;

        [SerializeField]
        private Vector3 _pushDirection = Vector3.forward;

        [SerializeField]
        private float _pushForce = 1f;

        [SerializeField]
        private float _pushAngleSpread = 0f;

        protected override void OnSignalRecieved(Signal signal)
        {
            if (_target != null && signal is ActivationSignal)
            {
                var dir = _mode switch
                {
                    RigidbodyPushSpace.World => Quaternion.LookRotation(_pushDirection.normalized),
                    RigidbodyPushSpace.RelativeToRigidbody
                        => _target.rotation * Quaternion.LookRotation(_pushDirection.normalized),
                    RigidbodyPushSpace.RelativeToThisObject
                        => transform.rotation * Quaternion.LookRotation(_pushDirection.normalized),
                    _ => throw new System.NotImplementedException()
                };

                var dirWithSpread = Quaternion.RotateTowards(
                    dir,
                    Random.rotation,
                    Random.Range(0f, _pushAngleSpread)
                );

                _target.AddForce(
                    dirWithSpread * Vector3.forward * _pushForce,
                    ForceMode.VelocityChange
                );
            }
        }

        private void OnDrawGizmos()
        {
            if (_target)
            {
                GizmosExtra.ColorPaletteSignalBehaviour();
                GizmosExtra.DrawArrow(
                    transform.position,
                    transform.position + _pushDirection.normalized,
                    string.Format("Push (Force: {0})", _pushForce),
                    1f
                );
            }
            else
            {
                GizmosExtra.ColorPaletteError();
                GizmosExtra.DrawLabel(transform.position, "Push target not set!");
            }
        }
    }
}
