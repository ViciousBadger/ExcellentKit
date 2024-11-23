using ExcellentKit;
using UnityEngine;

namespace ExcellentGame
{
    public class SignalToEquipmentAdd : SignalBehaviour
    {
        [SerializeField]
        private Equipable _equipableToAdd;

        protected override void OnSignalRecieved(Signal signal)
        {
            if (
                signal is ActivationSignal activationSignal
                && activationSignal.Args.SubjectIsPlayer(out Player player)
            )
            {
                player.Inventory.AddEquipment(_equipableToAdd, EquipmentAddMode.PickedUp);
            }
        }
    }
}
