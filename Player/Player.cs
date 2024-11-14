using UnityEngine;

namespace ExcellentKit
{
    /// <summary>
    /// Base class for a "player" component that exposes core player functionality. Since player scripts tend to get very complex, it is good practice to only expose what is expected to be interfaced with by the outside world.
    /// </summary>
    public abstract class Player : MonoBehaviour
    {
        public abstract IPlayerInventory Inventory { get; }
        public abstract IPlayerMortality Mortality { get; }
        public abstract IPlayerSight Sight { get; }
        public abstract IPlayerMovement Movement { get; }
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

    public interface IPlayerSight
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
