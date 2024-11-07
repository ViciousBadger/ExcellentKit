using ExcellentGame;
using UnityEngine;

namespace ExcellentKit
{
    public class SignalToInstakill : SignalBehaviour
    {
        protected override void OnSignalRecieved(Signal signal)
        {
            if (
                signal is ActivationSignal activationSignal
                && activationSignal.Args.SubjectIsPlayer(out PlayerActor player)
            )
            {
                player.Mortality.Die();
            }
        }

        private void OnDrawGizmos()
        {
            GizmosExtra.ColorPaletteSignalBehaviour();
            GizmosExtra.DrawLabel(transform.position, "Instakill");
        }
    }
}
