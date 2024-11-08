using ExcellentGame;
using UnityEngine;

namespace ExcellentKit
{
    public class SignalToDialogueFlag : SignalBehaviour
    {
        [SerializeField]
        private DialogueCharacter _targetCharacter;

        [SerializeField]
        private string _flag;

        [SerializeField]
        private DialogueFlagState _action;

        protected override void OnSignalRecieved(Signal signal)
        {
            if (signal is ActivationSignal activationSignal)
            {
                var target = _targetCharacter
                    ? _targetCharacter
                    : activationSignal.Args.Subject.GetComponent<DialogueCharacter>();
                if (target)
                {
                    target.State.SetFlag(_flag, _action);
                }
            }
        }

        private void OnDrawGizmos()
        {
            GizmosExtra.ColorPaletteSignalBehaviour();
            GizmosExtra.DrawLabel(
                transform.position,
                string.Format(
                    "{0} dialogue flag {1}\nTarget: {2}",
                    _action == DialogueFlagState.Set ? "Set" : "Unset",
                    _flag,
                    _targetCharacter ? _targetCharacter.gameObject.name : "Signal subject"
                )
            );
        }
    }
}
