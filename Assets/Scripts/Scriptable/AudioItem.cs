using BattleCity.Managers.Game;
using UnityEngine;

namespace BattleCity.Scriptable
{
    public class AudioItem : MonoBehaviour
    {
        [System.Serializable]
        public class SpellAnimationEntry
        {
            [SerializeField]
            private SoundTypes _audioType;
            [SerializeField]
            private AudioClip _audioClip;

            public AudioClip AudioClip => _audioClip;
            public SoundTypes AudioType => _audioType;
        }
    }
}
