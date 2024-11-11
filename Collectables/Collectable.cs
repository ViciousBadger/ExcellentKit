using UnityEngine;

namespace ExcellentKit
{
    [CreateAssetMenu(menuName = "ExcellentKit/Collectable")]
    public class Collectable : ScriptableObject
    {
        [SerializeField]
        private Sprite _displayImage;

        [SerializeField]
        private string _itemName = "A thing";

        [SerializeField]
        private int _sortOrder = 0;

        [SerializeField]
        private bool _showOnCollect = true;

        public string ItemName => _itemName;
        public int SortOrder => _sortOrder;
        public Sprite DisplayImage => _displayImage;
        public bool ShowOnCollect => _showOnCollect;
    }
}
