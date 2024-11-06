using System.Collections.Generic;

namespace ExcellentKit
{
    /// <summary>
    /// Emits all recieved messages, but only allow each signal type to be emitted once.
    /// </summary>
    public class SignalPipeWithSingleUse : SignalPipe
    {
        public HashSet<SignalType> UsedSignalTypes { get; set; } = new();

        protected override void OnSignalRecieved(Signal signal)
        {
            if (!UsedSignalTypes.Contains(signal.Type))
            {
                Emit(signal);
                UsedSignalTypes.Add(signal.Type);
            }
        }

        protected override string GetLabelTextForTarget(SignalReciever target)
        {
            return "Single use";
        }
    }
}
