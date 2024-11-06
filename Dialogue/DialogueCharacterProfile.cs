using UnityEngine;

namespace ExcellentGame
{
    [CreateAssetMenu(menuName = "Excellent/Dialogue Character Profile")]
    public class DialogueCharacterProfile : ScriptableObject
    {
        [SerializeField]
        private string _characterName;

        [SerializeField, Tooltip("In symbols per second")]
        private float _talkingSpeed = 40f;

        [SerializeField]
        private AudioClip _talkingSound;

        [SerializeField, Min(0.1f)]
        private float _talkingSoundInterval = 0.5f;

        [SerializeField]
        private float _talkingSoundMinPitch = 0.95f;

        [SerializeField]
        private float _talkingSoundMaxPitch = 1.05f;

        public string CharacterName => _characterName;
        public float TalkingSpeed => _talkingSpeed;
        public AudioClip TalkingSound => _talkingSound;
        public float TalkingSoundInterval => _talkingSoundInterval;
        public float TalkingSoundMinPitch => _talkingSoundMinPitch;
        public float TalkingSoundMaxPitch => _talkingSoundMaxPitch;
    }
}
