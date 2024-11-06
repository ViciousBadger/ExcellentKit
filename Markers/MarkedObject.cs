using System.Collections.Generic;
using UnityEngine;

namespace ExcellentKit
{
    public class MarkedObject : MonoBehaviour
    {
        [SerializeField]
        private List<Marker> _markers;

        public List<Marker> Markers => _markers;

        public bool HasMarker(Marker marker)
        {
            return Markers.Contains(marker);
        }

        private void OnDrawGizmos()
        {
            GizmosExtra.ColorPaletteMarker();
            var markerList = string.Join(", ", _markers.ConvertAll(m => m.name));
            GizmosExtra.DrawLabel(transform.position, "Marked: " + markerList);
        }
    }
}
