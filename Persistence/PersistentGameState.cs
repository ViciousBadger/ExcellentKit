#nullable enable

using System.Collections.Generic;
using System.IO;
using MemoryPack;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ExcellentKit
{
    [MemoryPackable]
    public partial class PersistentGameState<TGlobalState>
        where TGlobalState : struct, IMemoryPackable<TGlobalState>
    {
        public TGlobalState GlobalState { get; set; }
        public Dictionary<Hash128, PersistentSceneState> SceneStates { get; set; } = new();

        public void PersistCurrentScene()
        {
            Hash128 sceneIdentifier = Hash128.Compute(SceneManager.GetActiveScene().name);
            if (SceneStates.ContainsKey(sceneIdentifier))
            {
                SceneStates.Remove(sceneIdentifier);
            }
            var state = new PersistentSceneState();

            // Quirk: We always unpause the audio listener when persisting, otherwise AudioSource "isPlaying" state will be saved wrongly when the game is paused.
            var wasPaused = AudioListener.pause;
            AudioListener.pause = false;

            foreach (
                var persistentBehaviour in Object.FindObjectsByType<PersistentBehaviour>(
                    FindObjectsInactive.Include,
                    FindObjectsSortMode.None
                )
            )
            {
                state.PersistBehaviour(persistentBehaviour);
            }
            SceneStates.Add(sceneIdentifier, state);

            AudioListener.pause = wasPaused;
        }

        public void ApplyCurrentScene()
        {
            Hash128 sceneIdentifier = Hash128.Compute(SceneManager.GetActiveScene().name);
            if (SceneStates.TryGetValue(sceneIdentifier, out PersistentSceneState sceneState))
            {
                foreach (
                    var persistentBehaviour in Object.FindObjectsByType<PersistentBehaviour>(
                        FindObjectsInactive.Include,
                        FindObjectsSortMode.None
                    )
                )
                {
                    sceneState.ApplyBehaviour(persistentBehaviour);
                }
            }
        }

        public void SaveToFile(string filePath)
        {
            File.WriteAllBytes(filePath, MemoryPackSerializer.Serialize(this));
        }

        public static PersistentGameState<TGlobalState> LoadFromFile(string filePath)
        {
            var bytes = File.ReadAllBytes(filePath);
            var state = MemoryPackSerializer.Deserialize<PersistentGameState<TGlobalState>>(bytes);
            return state ?? throw new InvalidDataException("State was null!!");
        }
    }

    [MemoryPackable]
    public partial class PersistentSceneState
    {
        public Dictionary<Hash128, PersistentObjectState> ObjectStates { get; set; } = new();

        /// <summary>
        /// Store the current state of a PersistentBehaviour.
        /// </summary>
        public void PersistBehaviour(PersistentBehaviour behaviour)
        {
            if (behaviour.ObjectIdentifier == null)
            {
                Debug.Log(
                    "Skipped deserialize, ObjectIdentifier is null in: " + behaviour.gameObject.name
                );
            }
            else
            {
                var objectState = GetOrCreateObjectState(behaviour.ObjectIdentifier);
                var behaviourId = behaviour.ComputeIdentifier();

                if (objectState.PersistentBehaviours.ContainsKey(behaviourId))
                {
                    Debug.LogError(
                        string.Format(
                            "Duplicate component save attempted in persistent object {0}.\nDon't add the same PersistentComponent more than once and make sure the object has an unique identifier.",
                            behaviour.ObjectIdentifier.IdString
                        )
                    );
                }
                else
                {
                    objectState.PersistentBehaviours.Add(
                        behaviourId,
                        behaviour.PersistAndSerialize()
                    );
                }
            }
        }

        /// <summary>
        /// Apply the persisted state of a PersistentBehaviour (if there is any).
        /// </summary>
        public void ApplyBehaviour(PersistentBehaviour behaviour)
        {
            if (behaviour.ObjectIdentifier == null)
            {
                Debug.Log(
                    "Skipped deserialize, ObjectIdentifier is null in: " + behaviour.gameObject.name
                );
            }
            else
            {
                var objectState = GetOrCreateObjectState(behaviour.ObjectIdentifier);
                if (
                    objectState.PersistentBehaviours.TryGetValue(
                        behaviour.ComputeIdentifier(),
                        out byte[] value
                    )
                )
                {
                    behaviour.DeserializeAndApply(value);
                }
            }
        }

        public PersistentObjectState GetOrCreateObjectState(PersistentObjectIdentifier identifier)
        {
            var idHash = identifier.ComputeIdHash();
            if (!ObjectStates.ContainsKey(idHash))
            {
                ObjectStates.Add(idHash, new PersistentObjectState());
            }
            return ObjectStates[idHash];
        }
    }

    [MemoryPackable]
    public partial class PersistentObjectState
    {
        public Dictionary<Hash128, byte[]> PersistentBehaviours { get; private set; } = new();
    }
}
