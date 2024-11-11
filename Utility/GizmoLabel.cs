using UnityEngine;

namespace ExcellentKit
{
    public class GizmoLabel : MonoBehaviour
    {
        [SerializeField]
        private string _text;

        [SerializeField]
        private Color _color = Color.white;

        private void OnDrawGizmos()
        {
            Gizmos.color = _color;
            GizmosExtra.DrawLabel(transform.position, _text);
        }
    }
}
