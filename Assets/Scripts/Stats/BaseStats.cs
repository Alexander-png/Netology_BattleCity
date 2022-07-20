using UnityEngine;

namespace BattleCity.Stats
{
    public class BaseStats : EntityStats
    {
        [SerializeField]
        private SpriteRenderer _sprite;
        [SerializeField]
        private Sprite _destoryedSprite;

        public override void SetDamage(int damage)
        {
            Health -= damage;

            if (Health <= 0)
            {
                InvokeOnDestroyed();
                PlayDestroySound();
                _sprite.sprite = _destoryedSprite;
            }
        }
    }
}
