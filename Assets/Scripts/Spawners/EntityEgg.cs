using BattleCity.Animations;
using BattleCity.Managers.Game;
using BattleCity.Stats;
using UnityEngine;

namespace BattleCity.Spawners
{
    public class EntityEgg : MonoBehaviour
    {
        private EntityStats _entityToSpawn;
        private EntityFabric _entityFabric;
        private LiteAnimation _spawnAnimation;

        public EntityStats ContainingEntity => _entityToSpawn;

        public event SpawnerEggEvents OnEntitySpawned;

        private void OnDisable()
        {
            OnEntitySpawned = null;
        }

        public void Initialize(EntityStats entity)
        {
            _entityToSpawn = entity;
            LiteAnimationCollection animationCollection = LiteAnimationCollection.CurrentInstance;
            if (animationCollection != null)
            {
                LiteAnimation shieldAnimation = animationCollection.GetAnimation(LiteAnimationTypes.TankSpawn);
                _spawnAnimation = Instantiate(shieldAnimation, transform);
                _spawnAnimation.SetVisible(false);
                _spawnAnimation.AnimationEnded += OnSpawnAnimationEnded;
            }
        }

        public void PerformSpawn(EntityFabric fabric, bool doAnimation = true)
        {
            _entityFabric = fabric;

            if (_spawnAnimation != null && doAnimation)
            {
                _spawnAnimation.SetVisible(true);
            }
            else
            {
                SpawnEntityAndDesrotySelf();
            }
        }

        private void OnSpawnAnimationEnded()
        {
            SpawnEntityAndDesrotySelf();
        }

        private void SpawnEntityAndDesrotySelf()
        {
            EntityStats spawnedEntity = Instantiate(_entityToSpawn, transform.position, new Quaternion());
            _entityFabric.OnEntitySpawned(spawnedEntity);
            OnEntitySpawned?.Invoke(spawnedEntity);
            Destroy(gameObject);
        }

        public delegate void SpawnerEggEvents(EntityStats entity);
    }
}
