using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using ExcellentKit;
using JetBrains.Annotations;
using MemoryPack;
using UnityEditor;
using UnityEditor.Build;
using UnityEngine;

namespace ExcellentGame
{
    public class DialogueCharacter : PersistentBehaviour<DialogueCharacterData>, IPossessable
    {
        private enum DialogueSignalType
        {
            Entry,
            Choice
        }

        private sealed class ActiveDialogueSignal
        {
            public uint Id { get; init; }
            public SignalReciever Reciever { get; init; }
        }

        [SerializeField]
        private DialogueView _dialogueViewPrefab;

        [SerializeField]
        private NextDialogueEntry _initialEntry;

        [SerializeField]
        private DialogueCharacterProfile _characterProfile;

        [Header("Camera")]
        [SerializeField]
        private Transform _cameraAnchor;

        [SerializeField]
        private float _cameraFovModifier = 1.0f;

        [SerializeField]
        private float _cameraTargetChangeTime = 1.0f;

        private ExcellentControls.DialogueActions _actions;
        private DialogueProcess _activeProcess;
        private readonly Dictionary<DialogueSignalType, ActiveDialogueSignal> _activeSignals =
            new();

        public DialogueState State { get; private set; } = new();

        protected override void Awake()
        {
            base.Awake();
            _actions = new ExcellentControls().Dialogue;
            _actions.Exit.performed += (_) => Conciousness.Instance.EndPossession(this);
        }

        public void OnPossessionStart()
        {
            _actions.Enable();
        }

        public void OnPossessionEnd()
        {
            _actions.Disable();

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

        public void InitiateDialogue()
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
                var view = Instantiate(_dialogueViewPrefab);
                view.Init(_characterProfile, _activeProcess);
            }
        }

        private void OnActiveEntryChanged(DialogueEntry newEntry)
        {
            UpdateDialogueSignal(DialogueSignalType.Entry, newEntry);
        }

        private void OnChoiceSelected(DialogueChoice choice)
        {
            UpdateDialogueSignal(DialogueSignalType.Choice, choice);
        }

        private void UpdateDialogueSignal(DialogueSignalType type, Component target)
        {
            DeactivateExistingSignal(type);
            if (target.TryGetComponent(out SignalReciever reciever))
            {
                var newId = SignalId.Next();
                reciever.Push(
                    new()
                    {
                        Id = newId,
                        Type = SignalType.Activate,
                        Subject = gameObject
                    }
                );
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
                activeDialogueSignal.Reciever.Push(
                    new()
                    {
                        Id = activeDialogueSignal.Id,
                        Type = SignalType.Deactivate,
                        Subject = gameObject
                    }
                );
                _activeSignals.Remove(type);
            }
        }

        public void OnDrawGizmos()
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

        protected override DialogueCharacterData Persist()
        {
            return new() { State = State };
        }

        protected override void Apply(DialogueCharacterData savedData)
        {
            State = savedData.State;
        }
    }

    [MemoryPackable]
    public readonly partial struct DialogueCharacterData
    {
        public DialogueState State { get; init; }
    }
}
