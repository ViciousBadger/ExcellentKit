using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace ExcellentKit
{
    /// <summary>
    /// Represents a character you can dialogue with. Only handles the barebones logic of the dialogue system - inherit this class to customize and extend behaviour.
    /// </summary>
    public abstract class DialogueCharacterBase : PersistentBehaviour<DialogueState>
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

        public DialogueState State { get; private set; } = new() { Flags = new() };

        protected DialogueProcess _activeProcess;
        protected readonly Dictionary<DialogueSignalType, ActiveDialogueSignal> _activeSignals =
            new();

        public void InitiateDialogue()
        {
            if (_activeProcess == null)
            {
                _activeProcess = new DialogueProcess(State, _initialEntry.Test(State));
                _activeProcess.ActiveEntryChanged += OnActiveEntryChanged;
                _activeProcess.ChoiceSelected += OnChoiceSelected;
                _activeProcess.Exited += OnActiveDialogueProcessExited;
                OnActiveDialogueProcessStarted();
            }
        }

        public void ExitDialogue()
        {
            _activeProcess?.Exit();
        }

        protected virtual void OnActiveEntryChanged(DialogueEntry newEntry)
        {
            UpdateDialogueSignal(DialogueSignalType.Entry, newEntry);
        }

        protected virtual void OnChoiceSelected(DialogueChoice choice)
        {
            UpdateDialogueSignal(DialogueSignalType.Choice, choice);
        }

        protected virtual void OnActiveDialogueProcessStarted() { }

        protected virtual void OnActiveDialogueProcessExited()
        {
            DeactivateExistingSignal(DialogueSignalType.Entry);
            DeactivateExistingSignal(DialogueSignalType.Choice);
            _activeProcess = null;
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
            GizmosExtra.DrawLabel(transform.position, "Dialogue character");

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
    }

    [CustomEditor(typeof(DialogueCharacterBase))]
    class DialogueCharacterBaseEditor : Editor
    {
        SerializedProperty _initialMainEntry;

        void OnEnable()
        {
            _initialMainEntry = serializedObject.FindProperty("_initialEntry._main");
        }

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            EditorGUILayout.Separator();

            if (
                _initialMainEntry.objectReferenceValue == null
                && GUILayout.Button("Create initial main entry")
            )
            {
                var targetComponent = (DialogueCharacterBase)target;

                var newEntry = new GameObject("Entry");
                Undo.RegisterCreatedObjectUndo(newEntry, "New dialogue entry");

                var component = newEntry.AddComponent<DialogueEntry>();
                newEntry.transform.SetParent(targetComponent.transform, false);
                newEntry.transform.localPosition = new Vector3(0, 1f, 0);

                _initialMainEntry.objectReferenceValue = component;
                serializedObject.ApplyModifiedProperties();

                Selection.activeGameObject = newEntry;
            }
        }
    }
}
