using UnityEngine;

namespace ExcellentKit
{
    [CreateAssetMenu(menuName = "Excellent/Damage Source")]
    public class DamageType : ScriptableObject
    {
        [SerializeField]
        private string _label;

        [SerializeField]
        private float _recoveryTime = 10f;

        [SerializeField]
        private Color _color = Color.white;

        [SerializeField]
        private DamageEffect _effectPrefab;

        public string Label => _label;
        public float RecoveryTime => _recoveryTime;
        public Color Color => _color;
        public DamageEffect EffectPrefab => _effectPrefab;
    }
}
