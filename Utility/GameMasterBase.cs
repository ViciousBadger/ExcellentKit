using System.IO;
using JetBrains.Annotations;
using MemoryPack;
using UnityEngine;

namespace ExcellentKit
{
    /// <summary>
    /// <para>
    /// Your trusty "God object" also known as a "Manager" or "Singleton".  This
    /// is the object you access when you want to interact with core game
    /// systems (Such as accessing the current player object or saving/loading)
    /// and don't have an easier way to do so.
    /// </para>
    /// <para>
    /// This class provides your God object with methods to access functionality
    /// from the Excellent Kit. By extending it you can provide custom behaviour.
    /// </para>
    /// <para>
    /// The class is a MonoBehaviour so that you can expose variables in the
    /// Inspector and refer to ScriptableObjects and other assets (e.g. in an
    /// AssetRegistry). This means it must be added to at least one scene,
    /// usually the first loaded scene, optionally using a shared prefab.
    /// </para>
    /// <para>
    /// The Game Master will automatically make sure no other Game Master instances
    /// exist when being instantiated. Use the static "Instance" property to access
    /// the active Game Master instance.
    /// </para>
    /// </summary>
    /// <typeparam name="TInstance">The concrete type of the game master. Just set this to the class you're defining.</typeparam>
    /// <typeparam name="TGlobalState">A type used for persistent global state.</typeparam>
    public abstract class GameMasterBase<TInstance, TGlobalState> : MonoBehaviour
        where TInstance : GameMasterBase<TInstance, TGlobalState>
        where TGlobalState : struct, IMemoryPackable<TGlobalState>
    {
        public static TInstance Instance { get; private set; }

        public static TGlobalState GlobalState
        {
            get { return Instance._persistentGameState.GlobalState; }
            set { Instance._persistentGameState.GlobalState = value; }
        }

        [SerializeField]
        protected string _saveFileName = "savegame";

        [SerializeField, Tooltip("Enables some helpful debug messages.")]
        protected bool _enableDebugLogging = true;

        protected PersistentGameState<TGlobalState> _persistentGameState = new();

        protected virtual string GetSaveGamePath()
        {
            return string.Format("{0}/{1}.save", Application.persistentDataPath, _saveFileName);
        }

        [UsedImplicitly]
        protected virtual void Awake()
        {
            if (FindObjectsByType<TInstance>(FindObjectsSortMode.None).Length > 1)
            {
                if (_enableDebugLogging)
                {
                    Debug.Log(
                        "Game Master discovered that the world already has a Game Master and will now self destruct."
                    );
                }
                Destroy(gameObject);
            }
            else
            {
                // There can be only one!
                Instance = (TInstance)this;
            }
        }

        /// <summary>
        /// Persist the current scene and save all persistent state to the save file.
        /// </summary>
        public void Save()
        {
            var path = GetSaveGamePath();
            if (_enableDebugLogging)
            {
                Debug.Log("Saving to " + path);
            }
            _persistentGameState.PersistCurrentScene();
            _persistentGameState.SaveToFile(path);
        }

        /// <summary>
        /// Persist the current scene without saving anything to disk.
        /// </summary>
        public void PersistCurrentScene()
        {
            if (_enableDebugLogging)
            {
                Debug.Log("Persisting current scene");
            }
            _persistentGameState.PersistCurrentScene();
        }

        /// <summary>
        /// Load persistent state from the save file (if any) and apply it to the current scene.
        /// </summary>
        public void Load()
        {
            var path = GetSaveGamePath();
            if (File.Exists(path))
            {
                if (_enableDebugLogging)
                {
                    Debug.Log("Loading save from " + path);
                }

                _persistentGameState = PersistentGameState<TGlobalState>.LoadFromFile(path);
                _persistentGameState.ApplyCurrentScene();
            }
            else if (_enableDebugLogging)
            {
                Debug.Log("No savegame found at " + path);
            }
        }

        /// <summary>
        /// Apply the current scene from existing persistent state without loading anything from disk.
        /// </summary>
        public void ApplyCurrentScene()
        {
            if (_enableDebugLogging)
            {
                Debug.Log("Applying current scene");
            }
            _persistentGameState.ApplyCurrentScene();
        }

        public void DeleteSaveData()
        {
            var path = GetSaveGamePath();
            if (File.Exists(path))
            {
                File.Delete(path);
                if (_enableDebugLogging)
                {
                    Debug.Log(string.Format("Deleting save data ({0})", path));
                }
            }
            else if (_enableDebugLogging)
            {
                Debug.Log(string.Format("Found no save data to delete ({0})", path));
            }
        }

        /// <summary>
        /// Returns the current player component (if a player exists in the scene).
        /// </summary>
        public static bool TryGetPlayer(out Player player)
        {
            player = FindAnyObjectByType<Player>();
            return player != null;
        }
    }
}
