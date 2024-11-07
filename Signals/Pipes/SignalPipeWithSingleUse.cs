using System.Collections.Generic;

namespace ExcellentKit
{
    /// <summary>
    /// Only allows a single Activate signal to be emitted, followed by its Deactivate signal.
    /// </summary>
    public class SignalPipeWithSingleUse : SignalPipe
    {
        public bool Used { get; set; } = new();

        private uint? _activeSignalId = null;

        protected override void OnSignalRecieved(Signal signal)
        {
            switch (signal)
            {
                case ActivationSignal(uint id):
                    if (!Used)
                    {
                        _activeSignalId = id;
                        Used = true;
                        Emit(signal);
                    }
                    break;
                case DeactivationSignal(uint id):
                    if (id == _activeSignalId)
                    {
                        Emit(signal);
                    }
                    break;
            }
        }

        protected override string GetLabelTextForTarget(SignalReciever target)
        {
            return "Single use";
        }
    }
}
