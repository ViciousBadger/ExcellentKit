using ExcellentGame;
using UnityEngine;

namespace ExcellentKit
{
    public class SignalToPlayerTeleport : SignalBehaviour
    {
        [SerializeField]
        private Transform _targetLocation;

        [SerializeField]
        private bool _rotatePlayer;

        protected override void OnSignalRecieved(Signal signal)
        {
            if (
                signal is ActivationSignal activationSignal
                && activationSignal.Args.SubjectIsPlayer(out Player player)
            )
            {
                player.Movement.Teleport(_targetLocation.position);
                if (_rotatePlayer)
                {
                    player.Sight.TargetPitchYaw = new Vector2(
                        _targetLocation.rotation.eulerAngles.y,
                        _targetLocation.rotation.eulerAngles.z
                    );
                }
            }
        }

        private void OnDrawGizmos()
        {
            GizmosExtra.ColorPaletteSignalBehaviour();
            GizmosExtra.DrawLabel(transform.position, "Teleport player");
            if (_targetLocation)
            {
                GizmosExtra.DrawArrow(
                    transform.position,
                    _targetLocation.transform.position,
                    "Target teleportation location"
                );
                if (_rotatePlayer)
                {
                    GizmosExtra.ColorPaletteGameplay();
                    GizmosExtra.DrawArrow(
                        _targetLocation.position,
                        _targetLocation.position + _targetLocation.rotation * Vector3.forward,
                        "Target teleportation rotation",
                        1.0f
                    );
                }
            }
            else
            {
                GizmosExtra.ColorPaletteError();
                GizmosExtra.DrawLabel(transform.position, "No TP Target");
            }
        }
    }
}
