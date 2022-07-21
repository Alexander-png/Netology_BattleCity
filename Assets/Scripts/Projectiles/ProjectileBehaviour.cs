using BattleCity.Managers.Game;
using BattleCity.Map.Base;
using BattleCity.Movement.Base;
using BattleCity.Stats;
using UnityEngine;

namespace BattleCity.Projectiles
{
    public class ProjectileBehaviour : EntityMovement
    {
        [SerializeField]
        private float _lifeTime;
        [SerializeField]
        private int _damage;
        [SerializeField]
        private int _knockDownTime = 3;

        public SideType Side { get; set; }

        private void Start()
        {
            Destroy(gameObject, _lifeTime);
        }

        public override void SetMoveDirection(Direction newDirection)
        {
            base.SetMoveDirection(newDirection);
            _body.velocity = MovementConstraints.GetVectorMovement(newDirection) * _speed;
            _body.rotation = MovementConstraints.GetDirection(newDirection);
        }

        protected override void MovementLogic()
        {
            // empty
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            SoundCollection sounds = SoundCollection.CurrentInstance;
            bool isSentByMob = Side != SideType.Players;

            if (collision.gameObject.TryGetComponent(out MapElement element))
            {
                element.OnProjectileCollision(this);
                if (!isSentByMob)
                {
                    if (element.Destoryable)
                    {
                        AudioSource.PlayClipAtPoint(sounds.GetSound(SoundTypes.ProjectileExplosion), transform.position);
                    }
                    else
                    {
                        AudioSource.PlayClipAtPoint(sounds.GetSound(SoundTypes.SteelHit), transform.position);
                    }
                }
            }
            else if (collision.gameObject.TryGetComponent(out EnemyStats enemy))
            {
                if (enemy.Side != Side)
                {
                    enemy.SetDamage(_damage);
                }
            }
            else if (collision.gameObject.TryGetComponent(out PlayerStats player))
            {
                if (player.Side != Side)
                {
                    player.SetDamage(_damage);
                }
                if (player.Side == Side && Side == SideType.Players)
                {
                    player.SetKnockDown(_knockDownTime);
                }
            }
            else if (collision.gameObject.TryGetComponent(out BaseStats baseStats))
            {
                baseStats.SetDamage(_damage);
            }

            LiteAnimationCollection.CurrentInstance.PlayAnimation(transform.position, LiteAnimationTypes.ProjectileExplosion);
            if (!isSentByMob)
            {
                PlayExplosionSound();
            }
            Destroy(gameObject);
        }

        private void PlayExplosionSound()
        {
            AudioSource.PlayClipAtPoint(SoundCollection.CurrentInstance.GetSound(SoundTypes.ProjectileExplosion), transform.position);
        }
    }
}
