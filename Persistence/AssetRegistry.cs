#nullable enable

using System;
using System.Collections.Generic;
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
    /// reference is saved in the inspector.
    /// </para>
    /// <para>
    /// To work around this, you can use an
    /// AssetRegistry - simply put it as a serialized field of some globally
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
        private List<AssetRegistryEntry> _entries = new();

        public T? FindAsset(string identifier)
        {
            var entry = _entries.Find(e => e.Identifier == identifier);
            if (entry != null)
            {
                return entry.Asset;
            }
            else
            {
                Debug.LogWarning("Asset identifier is not in registry: " + identifier);
                return null;
            }
        }

        public string? FindIdentifier(T asset)
        {
            var entry = _entries.Find(e => e.Asset == asset);
            if (entry != null)
            {
                return entry.Identifier;
            }
            else
            {
                Debug.LogWarning("Asset is not in registry: " + asset.name);
                return null;
            }
        }

        [Serializable]
        public class AssetRegistryEntry
        {
            [SerializeField, HideLabel, AssetsOnly, HorizontalGroup]
            [OnValueChanged("SetDefaultIdentifier")]
            private T? _asset;

            [SerializeField, HideLabel, HorizontalGroup]
            private string _identifier = string.Empty;

            public string Identifier => _identifier;
            public T? Asset => _asset;

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
