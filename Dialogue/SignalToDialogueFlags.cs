using ExcellentGame;
using UnityEngine;

namespace ExcellentKit
{
    public class SignalToDialogueFlags : SignalBehaviour
    {
        [SerializeField]
        private DialogueCharacter _targetCharacter;

        [SerializeField]
        private DialogueFlag _flags;

        [SerializeField]
        private DialogueFlagState _newState;

        protected override void OnSignalRecieved(Signal signal)
        {
            if (signal.Type == SignalType.Activate)
            {
                var target = _targetCharacter
                    ? _targetCharacter
                    : signal.Subject.GetComponent<DialogueCharacter>();
                if (target)
                {
                    target.State.ChangeFlags(_flags, _newState);
                }
            }
            // NOTE: we do nothing on deactivate, but in theory it could also have an effect..
        }

        private void OnDrawGizmos()
        {
            GizmosExtra.ColorPaletteSignalBehaviour();
            GizmosExtra.DrawLabel(
                transform.position,
                string.Format(
                    "Set dialogue flag\n{0}: {1}\nTarget: {2}",
                    _flags,
                    _newState,
                    _targetCharacter ? _targetCharacter.gameObject.name : "Signal actor"
                )
            );
        }
    }
}
