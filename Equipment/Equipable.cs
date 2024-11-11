using UnityEngine;

namespace ExcellentKit
{
    [CreateAssetMenu(menuName = "Excellent/Equipable")]
    public class Equipable : ScriptableObject
    {
        [SerializeField]
        private Sprite _displayImage;

        [SerializeField]
        private EquipmentType _equipmentType = EquipmentType.Primary;

        [SerializeField]
        private string _itemName = "An item";

        [SerializeField]
        private int _sortOrder = 0;

        [SerializeField]
        private string _description = "An item description";

        [SerializeField]
        private EquippedItem _itemPrefab;

        public EquipmentType EquipmentType => _equipmentType;
        public int SortOrder => _sortOrder;
        public EquippedItem ItemPrefab => _itemPrefab;
        public Sprite DisplayImage => _displayImage;
        public string ItemName => _itemName;
        public string Description => _description;
    }

    public enum EquipmentType
    {
        Primary,
        Secondary
    }
}
