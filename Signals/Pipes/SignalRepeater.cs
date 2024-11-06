namespace ExcellentKit
{
    /// <summary>
    /// Emits all recieved signals with no extra functionality.
    /// </summary>
    public class SignalRepeater : SignalPipe
    {
        protected override void OnSignalRecieved(Signal signal)
        {
            Emit(signal);
        }

        protected override string GetLabelTextForTarget(SignalReciever target)
        {
            return "Repeated";
        }
    }
}
