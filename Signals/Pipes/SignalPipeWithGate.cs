using UnityEngine;

namespace ExcellentKit
{
    /// <summary>
    /// <para>
    /// Only allows one signal though at a time.<br/>
    /// Basically, as long as the first recieved signal is still active, the "gate" is "closed".
    /// </para>
    /// <para>
    /// Emitted signals will recieve new IDs.
    /// </para>
    /// </summary>
    public class SignalPipeWithGate : SignalPipe
    {
        private uint? _emittedId;
        private uint _activationCount;

        protected override string GetLabelTextForTarget(SignalReciever target)
        {
            return "Gated";
        }

        protected override void OnSignalRecieved(Signal signal)
        {
            switch (signal)
            {
                case ActivationSignal(_, SignalArgs args):
                    _activationCount += 1;
                    if (_activationCount == 1)
                    {
                        var newId = SignalId.Next();
                        _emittedId = newId;
                        Emit(new ActivationSignal(newId, args));
                    }
                    break;
                case DeactivationSignal:
                    if (_activationCount > 0)
                    {
                        _activationCount -= 1;
                        if (_activationCount == 0)
                        {
                            Emit(new DeactivationSignal((uint)_emittedId));
                            _emittedId = null;
                        }
                    }
                    break;
            }
        }
    }
}
