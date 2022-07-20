using BattleCity.Animations;
using BattleCity.Managers.Game;
using BattleCity.Movement;
using BattleCity.Shooting;
using System.Collections;
using UnityEngine;

namespace BattleCity.Stats
{
    public class PlayerStats : EntityStats
    {
        [SerializeField]
        private SpriteRenderer _sprite;
        [SerializeField]
        private TankMovement _movement;
        [SerializeField]
        private ShootController _shootController;
        [SerializeField, Range(0f, 5f), Space(15)]
        private float _protectionTime = 3;

        private bool _underProtection;
        private LiteAnimation _shieldAnimation;

        public override void Initialize()
        {
            LiteAnimationCollection animationCollection = LiteAnimationCollection.CurrentInstance;

            if (animationCollection != null)
            {
                LiteAnimation shieldAnimation = animationCollection.GetAnimation(LiteAnimationTypes.TankProtection);
                _shieldAnimation = Instantiate(shieldAnimation, transform);
                _shieldAnimation.SetVisible(false);
            }
        }

        public void SetKnockDown(float time)
        {
            StartCoroutine(KnockOutCoroutine(time));
        }

        public void EnableProtectionAfterSpawn()
        {
            StartCoroutine(ProtectionCoroutine());
        }

        public override void SetDamage(int damage)
        {
            if (_underProtection)
            {
                return;
            }
            base.SetDamage(damage);
        }

        private IEnumerator ProtectionCoroutine()
        {
            _underProtection = true;
            _shieldAnimation.SetVisible(true);
            yield return new WaitForSeconds(_protectionTime);
            _shieldAnimation.SetVisible(false);
            _underProtection = false;
        }

        private IEnumerator KnockOutCoroutine(float time)
        {
            _movement.InputEnabled = false;
            _shootController.InputEnabled = false;
            StartCoroutine(BlinkAnimation());
            yield return new WaitForSeconds(time);
            _movement.InputEnabled = true;
            _shootController.InputEnabled = true;
        }

        private IEnumerator BlinkAnimation()
        {
            while (!_movement.InputEnabled)
            {
                _sprite.enabled = !_sprite.enabled;
                yield return new WaitForSeconds(0.2f);
            }
            _sprite.enabled = true;
        }
    }
}
