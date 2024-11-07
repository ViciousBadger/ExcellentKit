using UnityEngine;

namespace ExcellentKit
{
    /// <summary>
    /// Emits recieved signals, but with an assigned message.
    /// </summary>
    public class SignalPipeWithMessage : SignalPipe
    {
        [SerializeField]
        private string _message;

        protected override string GetLabelTextForTarget(SignalReciever target)
        {
            return string.Format("With message: {0}", _message);
        }

        protected override void OnSignalRecieved(Signal signal)
        {
            if (signal is ActivationSignal activationSignal)
            {
                Emit(
                    activationSignal with
                    {
                        Args = activationSignal.Args with { Message = _message }
                    }
                );
            }
            else
            {
                Emit(signal);
            }
        }
    }
}
