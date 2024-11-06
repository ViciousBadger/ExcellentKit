using MemoryPack;
using UnityEngine;

namespace ExcellentKit
{
    public class PersistentTransform : PersistentBehaviour<PersistentTransformData>
    {
        protected override void Apply(PersistentTransformData savedData)
        {
            transform.SetLocalPositionAndRotation(savedData.Position, savedData.Rotation);
            transform.localScale = savedData.Scale;
        }

        protected override PersistentTransformData Persist()
        {
            return new()
            {
                Position = transform.localPosition,
                Rotation = transform.localRotation,
                Scale = transform.localScale,
            };
        }
    }

    [MemoryPackable]
    public readonly partial struct PersistentTransformData
    {
        // This could be saved more compactly with a Matrix4x4..
        public Vector3 Position { get; init; }
        public Quaternion Rotation { get; init; }
        public Vector3 Scale { get; init; }
    }
}
