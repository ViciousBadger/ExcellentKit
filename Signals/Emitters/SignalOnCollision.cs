using UnityEngine;

namespace ExcellentKit
{
    /// <summary>
    /// <para>
    /// Emits Activate/Deactivate signals when recieving OnCollisionEnter/OnCollisionExit messages.
    /// </para>
    /// <para>
    /// The signal strength is used to store the collision's "impulse magnitude" (how fast did we collide)
    /// </para>
    /// </summary>
    public class SignalOnCollision : MultiTrackingSignalEmitter<Collider>
    {
        private void OnCollisionEnter(Collision collision)
        {
            ActivateTracked(collision.collider, new() { Strength = collision.impulse.magnitude, });
        }

        private void OnCollisionExit(Collision collision)
        {
            DeactivateTracked(collision.collider, new() { Strength = collision.impulse.magnitude });
        }

        protected override string GetLabelTextForTarget(SignalReciever target)
        {
            return "On collision";
        }
    }
}
