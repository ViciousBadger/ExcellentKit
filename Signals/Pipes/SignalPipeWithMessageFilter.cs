using UnityEngine;

namespace ExcellentKit
{
    /// <summary>
    /// Emits recieved signals if their message matches a given string.
    /// </summary>
    public class SignalPipeWithMessageFilter : SignalPipe
    {
        [SerializeField]
        private string _requiredMessage;

        protected override void OnSignalRecieved(Signal signal)
        {
            if (signal.Message == _requiredMessage)
            {
                Emit(signal);
            }
        }

        protected override string GetLabelTextForTarget(SignalReciever target)
        {
            return string.Format("Message filter: {0}", _requiredMessage);
        }
    }
}
