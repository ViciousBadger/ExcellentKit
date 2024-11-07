using ExcellentGame;
using UnityEngine;

namespace ExcellentKit
{
    /// <summary>
    /// <para>
    /// Reacts to OnInteractionStart and OnInteractionEnd events from the player interaction system.
    /// </para>
    /// <para>
    /// The signal's subject will be set to the interacting player.
    /// </para>
    /// </summary>
    [RequireComponent(typeof(Interactable))]
    public class SignalOnInteract : MultiTrackingSignalEmitter<GameObject>
    {
        private Interactable _interactable;

        protected void Awake()
        {
            _interactable = GetComponent<Interactable>();
        }

        protected void OnEnable()
        {
            _interactable.InteractionStarted += OnInteractionStart;
            _interactable.InteractionEnded += OnInteractionEnd;
        }

        protected void OnDisable()
        {
            _interactable.InteractionStarted -= OnInteractionStart;
            _interactable.InteractionEnded -= OnInteractionEnd;
        }

        private void OnInteractionStart(InteractionArgs args)
        {
            // We don't expect multiple simultaneous
            // "interactions" by the same actor.
            ActivateTracked(args.Actor, new() { Subject = args.Actor });
        }

        private void OnInteractionEnd(InteractionArgs args)
        {
            DeactivateTracked(args.Actor);
        }

        protected override string GetLabelTextForTarget(SignalReciever target)
        {
            return "On interact";
        }
    }
}
