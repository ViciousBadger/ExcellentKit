using System.Collections.Generic;
using ExcellentGame;
using UnityEngine;

namespace ExcellentKit
{
    public class SignalToDamage : SignalBehaviour
    {
        [SerializeField]
        DamageSource _source;

        private readonly Dictionary<uint, IPlayer> _activations = new();

        protected override void OnSignalRecieved(Signal signal)
        {
            switch (signal)
            {
                case ActivationSignal(uint id, SignalArgs args):
                    if (args.SubjectIsPlayer(out IPlayer player))
                    {
                        _activations.Add(id, player);
                        player.Mortality.AddDamageSource(_source);
                    }
                    break;
                case DeactivationSignal(uint id):
                    if (_activations.TryGetValue(id, out IPlayer hurtingPlayer))
                    {
                        _activations.Remove(id);
                        hurtingPlayer.Mortality.RemoveDamageSource(_source);
                    }
                    break;
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
