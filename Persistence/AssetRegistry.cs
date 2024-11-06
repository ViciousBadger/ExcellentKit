using System;
using System.Collections.Generic;
using System.Linq;
using Alchemy.Inspector;
using JetBrains.Annotations;
using UnityEngine;

namespace ExcellentKit
{
    /// <summary>
    /// <para>
    /// A registry to supply stable identifiers to a certain type of ScriptableObject.
    /// </para>
    /// <para>
    /// ScriptableObjects are very useful attaching reusable data to
    /// GameObjects, but Unity provides no built-in way to "look up" a certain
    /// ScriptableObject via code, the only way to access them is when a
    /// reference is saved in the inspector.  To work around this you can use an
    /// AssetRegistry, simply put it as a serialized field of some globally
    /// accessable GameObject. Whenever you need a reference to certain
    /// ScriptableObject, use FindIdentifier and save the string you get. You
    /// can then use FindAsset to find the same asset by the identifier string.
    /// </para>
    /// </summary>
    /// <typeparam name="T">The type of ScriptableObject to provide identifiers for</typeparam>
    [Serializable]
    public class AssetRegistry<T>
        where T : ScriptableObject
    {
        [SerializeField, ListViewSettings(ShowFoldoutHeader = false)]
        private List<AssetRegistryEntry> _entries;

        public T FindAsset(string identifier)
        {
            return _entries.First(e => e.Identifier == identifier).Asset;
        }

        public string FindIdentifier(T asset)
        {
            return _entries.First(e => e.Asset == asset).Identifier;
        }

        [Serializable]
        public class AssetRegistryEntry
        {
            [SerializeField, HideLabel, AssetsOnly, HorizontalGroup]
            [OnValueChanged("SetDefaultIdentifier")]
            private T _asset;

            // TODO: Hash this identifier!
            [SerializeField, HideLabel, HorizontalGroup]
            private string _identifier;

            public string Identifier => _identifier;
            public T Asset => _asset;

            [UsedImplicitly]
            private void SetDefaultIdentifier(T asset)
            {
                if (string.IsNullOrEmpty(_identifier))
                {
                    _identifier = asset.name;
                }
            }
        }
    }
}
