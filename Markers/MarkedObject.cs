using System.Collections.Generic;
using System.Linq;
using Alchemy.Inspector;
using ExcellentKit;
using UnityEngine;

namespace ExcellentGame
{
    public class MarkedObject : MonoBehaviour
    {
        [SerializeField, ListViewSettings(ShowFoldoutHeader = false)]
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
