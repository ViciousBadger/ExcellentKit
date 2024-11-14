using JetBrains.Annotations;
using UnityEditor;
using UnityEngine;

namespace ExcellentKit
{
    public enum GameObjectActivationAction
    {
        SetActive,
        SetInactive,
        Ignore
    }

    public class SignalToActive : SignalBehaviour
    {
        [SerializeField]
        private GameObject _target;

        [SerializeField]
        private GameObjectActivationAction _onActivate = GameObjectActivationAction.Ignore;

        [SerializeField]
        private GameObjectActivationAction _onDeactivate = GameObjectActivationAction.Ignore;

        protected override void OnSignalRecieved(Signal signal)
        {
            switch (signal)
            {
                case ActivationSignal:
                    Perform(_onActivate);
                    break;
                case DeactivationSignal:
                    Perform(_onDeactivate);
                    break;
            }
        }

        private void Perform(GameObjectActivationAction mode)
        {
            switch (mode)
            {
                case GameObjectActivationAction.SetActive:
                    _target.SetActive(true);
                    break;
                case GameObjectActivationAction.SetInactive:
                    _target.SetActive(false);
                    break;
            }
        }

        public void OnDrawGizmos()
        {
            if (_target)
            {
                GizmosExtra.ColorPaletteSignalBehaviour();
                GizmosExtra.DrawArrow(
                    transform.position,
                    _target.transform.position,
                    string.Format(
                        "Activation of {0}\nOn activate: {1}\nOn deactivate: {2}",
                        _target.name,
                        _onActivate,
                        _onDeactivate
                    )
                );
            }
            else
            {
                GizmosExtra.ColorPaletteError();
                GizmosExtra.DrawLabel(transform.position, "Activation target not set!");
            }
        }
    }

    [CustomEditor(typeof(SignalToActive))]
    [CanEditMultipleObjects]
    public class SignalToActiveEditor : Editor
    {
        private SerializedProperty _target;

        void OnEnable()
        {
            _target = serializedObject.FindProperty("_target");
        }

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            var target = (GameObject)_target.objectReferenceValue;
            var targetIsSignalLink = target && target.GetComponent<SignalReciever>() != null;
            if (targetIsSignalLink)
            {
                EditorGUILayout.HelpBox(
                    "Beware! The target object has a SignalReciever component. If you make the object inactive, it will ignore any recieved signals until it is made active again.",
                    MessageType.Warning
                );
            }
        }
    }
}
