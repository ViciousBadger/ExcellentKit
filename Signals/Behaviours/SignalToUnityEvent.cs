using UnityEngine;

namespace ExcellentKit
{
    public class SignalToUnityEvent : SignalBehaviour
    {
        [SerializeField]
        private SignalUnityEvent _onActivate;

        [SerializeField]
        private SignalUnityEvent _onDeactivate;

        protected override void OnSignalRecieved(Signal signal)
        {
            switch (signal.Type)
            {
                case SignalType.Activate:
                    _onActivate.Invoke(signal);
                    break;
                case SignalType.Deactivate:
                    _onDeactivate.Invoke(signal);
                    break;
            }
        }

        private void OnDrawGizmos()
        {
            GizmosExtra.ColorPaletteSignalBehaviour();
            DrawGizmosFor(_onActivate, "On activate");
            DrawGizmosFor(_onDeactivate, "On deactivate");
        }

        private void DrawGizmosFor(SignalUnityEvent unityEvent, string label)
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
