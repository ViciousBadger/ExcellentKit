#nullable enable

using ExcellentKit;
using UnityEngine;

namespace ExcellentKit
{
    public static class SignalExtensions
    {
        public static bool SubjectIsPlayer(this SignalArgs value, out IPlayer? player)
        {
            if (value.Subject != null)
            {
                return value.Subject.TryGetComponent(out player);
            }
            else
            {
                player = null;
                return false;
            }
        }
    }
}
