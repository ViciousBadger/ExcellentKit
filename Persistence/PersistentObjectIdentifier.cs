using System;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace ExcellentKit
{
    public class PersistentObjectIdentifier : MonoBehaviour
    {
        [SerializeField]
        private string _idString = Guid.NewGuid().ToString();

        public string IdString => _idString;

        public Hash128 ComputeIdHash()
        {
            return Hash128.Compute(_idString);
        }

        private void OnDrawGizmos()
        {
            GizmosExtra.ColorPaletteGameplay();
            GizmosExtra.DrawLabel(transform.position, string.Format("Persistent\n{0}", _idString));
        }
    }

    [CustomEditor(typeof(PersistentObjectIdentifier))]
    [CanEditMultipleObjects]
    public class PersistentObjectIdentifierEditor : Editor
    {
        private SerializedProperty _idString;

        void OnEnable()
        {
            _idString = serializedObject.FindProperty("_idString");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            DrawDefaultInspector();
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Set to random GUID"))
            {
                foreach (var target in serializedObject.targetObjects)
                {
                    var serialized = new SerializedObject(target);
                    serialized.FindProperty("_idString").stringValue = Guid.NewGuid().ToString();
                    serialized.ApplyModifiedProperties();
                }
            }
            if (GUILayout.Button("Set to GameObject name"))
            {
                foreach (var target in serializedObject.targetObjects)
                {
                    var serialized = new SerializedObject(target);
                    serialized.FindProperty("_idString").stringValue = target.name;
                    serialized.ApplyModifiedProperties();
                }
            }
            GUILayout.EndHorizontal();

            var count = GetIdCount();
            var text = count > 1 ? "NOT unique" : "unique";

            GUI.color = count > 1 ? Color.yellow : Color.green;
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label(string.Format("ID is {0} ({1} in scene)", text, count));
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            serializedObject.ApplyModifiedProperties();
        }

        private int GetIdCount()
        {
            var idStringValue = _idString.stringValue;
            return FindObjectsByType<PersistentObjectIdentifier>(FindObjectsSortMode.None)
                .Count(obj => obj.IdString == idStringValue);
        }
    }
}
