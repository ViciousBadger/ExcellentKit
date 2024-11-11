using System;
using ExcellentKit;
using MemoryPack;
using UnityEngine;

namespace ExcellentKit
{
    public enum CheckpointTurnOnMode
    {
        Activated,
        Loaded
    }

    public class Checkpoint : PersistentBehaviour<CheckpointData>
    {
        [SerializeField]
        private string _checkpointName;

        [SerializeField]
        private Transform _spawnPosition;

        public string CheckpointName => _checkpointName;
        public Vector3 SpawnPosition =>
            _spawnPosition ? _spawnPosition.position : transform.position;
        public Quaternion SpawnRotation => _spawnPosition.rotation;

        private bool _isTurnedOn;
        private bool _isDiscovered;

        public event Action<CheckpointTurnOnMode> TurnedOn;
        public event Action TurnedOff;

        public void TurnOn(SignalArgs args)
        {
            if (!_isTurnedOn)
            {
                if (args.SubjectIsPlayer(out IPlayer player))
                {
                    player.Mortality.CurrentCheckpoint = this;
                }
                _isTurnedOn = true;
                _isDiscovered = true;
                TurnedOn?.Invoke(CheckpointTurnOnMode.Activated);
            }
        }

        public void TurnOff()
        {
            if (_isTurnedOn)
            {
                _isTurnedOn = false;
                TurnedOff?.Invoke();
            }
        }

        private void OnDrawGizmos()
        {
            GizmosExtra.ColorPaletteGameplay();
            GizmosExtra.DrawLabel(
                transform.position,
                string.Format("Checkpoint\n'{0}'", _checkpointName)
            );
        }

        protected override CheckpointData Persist()
        {
            return new() { IsTurnedOn = _isTurnedOn, IsDiscovered = _isDiscovered };
        }

        protected override void Apply(CheckpointData savedData)
        {
            _isTurnedOn = savedData.IsTurnedOn;
            _isDiscovered = savedData.IsDiscovered;

            if (_isTurnedOn)
            {
                TurnedOn?.Invoke(CheckpointTurnOnMode.Loaded);

                // TODO: Find player and set current checkpoint..
                // .. or save current the checkpoint 'id' on the player! (could use its persistent identifier)
                // if (GameMaster.TryGetPlayer(out PlayerActor player))
                // {
                //     player.Mortality.CurrentCheckpoint = this;
                // }
            }
        }
    }

    [MemoryPackable]
    public readonly partial struct CheckpointData
    {
        public bool IsTurnedOn { get; init; }
        public bool IsDiscovered { get; init; }
    }
}
