using ExcellentGame;
using UnityEngine;

namespace ExcellentKit
{
    public class SignalToInstakill : SignalBehaviour
    {
        protected override void OnSignalRecieved(Signal signal)
        {
            if (signal.Type == SignalType.Activate)
            {
                if (signal.IsPlayer(out PlayerActor player))
                {
                    player.Mortality.Die();
                }
            }
            // NOTE: Deactivation is ignored.. player is hopefully dead and gone by then.
        }

        private void OnDrawGizmos()
        {
            GizmosExtra.ColorPaletteSignalBehaviour();
            GizmosExtra.DrawLabel(transform.position, "Instakill");
        }
    }
}
