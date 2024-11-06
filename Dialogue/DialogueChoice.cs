using System;
using System.Collections.Generic;
using System.Text;
using Alchemy.Inspector;
using ExcellentKit;
using JetBrains.Annotations;
using UnityEditor;
using UnityEngine;

namespace ExcellentGame
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

        [Button, UsedImplicitly]
        private void CreateNextEntry()
        {
            var newEntry = new GameObject("Entry");
            Undo.RegisterCreatedObjectUndo(newEntry, "New dialogue entry");

            var component = newEntry.AddComponent<DialogueEntry>();
            newEntry.transform.SetParent(transform, false);
            newEntry.transform.localPosition = new Vector3(0, 1f, 0);

            var serialized = new SerializedObject(this);
            var next = serialized.FindProperty("_nextEntry._main");
            next.objectReferenceValue = component;
            serialized.ApplyModifiedProperties();

            Selection.activeGameObject = newEntry;
        }
    }
}
