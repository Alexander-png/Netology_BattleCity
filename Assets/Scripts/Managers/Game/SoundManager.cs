using UnityEngine;
using static BattleCity.Scriptable.AudioItem;

namespace BattleCity.Managers.Game
{
    public enum SoundTypes : byte
    {
        None = 0,
        BonusPick = 1,
        ProjectileExplosion = 2,
        EnemyTankExplosion = 3,
        PlayerTankExplosion = 4,
        GameOver = 5,
        Ice = 6,
        LevelStart = 7,
        LifeAdd = 8,
        PlayerMove = 9,
        PlayerIdle = 10,
        GamePause = 11,
        ShieldHit = 12,
        PlayerShoot = 13,
        SteelHit = 14,
        BonusPickUp = 15,
        LevanPolka = 16,
    }

    public class SoundManager : MonoBehaviour
    {
        [SerializeField]
        private AudioSource _source;

        [SerializeField, Space(15)]
        private SpellAnimationEntry[] _sounds;

        public static SoundManager CurrentInstance { get; private set; }

        public void PlaySound(SoundTypes sound)
        {
            
        }
    }
}
