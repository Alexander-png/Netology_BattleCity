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
        private AudioSource _audioSource;
        [SerializeField]
        private SpriteRenderer _sprite;
        [SerializeField]
        private TankMovement _movement;
        [SerializeField]
        private ShootController _shootController;
        [SerializeField, Range(0f, 5f), Space(15)]
        private float _protectionTime = 3;

        private bool _lastMoveState = false;
        private bool _underProtection;
        private LiteAnimation _shieldAnimation;
        private AudioClip _moveClip;
        private AudioClip _idleClip;

        public TankMovement Movement => _movement;

        public override void Initialize()
        {
            base.Initialize();
            LiteAnimationCollection animationCollection = LiteAnimationCollection.CurrentInstance;

            if (animationCollection != null)
            {
                LiteAnimation shieldAnimation = animationCollection.GetAnimation(LiteAnimationTypes.TankProtection);
                _shieldAnimation = Instantiate(shieldAnimation, transform);
                _shieldAnimation.SetVisible(false);
            }
            //_moveClip = SoundCollection.GetSound(SoundTypes.PlayerMove);
            //_idleClip = SoundCollection.GetSound(SoundTypes.PlayerIdle);
            //_audioSource.clip = _idleClip;
            //_audioSource.Play();
        }

        public void SetMovementEnabled(bool value)
        {
            _movement.InputEnabled = value;
            if (!value)
            {
                _audioSource.Stop();
            }
            else
            {
                _audioSource.Play();
            }
        }

        private void FixedUpdate()
        {
            //DetermineMoveSound();
        }

        private void DetermineMoveSound()
        {
            if (_lastMoveState != _movement.IsMoving)
            {
                if (_movement.IsMoving)
                {
                    _audioSource.clip = _moveClip;
                    _audioSource.Play();
                }
                else
                {
                    _audioSource.clip = _idleClip;
                    _audioSource.Play();
                }
            }
            _lastMoveState = _movement.IsMoving;
        }

        public void SetKnockDown(float time)
        {
            if (_underProtection)
            {
                return;
            }
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

        protected override void PlayDestroySound()
        {
            AudioSource.PlayClipAtPoint(SoundCollection.GetSound(SoundTypes.PlayerTankExplosion), transform.position);
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
            _movement.InputEnabledMaster = false;
            _shootController.InputEnabled = false;
            StartCoroutine(BlinkAnimation());
            yield return new WaitForSeconds(time);
            _movement.InputEnabledMaster = true;
            _shootController.InputEnabled = true;
        }

        private IEnumerator BlinkAnimation()
        {
            while (!_movement.InputEnabledMaster)
            {
                _sprite.enabled = !_sprite.enabled;
                yield return new WaitForSeconds(0.2f);
            }
            _sprite.enabled = true;
        }
    }
}
