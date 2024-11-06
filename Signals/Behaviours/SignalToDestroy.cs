using UnityEngine;

namespace ExcellentKit
{
    public class SignalToDestroy : SignalBehaviour
    {
        [SerializeField]
        private GameObject _target;

        protected override void OnSignalRecieved(Signal signal)
        {
            if (_target)
            {
                Destroy(_target);
            }
        }

        private void OnDrawGizmos()
        {
            if (_target)
            {
                GizmosExtra.ColorPaletteSignalBehaviour();
                GizmosExtra.DrawLabel(
                    transform.position,
                    string.Format("Destroy ({0})", _target.name)
                );
            }
            else
            {
                GizmosExtra.ColorPaletteError();
                GizmosExtra.DrawLabel(transform.position, "Destroy target not set!");
            }
        }
    }
}
