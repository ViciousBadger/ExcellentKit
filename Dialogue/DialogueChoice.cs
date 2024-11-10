using System.Text;
using JetBrains.Annotations;
using UnityEditor;
using UnityEngine;

namespace ExcellentKit
{
    public class DialogueChoice : MonoBehaviour
    {
        [SerializeField]
        private DialogueStateGuard _guard;

        [SerializeField, TextArea]
        private string _text;

        [SerializeField]
        private NextDialogueEntry _nextEntry;

        public DialogueStateGuard Guard => _guard;
        public string Text => _text;
        public NextDialogueEntry NextEntry => _nextEntry;

        public void OnDrawGizmos()
        {
            GizmosExtra.ColorPaletteDialogue();

            var str = new StringBuilder();
            str.AppendLine("Choice");

            str.AppendLine(string.Format("\"{0}\"", _text));

            if (_guard != null && !string.IsNullOrEmpty(_guard.ToString()))
            {
                str.AppendLine(_guard.ToString());
            }

            if (_nextEntry.Main)
            {
                GizmosExtra.DrawArrow(
                    transform.position,
                    _nextEntry.Main.transform.position,
                    "Main entry"
                );
            }
            else
            {
                str.AppendLine("No main entry");
            }

            foreach (var alternative in _nextEntry.Alternatives)
            {
                if (alternative.Entry)
                {
                    GizmosExtra.DrawArrow(
                        transform.position,
                        alternative.Entry.transform.position,
                        alternative.Guard.ToString()
                    );
                }
            }

            str.Length--;
            GizmosExtra.DrawLabel(transform.position, str.ToString());
        }
    }

    [CustomEditor(typeof(DialogueChoice))]
    class DialogueChoiceEditor : Editor
    {
        SerializedProperty _nextMainEntry;

        void OnEnable()
        {
            _nextMainEntry = serializedObject.FindProperty("_nextEntry._main");
        }

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            EditorGUILayout.Separator();

            if (
                _nextMainEntry.objectReferenceValue == null
                && GUILayout.Button("Create next main entry")
            )
            {
                var targetComponent = (DialogueChoice)target;

                var newEntry = new GameObject("Entry");
                Undo.RegisterCreatedObjectUndo(newEntry, "New dialogue entry");

                var component = newEntry.AddComponent<DialogueEntry>();
                newEntry.transform.SetParent(targetComponent.transform, false);
                newEntry.transform.localPosition = new Vector3(0, 1f, 0);

                _nextMainEntry.objectReferenceValue = component;
                serializedObject.ApplyModifiedProperties();

                Selection.activeGameObject = newEntry;
            }
        }
    }
}
