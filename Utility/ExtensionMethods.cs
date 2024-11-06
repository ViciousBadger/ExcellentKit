using UnityEngine;

namespace ExcellentKit
{
    public static class ExtensionMethods
    {
        public static float Remap(this float value, float from1, float to1, float from2, float to2)
        {
            return (value - from1) / (to1 - from1) * (to2 - from2) + from2;
        }

        public static float Clamp(this float value, float min, float max)
        {
            return Mathf.Clamp(value, min, max);
        }

        public static bool HasMarker(this GameObject gameObject, Marker marker)
        {
            var markersComponent = gameObject.GetComponent<MarkedObject>();
            return markersComponent ? markersComponent.HasMarker(marker) : false;
        }
    }
}
