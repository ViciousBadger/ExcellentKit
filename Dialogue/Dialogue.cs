using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;
using Alchemy.Inspector;
using MemoryPack;
using UnityEngine;

namespace ExcellentGame
{
    [MemoryPackable]
    public partial class DialogueState
    {
        public DialogueFlag FlagMask { get; private set; } = new();

        public void ChangeFlags(DialogueFlag flag, DialogueFlagState newState)
        {
            switch (newState)
            {
                case DialogueFlagState.Set:
                    FlagMask |= flag;
                    break;
                case DialogueFlagState.Unset:
                    FlagMask &= ~flag;
                    break;
            }
        }

        public DialogueFlagState CheckFlags(DialogueFlag flags)
        {
            return FlagMask.HasFlag(flags) ? DialogueFlagState.Set : DialogueFlagState.Unset;
        }
    }

    [Flags]
    public enum DialogueFlag
    {
        None = 0b_0000_0000,
        Alpha = 0b_0000_0001,
        Beta = 0b_0000_0010,
        Gamma = 0b_0000_0100,
        Delta = 0b_0000_1000,
        Epsilon = 0b_0001_0000,
        Zeta = 0b_0010_0000,
        Eta = 0b_0100_0000,
        Theta = 0b_1000_0000
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
        private DialogueFlag _requiredSetFlags;

        [SerializeField]
        private DialogueFlag _requiredUnsetFlags;

        public bool Test(DialogueState stateToCheck)
        {
            return (
                    _requiredSetFlags == DialogueFlag.None
                    || stateToCheck.CheckFlags(_requiredSetFlags) == DialogueFlagState.Set
                )
                && (
                    _requiredUnsetFlags == DialogueFlag.None
                    || stateToCheck.CheckFlags(_requiredUnsetFlags) == DialogueFlagState.Unset
                );
        }

        public override string ToString()
        {
            var str = new StringBuilder();
            if (_requiredSetFlags != DialogueFlag.None)
            {
                str.AppendLine(string.Format("Requires {0} to be SET", _requiredSetFlags));
            }
            if (_requiredUnsetFlags != DialogueFlag.None)
            {
                str.AppendLine(string.Format("Requires {0} to be UNSET", _requiredUnsetFlags));
            }
            if (str.Length > 0)
            {
                str.Length--;
            }
            return str.ToString();
        }
    }

    [Serializable]
    public class DialogueFlagRequirement
    {
        [SerializeField]
        private DialogueFlag _flag;

        [SerializeField]
        private DialogueFlagState _requiredState;

        public DialogueFlag Flag => _flag;
        public DialogueFlagState RequiredState => _requiredState;
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
            foreach (var alternative in _alternatives)
            {
                if (alternative.Guard.Test(state))
                {
                    return alternative.Entry;
                }
            }
            return _main;
        }
    }

    [Serializable]
    public class AlternateDialogueEntry
    {
        [SerializeField, Required]
        private DialogueEntry _entry;

        [SerializeField]
        private DialogueStateGuard _guard;

        public DialogueEntry Entry => _entry;
        public DialogueStateGuard Guard => _guard;
    }
}
