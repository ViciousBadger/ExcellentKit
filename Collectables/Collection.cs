using System;
using System.Collections.Generic;
using System.Linq;
using MemoryPack;

namespace ExcellentKit
{
    /// <summary>
    /// A persist-able container for collectables.
    /// </summary>
    public class Collection
    {
        public Dictionary<Collectable, int> Contents { get; private set; } = new();

        public event Action<Collectable, int> ContentsChanged;

        public int GetAmount(Collectable thing)
        {
            return Contents.ContainsKey(thing) ? Contents[thing] : 0;
        }

        public void Change(Collectable collectable, int delta)
        {
            if (Contents.ContainsKey(collectable))
            {
                Contents[collectable] += delta;
                //Remove if no items left
                if (Contents[collectable] <= 0)
                {
                    Contents.Remove(collectable);
                }
            }
            else
            {
                if (delta > 0)
                {
                    Contents.Add(collectable, delta);
                }
            }
            ContentsChanged?.Invoke(
                collectable,
                Contents.ContainsKey(collectable) ? Contents[collectable] : 0
            );
        }

        /// <summary>
        /// Persist this collection, using an asset registry to look up asset identifiers.
        /// </summary>
        public PersistedCollection Persist(AssetRegistry<Collectable> registry)
        {
            return new()
            {
                ContentsByAssetId = Contents
                    .Select(t => new { Key = registry.FindIdentifier(t.Key), t.Value })
                    .Where(t => t.Key != null)
                    .Select(t => new { Key = SecretString.Create(t.Key), t.Value })
                    .ToDictionary(t => t.Key, t => t.Value)
            };
        }

        /// <summary>
        /// Apply persisted data to this collection, using an asset registry to look up asset references.
        /// </summary>
        public void Apply(
            AssetRegistry<Collectable> registry,
            PersistedCollection persistedCollection
        )
        {
            Contents = persistedCollection
                .ContentsByAssetId.Select(t => new
                {
                    Key = registry.FindAsset(t.Key.Reveal()),
                    t.Value
                })
                .Where(t => t.Key != null)
                .ToDictionary(t => t.Key, t => t.Value);
        }
    }

    [MemoryPackable]
    public readonly partial struct PersistedCollection
    {
        public Dictionary<SecretString, int> ContentsByAssetId { get; init; }
    }
}
