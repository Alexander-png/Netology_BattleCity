using BattleCity.Managers.Game;
using UnityEngine;

namespace BattleCity.Stats
{
    public class EnemyStats : EntityStats
    {
        [SerializeField]
        private int _bounty;

        private bool _isBonus;

        public bool IsBonus
        {
            get => _isBonus;
            set
            {
                _isBonus = value;
            }
        }

        public int Bounty => _bounty;

        public override void SetDamage(int damage)
        {
            base.SetDamage(damage);
            if (Health > 0)
            {
                PlayShieldHitSound();
            }
        }

        private void PlayShieldHitSound()
        {
            AudioSource.PlayClipAtPoint(SoundCollection.GetSound(SoundTypes.ShieldHit), transform.position);
        }

        protected override void PlayDestroySound()
        {
            AudioSource.PlayClipAtPoint(SoundCollection.GetSound(SoundTypes.EnemyTankExplosion), transform.position);
        }
    }
}
