using ExcellentGame;
using UnityEngine;

namespace ExcellentKit
{
    public class SignalToPlayerVelocity : SignalBehaviour
    {
        [SerializeField]
        private Vector3 _newVelocity = Vector3.up;

        [SerializeField]
        private bool _relativeToRotation = true;

        protected override void OnSignalRecieved(Signal signal)
        {
            if (signal.Type == SignalType.Activate && signal.IsPlayer(out PlayerActor player))
            {
                player.Movement.ForceVelocity(GetNewVelocity());
            }
            // NOTE: We ignore Deactivate, I don't see a reason for handling it atm
        }

        private Vector3 GetNewVelocity()
        {
            return _relativeToRotation ? transform.rotation * _newVelocity : _newVelocity;
        }

        private void OnDrawGizmos()
        {
            GizmosExtra.ColorPaletteSignalBehaviour();
            GizmosExtra.DrawLabel(transform.position, "Set player velocity");
            GizmosExtra.DrawArrow(
                transform.position,
                transform.position + GetNewVelocity(),
                string.Format(
                    "Relative: {0}\nX {1} Y {2} Z {3}",
                    _relativeToRotation,
                    _newVelocity.x,
                    _newVelocity.y,
                    _newVelocity.z
                ),
                1f / _newVelocity.magnitude
            );
        }
    }
}
