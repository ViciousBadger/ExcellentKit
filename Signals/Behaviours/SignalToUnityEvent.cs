using UnityEngine;
using UnityEngine.Events;

namespace ExcellentKit
{
    public class SignalToUnityEvent : SignalBehaviour
    {
        [SerializeField]
        private UnityEvent<SignalArgs> _onActivate;

        [SerializeField]
        private UnityEvent _onDeactivate;

        protected override void OnSignalRecieved(Signal signal)
        {
            switch (signal)
            {
                case ActivationSignal(_, SignalArgs args):
                    _onActivate.Invoke(args);
                    break;
                case DeactivationSignal:
                    _onDeactivate.Invoke();
                    break;
            }
        }

        private void OnDrawGizmos()
        {
            GizmosExtra.ColorPaletteSignalBehaviour();
            DrawGizmosFor(_onActivate, "On activate");
            DrawGizmosFor(_onDeactivate, "On deactivate");
        }

        private void DrawGizmosFor(UnityEventBase unityEvent, string label)
        {
            for (int idx = 0; idx < unityEvent.GetPersistentEventCount(); idx++)
            {
                Vector3 targetPos = transform.position;
                string targetName = "unknown";

                var targetObj = unityEvent.GetPersistentTarget(idx);
                if (targetObj is Component component)
                {
                    targetPos = component.transform.position;
                    targetName = string.Format(
                        "{0}\n{1}",
                        component.gameObject.name,
                        component.GetType()
                    );
                }
                if (targetObj is GameObject gameObject)
                {
                    targetPos = gameObject.transform.position;
                    targetName = string.Format("{0} - GameObject", gameObject.name);
                }

                GizmosExtra.DrawArrow(
                    transform.position,
                    targetPos,
                    string.Format(
                        "{0}\n{1}.{2}",
                        label,
                        targetName,
                        unityEvent.GetPersistentMethodName(idx)
                    )
                );
            }
        }
    }
}
