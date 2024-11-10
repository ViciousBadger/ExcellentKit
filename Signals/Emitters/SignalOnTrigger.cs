using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace ExcellentKit
{
    /// <summary>
    /// <para>
    /// Emits Activate/Deactivate signals when recieving OnTriggerEnter/OnTriggerExit messages.
    /// </para>
    /// <para>
    /// Very flexible - can be specialized to e.g. only react to the player or only activate for the first collider to enter and the last to leave.
    /// </para>
    /// </summary>
    public class SignalOnTrigger : SignalEmitter
    {
        [SerializeField]
        private float _activationDelay = 0;

        [SerializeField]
        private float _deactivationDelay = 0;

        [SerializeField]
        private Marker _requiredMarker;

        private readonly Dictionary<Collider, Overlap> _overlaps = new();

        class Overlap
        {
            public int OverlapCount { get; set; }
            public float DelayTimer { get; set; }
            public uint? ActivatedSignalId { get; set; } = null;
        }

        private void Update()
        {
            var overlapsToRemove = new List<Collider>();

            foreach (var pair in _overlaps)
            {
                var overlap = pair.Value;

                if (overlap.DelayTimer > 0)
                {
                    overlap.DelayTimer -= Time.deltaTime;
                }

                if (
                    overlap.OverlapCount > 0
                    && overlap.ActivatedSignalId == null
                    && overlap.DelayTimer <= 0
                )
                {
                    var newId = SignalId.Next();
                    overlap.ActivatedSignalId = newId;

                    Emit(
                        new ActivationSignal(
                            newId,
                            new SignalArgs() { Subject = pair.Key.gameObject }
                        )
                    );
                }

                if (overlap.OverlapCount <= 0 && overlap.ActivatedSignalId == null)
                {
                    // Was never activated, just remove!
                    overlapsToRemove.Add(pair.Key);
                }

                if (
                    overlap.OverlapCount <= 0
                    && overlap.ActivatedSignalId != null
                    && overlap.DelayTimer <= 0
                )
                {
                    Emit(new DeactivationSignal((uint)overlap.ActivatedSignalId));
                    overlapsToRemove.Add(pair.Key);
                }
            }

            foreach (var coll in overlapsToRemove)
            {
                _overlaps.Remove(coll);
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!_requiredMarker || other.gameObject.HasMarker(_requiredMarker))
            {
                if (_overlaps.ContainsKey(other))
                {
                    var overlap = _overlaps[other];
                    overlap.OverlapCount++;
                }
                else
                {
                    _overlaps.Add(
                        other,
                        new Overlap { OverlapCount = 1, DelayTimer = _activationDelay }
                    );
                }
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (_overlaps.ContainsKey(other))
            {
                var overlap = _overlaps[other];
                overlap.OverlapCount--;
                if (overlap.OverlapCount <= 0)
                {
                    overlap.DelayTimer = _deactivationDelay;
                }
            }
        }

        protected override string GetLabelTextForTarget(SignalReciever target)
        {
            var str = new StringBuilder();
            str.AppendLine("On trigger");

            if (_requiredMarker)
            {
                str.AppendLine(string.Format("Marker: {0}", _requiredMarker.name));
            }

            if (_activationDelay > 0f || _deactivationDelay > 0f)
            {
                str.AppendLine(string.Format("{0}s / {1}s", _activationDelay, _deactivationDelay));
            }

            // Remove last line break
            str.Length--;

            return str.ToString();
        }
    }
}
