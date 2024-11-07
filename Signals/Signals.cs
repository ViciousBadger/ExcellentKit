#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ExcellentGame;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Events;

namespace ExcellentKit
{
    public enum SignalType
    {
        Activate,
        Deactivate
    }

    public static class SignalId
    {
        private static uint _nextId = 0;

        public static uint Next()
        {
            return _nextId++;
        }
    }

    public abstract record Signal(uint Id)
    {
        public abstract bool IsOfType(SignalType type);
    };

    public record ActivationSignal(uint Id, SignalArgs Args) : Signal(Id)
    {
        public override bool IsOfType(SignalType type)
        {
            return type == SignalType.Activate;
        }
    }

    public record DeactivationSignal(uint Id) : Signal(Id)
    {
        public override bool IsOfType(SignalType type)
        {
            return type == SignalType.Deactivate;
        }
    }

    public record SignalArgs
    {
        /// <summary>
        /// An optional "subject" of the signal. Use cases include:
        /// <list type="bullet">
        ///     <item>A subject that entered a trigger collider</item>
        ///     <item>A subject that interacted with something.</item>
        ///     <item>A subject that is being "dialogued" with.</item>
        /// </list>
        /// </summary>
        public GameObject? Subject { get; init; }

        /// <summary>
        /// An optional "strength" of the signal for when a float value is useful. Defaults to zero.
        /// </summary>
        public float Strength { get; init; }

        /// <summary>
        /// An optional "message" for the signal, useful for filtering out certain signals in complex logic. Defaults to null.
        /// </summary>
        public string? Message { get; init; }

        public bool SubjectIsPlayer(out PlayerActor? player)
        {
            if (Subject != null)
            {
                return Subject.TryGetComponent(out player);
            }
            else
            {
                player = null;
                return false;
            }
        }
    }

    /// <summary>
    /// A function that takes a signal as its only argument.
    /// </summary>
    public delegate void SignalHandler(Signal signal);

    /// <summary>
    /// <para>
    /// Base class for components that subscribe and react to a signal reciever on the same GameObject.<br />
    /// Override OnSignalRecieved(Signal) to provide concrete behaviour.
    /// </para>
    /// <para>
    /// Make sure to consider all three signal types - Pulse, Activate and Deactivate.<br />
    /// Depending on the case, Pulse and Activate can run the same or differing behaviour.<br />
    /// It's okay to ignore Deactivate signals if they don't make sense in context.
    /// </para>
    /// </summary>
    [RequireComponent(typeof(SignalReciever))]
    public abstract class SignalBehaviour : MonoBehaviour
    {
        protected SignalReciever? _reciever;

        protected virtual void Awake()
        {
            _reciever = GetComponent<SignalReciever>();
        }

        // TODO: Consider if it's better to subscribe on awake and unsubscribe on destroy..
        // Having signals stop on inactive gameobjects may cause issues w/ signals never being deactivated in weird cases..
        protected virtual void OnEnable()
        {
            if (_reciever != null)
            {
                _reciever.SignalRecieved += OnSignalRecieved;
            }
        }

        protected virtual void OnDisable()
        {
            if (_reciever != null)
            {
                _reciever.SignalRecieved -= OnSignalRecieved;
            }
        }

        protected abstract void OnSignalRecieved(Signal signal);
    }

    /// <summary>
    /// <para>
    /// Base class for components that can emit signals to one or more signal recievers.
    /// Call Emit() with a new Signal when something should be emitted.
    /// </para>
    /// <para>
    /// Signal emitters are expected to emit an equal amount of Activate and Deactivate signals in correct order.
    /// </para>
    /// </summary>
    public abstract class SignalEmitter : MonoBehaviour
    {
        [SerializeField]
        protected SignalReciever[] _targets = new SignalReciever[0];

        public void Emit(Signal signal)
        {
            foreach (var reciver in _targets.Where(t => t != null))
            {
                reciver.Push(signal);
            }
        }

        protected abstract string GetLabelTextForTarget(SignalReciever target);

        [UsedImplicitly]
        private void OnDrawGizmos()
        {
            foreach (var target in _targets)
            {
                if (target != null)
                {
                    GizmosExtra.ColorPaletteSignalEmitter();
                    if (target.gameObject == gameObject)
                    {
                        GizmosExtra.DrawLabel(transform.position, GetLabelTextForTarget(target));
                    }
                    else
                    {
                        GizmosExtra.DrawArrow(
                            transform.position,
                            target.transform.position,
                            GetLabelTextForTarget(target)
                        );
                    }
                }
                else
                {
                    GizmosExtra.ColorPaletteError();
                    GizmosExtra.DrawLabel(transform.position, "A target is null");
                }
            }
            if (_targets.Length == 0)
            {
                GizmosExtra.ColorPaletteError();
                GizmosExtra.DrawLabel(
                    transform.position,
                    string.Format("{0} has no targets", GetType().Name)
                );
            }
        }
    }

    /// <summary>
    /// Base class for signal emitters that only expect to emit one active signal at a time.
    /// </summary>
    public abstract class SingleTrackingSignalEmitter : SignalEmitter
    {
        private uint? _activeSignalId = null;

        /// <summary>
        /// Emit an Activate signal and track its ID.
        /// </summary>
        public void ActivateTracked(SignalArgs args)
        {
            if (_activeSignalId == null)
            {
                var newId = SignalId.Next();
                _activeSignalId = newId;
                Emit(new ActivationSignal(newId, args));
            }
            else
            {
                Debug.LogWarning(
                    "Invalid behaviour: single tracking signal emitter was activated twice in a row."
                );
            }
        }

        /// <summary>
        /// Emit a Deactivate signal with the same ID as previous Activate signal.
        /// </summary>
        public void DeactivateTracked()
        {
            if (_activeSignalId != null)
            {
                Emit(new DeactivationSignal((uint)_activeSignalId));
            }
            else
            {
                Debug.LogWarning(
                    "Invalid behaviour: single tracking signal emitter was deactivated twice in a row."
                );
            }
        }
    }

    /// <summary>
    /// Base class for signal emitters that expect to handle multiple simultaneous active signals correctly.
    /// </summary>
    public abstract class MultiTrackingSignalEmitter<TKey> : SignalEmitter
    {
        private readonly Dictionary<TKey, uint> _activeSignals = new();

        /// <summary>
        /// Emit an Activate signal and track its ID and the supplied key.
        /// </summary>
        /// <param name="template">Optional signal to copy extra properties from.</param>
        public void ActivateTracked(TKey key, SignalArgs args)
        {
            var newId = SignalId.Next();
            _activeSignals.Add(key, newId);
            Emit(new ActivationSignal(newId, args));
        }

        /// <summary>
        /// Emit a Deactivate signal with the same ID as previous Activate signal using this key.
        /// </summary>
        /// <param name="template">Optional signal to copy extra properties from.</param>
        public void DeactivateTracked(TKey key)
        {
            if (_activeSignals.TryGetValue(key, out uint signalId))
            {
                _activeSignals.Remove(key);
                Emit(new DeactivationSignal(signalId));
            }
            else
            {
                Debug.Log(
                    "Invalid behaviour: MultiTrackingSignalEmitter Deactivate called with a missing key."
                );
            }
        }
    }

    /// <summary>
    /// <para>
    /// Base class for signal behaviours that can emit events themselves - like a "pipe" that spits out things that enter it.
    /// </para>
    /// <para>
    /// Usually some or all events recieved are "forwarded" to targets, sometimes with modifications applied.
    /// </para>
    /// </summary>
    public abstract class SignalPipe : SignalBehaviour
    {
        [SerializeField]
        protected SignalReciever[] _targets = new SignalReciever[0];

        public void Emit(Signal signal)
        {
            foreach (var reciever in _targets.Where(t => t != null))
            {
                reciever.Push(signal);
            }
        }

        protected abstract string GetLabelTextForTarget(SignalReciever target);

        [UsedImplicitly]
        private void OnDrawGizmos()
        {
            foreach (var target in _targets)
            {
                if (target != null)
                {
                    GizmosExtra.ColorPaletteSignalPipe();
                    if (target.gameObject == gameObject)
                    {
                        GizmosExtra.DrawLabel(transform.position, GetLabelTextForTarget(target));
                    }
                    else
                    {
                        GizmosExtra.DrawArrow(
                            transform.position,
                            target.transform.position,
                            GetLabelTextForTarget(target)
                        );
                    }
                }
                else
                {
                    GizmosExtra.ColorPaletteError();
                    GizmosExtra.DrawLabel(transform.position, "A target is null");
                }
            }
            if (_targets.Length == 0)
            {
                GizmosExtra.ColorPaletteError();
                GizmosExtra.DrawLabel(
                    transform.position,
                    string.Format("{0} has no targets", GetType().Name)
                );
            }
        }
    }
}
