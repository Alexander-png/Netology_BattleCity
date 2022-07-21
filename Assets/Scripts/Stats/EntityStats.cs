using BattleCity.Managers.Game;
using UnityEngine;

namespace BattleCity.Stats
{
    public enum SideType : byte
    {
        Players = 0,
        AI = 1,
    }

    public class EntityStats : MonoBehaviour
    {
        [SerializeField]
        private int _health;
        [SerializeField]
        private SideType _side;

        private SoundCollection _soundCollection;

        public int Health
        {
            get
            {
                return _health;
            }
            protected set
            {
                _health = value;
            }
        }

        public SideType Side => _side;

        public bool IsPlayer => _side == SideType.Players;

        public event EntityStatsEvents OnDestroyed;

        protected SoundCollection SoundCollection => _soundCollection;

        protected void InvokeOnDestroyed()
        {
            OnDestroyed?.Invoke(this);
        }

        private void OnDisable()
        {
            OnDestroyed = null;
        }

        public virtual void SetDamage(int damage)
        {
            _health -= damage;

            if (_health <= 0)
            {
                InstatiateExplosion();
                OnDestroyed?.Invoke(this);
                PlayDestroySound();
                Destroy(gameObject);
            }
        }

        protected virtual void PlayDestroySound()
        {
            
        }

        public virtual void Initialize()
        {
            _soundCollection = SoundCollection.CurrentInstance;
        }

        protected void InstatiateExplosion()
        {
            LiteAnimationCollection.CurrentInstance.PlayAnimation(transform.position, LiteAnimationTypes.TankExplosion);
        }

        public delegate void EntityStatsEvents(EntityStats sender);
    }
}
