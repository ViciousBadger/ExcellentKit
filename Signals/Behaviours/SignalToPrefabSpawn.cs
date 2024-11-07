using System.Collections.Generic;
using UnityEngine;

namespace ExcellentKit
{
    public class SignalToPrefabSpawn : SignalBehaviour
    {
        [SerializeField]
        private GameObject _prefabToSpawn;

        [SerializeField]
        private Transform _spawnTransform;

        [SerializeField]
        private bool _setParent = false;

        [SerializeField]
        private bool _destroyOnSignalDeactivation = false;

        public Vector3 SpawnPosition
        {
            get { return _spawnTransform ? _spawnTransform.position : Vector3.zero; }
        }

        public Quaternion SpawnRotation
        {
            get { return _spawnTransform ? _spawnTransform.rotation : Quaternion.identity; }
        }

        private readonly List<GameObject> _spawnedObjects = new();

        protected override void OnSignalRecieved(Signal signal)
        {
            switch (signal)
            {
                case ActivationSignal:
                    if (_prefabToSpawn)
                    {
                        var instance = Instantiate(_prefabToSpawn, SpawnPosition, SpawnRotation);
                        _spawnedObjects.Add(instance);

                        if (_setParent)
                        {
                            instance.transform.SetParent(_spawnTransform, true);
                        }
                    }
                    break;
                case DeactivationSignal:
                    if (_destroyOnSignalDeactivation)
                    {
                        DestroyAllSpawnedObjects();
                    }
                    break;
            }
        }

        public void DestroyAllSpawnedObjects()
        {
            foreach (var obj in _spawnedObjects)
            {
                Destroy(obj);
            }
            _spawnedObjects.Clear();
        }

        public void OnDrawGizmos()
        {
            if (_prefabToSpawn != null)
            {
                GizmosExtra.ColorPaletteSignalBehaviour();
                GizmosExtra.DrawLabel(
                    transform.position,
                    string.Format("Spawn prefab\n{0}", _prefabToSpawn.name)
                );
            }
            else
            {
                GizmosExtra.ColorPaletteError();
                GizmosExtra.DrawLabel(transform.position, "Prefab not assigned");
            }
        }

        public void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(transform.position, SpawnPosition);

            Gizmos.color = Color.cyan;
            Gizmos.DrawSphere(SpawnPosition, 0.2f);
            Gizmos.DrawLine(SpawnPosition, SpawnPosition + SpawnRotation * Vector3.forward);
        }
    }
}
