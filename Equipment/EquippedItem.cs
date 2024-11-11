using System;
using UnityEngine;

namespace ExcellentKit
{
    public abstract class EquippedItem : MonoBehaviour
    {
        public IPlayer Owner { get; set; }

        protected virtual void OnDrawGizmos()
        {
            Gizmos.color = Color.green;
            Gizmos.DrawSphere(transform.position, 0.2f);
            Gizmos.DrawLine(transform.position, transform.position + Vector3.forward);
            Gizmos.DrawLine(
                transform.position,
                transform.position + new Vector3(0.5f, -0.2f, 0.7f)
            );
            Gizmos.DrawLine(
                transform.position,
                transform.position + new Vector3(-0.5f, -0.2f, 0.7f)
            );
            Gizmos.color = Color.yellow;
            Gizmos.DrawFrustum(transform.position, 72f, 2f, 0.1f, 16f / 9f);
        }

        public abstract void StartItemUse();

        public abstract void EndItemUse();
    }
}
