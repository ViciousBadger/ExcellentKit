using ExcellentGame;
using UnityEngine;

namespace ExcellentKit
{
    /// <summary>
    /// <para>
    /// Reacts to OnHoverStart and OnHoverEnd events from the player interaction system.
    /// </para>
    /// <para>
    /// The signal's subject will be set to the hovering player.
    /// </para>
    /// </summary>
    [RequireComponent(typeof(Interactable))]
    public class SignalOnHover : MultiTrackingSignalEmitter<GameObject>
    {
        private Interactable _interactable;

        protected void Awake()
        {
            _interactable = GetComponent<Interactable>();
        }

        protected void OnEnable()
        {
            _interactable.HoverStarted += OnHoverStart;
            _interactable.HoverEnded += OnHoverEnd;
        }

        protected void OnDisable()
        {
            _interactable.HoverStarted -= OnHoverStart;
            _interactable.HoverEnded -= OnHoverEnd;
        }

        private void OnHoverStart(HoverArgs args)
        {
            // We don't expect multiple simultaneous
            // "hoverings" by the same actor.
            ActivateTracked(args.Actor, new() { Subject = args.Actor });
        }

        private void OnHoverEnd(HoverArgs args)
        {
            DeactivateTracked(args.Actor);
        }

        protected override string GetLabelTextForTarget(SignalReciever target)
        {
            return "On hover";
        }
    }
}
