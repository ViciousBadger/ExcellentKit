using System.Collections.Generic;
using MemoryPack;
using UnityEditor;
using UnityEngine;

namespace ExcellentKit
{
    public abstract class DialogueCharacterBase : PersistentBehaviour<DialogueState>, IPossessable
    {
        protected enum DialogueSignalType
        {
            Entry,
            Choice
        }

        protected sealed class ActiveDialogueSignal
        {
            public uint Id { get; init; }
            public SignalReciever Reciever { get; init; }
        }

        [SerializeField]
        protected NextDialogueEntry _initialEntry;

        [SerializeField]
        protected DialogueCharacterProfile _characterProfile;

        [Header("Camera")]
        [SerializeField]
        private Transform _cameraAnchor;

        [SerializeField]
        private float _cameraFovModifier = 1.0f;

        [SerializeField]
        private float _cameraTargetChangeTime = 1.0f;

        public DialogueState State { get; private set; } = new() { Flags = new() };

        protected DialogueProcess _activeProcess;
        protected readonly Dictionary<DialogueSignalType, ActiveDialogueSignal> _activeSignals =
            new();

        public virtual void InitiateDialogue()
        {
            if (_activeProcess == null)
            {
                Conciousness.Instance.Possess(this, _cameraTargetChangeTime);
                _activeProcess = new DialogueProcess(State, _initialEntry.Test(State));
                _activeProcess.ActiveEntryChanged += OnActiveEntryChanged;
                _activeProcess.ChoiceSelected += OnChoiceSelected;
                _activeProcess.Exited += () =>
                {
                    // ActiveProcess must be set to null immediately to avoid recursion!
                    // (Because OnPossesionEnd will call Exit() in the process otherwise..)
                    _activeProcess = null;
                    DeactivateExistingSignal(DialogueSignalType.Entry);
                    DeactivateExistingSignal(DialogueSignalType.Choice);
                    Conciousness.Instance.EndPossession(this);
                };
            }
        }

        protected virtual void OnActiveEntryChanged(DialogueEntry newEntry)
        {
            UpdateDialogueSignal(DialogueSignalType.Entry, newEntry);
        }

        protected virtual void OnChoiceSelected(DialogueChoice choice)
        {
            UpdateDialogueSignal(DialogueSignalType.Choice, choice);
        }

        private void UpdateDialogueSignal(DialogueSignalType type, Component target)
        {
            DeactivateExistingSignal(type);
            if (target.TryGetComponent(out SignalReciever reciever))
            {
                var newId = SignalId.Next();
                reciever.Push(new ActivationSignal(newId, new() { Subject = gameObject }));
                _activeSignals.Add(
                    type,
                    new ActiveDialogueSignal() { Id = newId, Reciever = reciever }
                );
            }
        }

        private void DeactivateExistingSignal(DialogueSignalType type)
        {
            if (_activeSignals.TryGetValue(type, out ActiveDialogueSignal activeDialogueSignal))
            {
                activeDialogueSignal.Reciever.Push(new DeactivationSignal(activeDialogueSignal.Id));
                _activeSignals.Remove(type);
            }
        }

        public virtual void OnPossessionStart()
        {
            // Override this to to things like enabling dialogue-related input
        }

        public virtual void OnPossessionEnd()
        {
            if (_activeProcess != null)
            {
                _activeProcess.Exit();
                _activeProcess = null;
            }
        }

        public float GetCameraFOVModifier()
        {
            return _cameraFovModifier;
        }

        public Vector3 GetCameraPosition()
        {
            return _cameraAnchor.position;
        }

        public Quaternion GetCameraRotation()
        {
            return _cameraAnchor.rotation;
        }

        protected override DialogueState Persist()
        {
            return State;
        }

        protected override void Apply(DialogueState savedData)
        {
            State = savedData;
        }

        [SerializeField]
        private void OnDrawGizmos()
        {
            GizmosExtra.ColorPaletteDialogue();
            var name = _characterProfile
                ? _characterProfile.CharacterName
                : "No character profile set!";
            GizmosExtra.DrawLabel(transform.position, string.Format("Character\n{0}", name));

            if (_initialEntry.Main)
            {
                GizmosExtra.DrawArrow(
                    transform.position,
                    _initialEntry.Main.transform.position,
                    "Main entry"
                );
            }

            foreach (var alternative in _initialEntry.Alternatives)
            {
                if (alternative.Entry)
                {
                    GizmosExtra.DrawArrow(
                        transform.position,
                        alternative.Entry.transform.position,
                        alternative.Guard.ToString()
                    );
                }
            }
        }

        // TODO: make editor button
        private void CreateInitialEntry()
        {
            var newEntry = new GameObject("Entry");
            Undo.RegisterCreatedObjectUndo(newEntry, "New dialogue entry");

            var component = newEntry.AddComponent<DialogueEntry>();
            newEntry.transform.SetParent(transform, false);
            newEntry.transform.localPosition = new Vector3(0, 1f, 0);

            var serialized = new SerializedObject(this);
            var initial = serialized.FindProperty("_nextEntry._main");
            initial.objectReferenceValue = component;
            serialized.ApplyModifiedProperties();

            Selection.activeGameObject = newEntry;
        }
    }
}
