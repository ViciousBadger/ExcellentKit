using System;
using System.Linq;
using Alchemy.Inspector;
using JetBrains.Annotations;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace ExcellentGame
{
    public class PersistentObjectIdentifier : MonoBehaviour
    {
        [SerializeField]
        [ValidateInput(
            "IdIsUnique",
            "This ID is used by multiple objects in this scene. Things will go horribly wrong."
        )]
        private string _idString = Guid.NewGuid().ToString();

        public string IdString => _idString;

        public Hash128 ComputeIdHash()
        {
            return Hash128.Compute(_idString);
        }

        [Button, UsedImplicitly]
        private void SetToRandomGuid()
        {
            var serialized = new SerializedObject(this);
            var idString = serialized.FindProperty("_idString");
            idString.stringValue = Guid.NewGuid().ToString();
            serialized.ApplyModifiedProperties();
        }

        [Button, UsedImplicitly]
        private void SetToGameObjectName()
        {
            var serialized = new SerializedObject(this);
            var idString = serialized.FindProperty("_idString");
            idString.stringValue = gameObject.name;
            serialized.ApplyModifiedProperties();
        }

        [UsedImplicitly]
        private bool IdIsUnique(string id)
        {
            return FindObjectsByType<PersistentObjectIdentifier>(FindObjectsSortMode.None)
                    .Count(obj => obj.IdString == id) <= 1;
        }

        private void OnDrawGizmos()
        {
            GizmosExtra.ColorPaletteGameplay();
            GizmosExtra.DrawLabel(transform.position, string.Format("Persistent\n{0}", _idString));
            //GizmosExtra.DrawIcon(transform.position + Vector3.down * 0.1f, "Saving.png");
        }
    }
}
