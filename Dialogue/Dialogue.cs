using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MemoryPack;
using UnityEngine;

namespace ExcellentKit
{
    [MemoryPackable]
    public partial struct DialogueState
    {
        public HashSet<SecretString> Flags { get; init; }

        public void SetFlag(string flag, DialogueFlagState newState)
        {
            switch (newState)
            {
                case DialogueFlagState.Set:
                    Flags.Add(SecretString.Create(flag));
                    break;
                case DialogueFlagState.Unset:
                    Flags.Remove(SecretString.Create(flag));
                    break;
            }
        }

        public DialogueFlagState CheckFlag(string flag)
        {
            return Flags.Contains(SecretString.Create(flag))
                ? DialogueFlagState.Set
                : DialogueFlagState.Unset;
        }
    }

    public enum DialogueFlagState
    {
        Set,
        Unset
    }

    [Serializable]
    public class DialogueStateGuard
    {
        [SerializeField]
        private string[] _requiredSetFlags;

        [SerializeField]
        private string[] _requiredUnsetFlags;

        public bool Test(DialogueState stateToCheck)
        {
            return Array.TrueForAll(
                    _requiredSetFlags,
                    setFlag => stateToCheck.CheckFlag(setFlag) == DialogueFlagState.Set
                )
                && Array.TrueForAll(
                    _requiredUnsetFlags,
                    unsetFlag => stateToCheck.CheckFlag(unsetFlag) == DialogueFlagState.Unset
                );
        }

        public override string ToString()
        {
            var str = new StringBuilder();
            if (_requiredSetFlags.Length > 0)
            {
                str.AppendLine(
                    string.Format("Requires {0} to be SET", string.Join(", ", _requiredSetFlags))
                );
            }
            if (_requiredUnsetFlags.Length > 0)
            {
                str.AppendLine(
                    string.Format(
                        "Requires {0} to be UNSET",
                        string.Join(", ", _requiredUnsetFlags)
                    )
                );
            }
            if (str.Length > 0)
            {
                str.Length--;
            }
            return str.ToString();
        }
    }

    [Serializable]
    public class NextDialogueEntry
    {
        [SerializeField]
        private DialogueEntry _main;

        [SerializeField]
        private List<AlternateDialogueEntry> _alternatives = new();

        public DialogueEntry Main => _main;
        public List<AlternateDialogueEntry> Alternatives => _alternatives;

        public DialogueEntry Test(DialogueState state)
        {
            var validAlt = _alternatives.FirstOrDefault(alt => alt.Guard.Test(state));
            return validAlt != null ? validAlt.Entry : _main;
        }
    }

    [Serializable]
    public class AlternateDialogueEntry
    {
        [SerializeField]
        private DialogueEntry _entry;

        [SerializeField]
        private DialogueStateGuard _guard;

        public DialogueEntry Entry => _entry;
        public DialogueStateGuard Guard => _guard;
    }
}
