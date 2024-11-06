using System;

namespace ExcellentGame
{
    public class DialogueProcess
    {
        public DialogueState State { get; private set; }
        public DialogueEntry ActiveEntry { get; private set; }

        public event Action<DialogueChoice> ChoiceSelected;
        public event Action<DialogueEntry> ActiveEntryChanged;
        public event Action Exited;

        public DialogueProcess(DialogueState state, DialogueEntry initialEntry)
        {
            State = state;
            ActiveEntry = initialEntry;
        }

        public void SelectChoice(DialogueChoice choice)
        {
            if (choice)
            {
                ChoiceSelected?.Invoke(choice);
                var nextEntry = choice.NextEntry.Test(State);
                if (nextEntry)
                {
                    ActiveEntry = nextEntry;
                    ActiveEntryChanged?.Invoke(ActiveEntry);
                }
                else
                {
                    Exit();
                }
            }
            else
            {
                Exit();
            }
        }

        public void Exit()
        {
            Exited?.Invoke();
        }
    }
}
