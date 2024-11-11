using UnityEngine;

namespace ExcellentKit
{
    public interface IPlayer
    {
        public GameObject GameObject { get; }
        public IPlayerInventory Inventory { get; }
        public IPlayerMortality Mortality { get; }
        public IPlayerLooking Looking { get; }
        public IPlayerMovement Movement { get; }
    }

    public interface IPlayerInventory
    {
        public Collection Collection { get; }

        public void AddEquipment(Equipable newEquipable, bool pickUp = false);
    }

    public interface IPlayerMortality
    {
        public Checkpoint CurrentCheckpoint { get; set; }
        public void AddDamageSource(DamageSource source);
        public void RemoveDamageSource(DamageSource source);
        public void Die();
        public void Resurrect();
        // TODO: instant damage (of certain type)
    }

    public class PlayerFovModifier
    {
        public float FovScale { get; set; } = 1f;
    }

    public interface IPlayerLooking
    {
        public Vector2 TargetPitchYaw { get; set; }
        public void LockMouse();
        public void UnlockMouse();
        public void AddFovModifier(PlayerFovModifier modifier);
        public void RemoveFovModifier(PlayerFovModifier modifier);
    }

    public interface IPlayerMovement
    {
        public void Teleport(Vector3 position);
        public void ForceVelocity(Vector3 newVelocity);
    }
}
