using System.Collections.Generic;
using System.Linq;
using System.Text;
using Alchemy.Inspector;
using ExcellentKit;
using JetBrains.Annotations;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace ExcellentGame
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

        [Button, UsedImplicitly]
        private void CreateNewChoice()
        {
            var newChoice = new GameObject("Choice");
            Undo.RegisterCreatedObjectUndo(newChoice, "New dialogue choice");

            var component = newChoice.AddComponent<DialogueChoice>();
            newChoice.transform.SetParent(transform, false);
            newChoice.transform.localPosition = new Vector3(_choices.Count, 1f, 0);

            var serialized = new SerializedObject(this);
            var choices = serialized.FindProperty("_choices");
            var idx = choices.arraySize;
            choices.InsertArrayElementAtIndex(idx);
            choices.GetArrayElementAtIndex(idx).objectReferenceValue = component;
            serialized.ApplyModifiedProperties();

            Selection.activeGameObject = newChoice;
        }
    }
}
