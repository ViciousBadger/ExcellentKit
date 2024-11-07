using ExcellentGame;
using UnityEngine;

namespace ExcellentKit
{
    public class SignalToInstakill : SignalBehaviour
    {
        [SerializeField]
        private SignalType _killOn = SignalType.Activate;

        protected override void OnSignalRecieved(Signal signal)
        {
            if (signal.IsPlayer(out PlayerActor player) && signal.Type == _killOn)
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
