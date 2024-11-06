using UnityEngine;

namespace ExcellentKit
{
    [DisallowMultipleComponent]
    public class SignalReciever : MonoBehaviour
    {
        public event SignalHandler SignalRecieved;

        /// <summary>
        /// Push a signal to all attached SignalBehaviours.
        /// </summary>
        /// <param name="signal"></param>
        public void Push(Signal signal)
        {
            SignalRecieved?.Invoke(signal);
        }

        private void OnDrawGizmos()
        {
            GizmosExtra.ColorPaletteSignalReciever();
            GizmosExtra.DrawLabel(
                transform.position,
                string.Format("Signal reciever {0}", gameObject.name)
            );
        }
    }
}
