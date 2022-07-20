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

        public void PerformSpawn(EntityFabric fabric, EntityStats entity, bool doAnimation = true)
        {
            _entityFabric = fabric;
            _entityToSpawn = entity;

            LiteAnimationCollection animationCollection = LiteAnimationCollection.CurrentInstance;
            if (animationCollection != null && doAnimation)
            {
                LiteAnimation shieldAnimation = animationCollection.GetAnimation(LiteAnimationTypes.TankSpawn);
                _spawnAnimation = Instantiate(shieldAnimation, transform);
                _spawnAnimation.SetVisible(true);
                _spawnAnimation.AnimationEnded += OnSpawnAnimationEnded;
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
            Destroy(this);
        }
    }
}
