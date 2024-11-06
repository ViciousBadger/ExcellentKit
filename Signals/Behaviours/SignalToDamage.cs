using ExcellentGame;
using UnityEngine;

namespace ExcellentKit
{
    public class SignalToDamage : SignalBehaviour
    {
        [SerializeField]
        DamageSource _source;

        protected override void OnSignalRecieved(Signal signal)
        {
            if (signal.IsPlayer(out PlayerActor player))
            {
                switch (signal.Type)
                {
                    case SignalType.Activate:
                        player.Mortality.AddDamageSource(_source);
                        break;
                    case SignalType.Deactivate:
                        player.Mortality.RemoveDamageSource(_source);
                        break;
                }
            }
        }

        private void OnDrawGizmos()
        {
            if (_source.DamageType)
            {
                GizmosExtra.ColorPaletteSignalBehaviour();
                GizmosExtra.DrawLabel(
                    transform.position,
                    string.Format("Damage ({0})", _source.DamageType.name)
                );
            }
            else
            {
                GizmosExtra.ColorPaletteError();
                GizmosExtra.DrawLabel(transform.position, "Damage type not set!");
            }
        }
    }
}
