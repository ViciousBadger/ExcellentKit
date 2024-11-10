using System.Collections.Generic;
using System.Text;
using JetBrains.Annotations;
using UnityEditor;
using UnityEngine;

namespace ExcellentKit
{
    public class DialogueEntry : MonoBehaviour
    {
        [SerializeField, TextArea]
        private string _text;

        [SerializeField]
        private List<DialogueChoice> _choices = new();

        public string Text => _text;
        public List<DialogueChoice> Choices => _choices;

        public void OnDrawGizmos()
        {
            GizmosExtra.ColorPaletteDialogue();

            var str = new StringBuilder();
            str.AppendLine("Entry");
            str.AppendLine(string.Format("\"{0}\"", _text));

            if (_choices.Count == 0)
            {
                str.AppendLine("No choices");
            }
            str.Length--;

            GizmosExtra.DrawLabel(transform.position, str.ToString());

            foreach (var choice in _choices)
            {
                if (choice)
                {
                    GizmosExtra.DrawArrow(transform.position, choice.transform.position);
                }
            }
        }
    }

    [CustomEditor(typeof(DialogueEntry))]
    class DialogueEntryEditor : Editor
    {
        SerializedProperty _choicesList;

        void OnEnable()
        {
            _choicesList = serializedObject.FindProperty("_choices");
        }

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            EditorGUILayout.Separator();

            if (GUILayout.Button("Create new choice"))
            {
                var targetComponent = (DialogueEntry)target;

                var newChoice = new GameObject("Choice");
                Undo.RegisterCreatedObjectUndo(newChoice, "New dialogue choice");

                var component = newChoice.AddComponent<DialogueChoice>();
                var idx = _choicesList.arraySize;
                newChoice.transform.SetParent(targetComponent.transform, false);
                newChoice.transform.localPosition = new Vector3(idx, 1f, 0);

                _choicesList.InsertArrayElementAtIndex(idx);
                _choicesList.GetArrayElementAtIndex(idx).objectReferenceValue = component;
                serializedObject.ApplyModifiedProperties();

                Selection.activeGameObject = newChoice;
            }
        }
    }
}
