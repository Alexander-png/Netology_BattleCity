using BattleCity.Managers.Game;
using BattleCity.Movement.Base;
using BattleCity.Spawners;
using BattleCity.Stats;
using System.Collections;
using UnityEngine;

namespace BattleCity.Managers.Map
{
    public class RollwarManager : BaseMapManager
    {
        [SerializeField]
        private Transform _playerPlaceContainer;
        [SerializeField]
        private AudioSource _levelAudioSource;

        [SerializeField, Min(0), Space(15)]
        private float _rollSpeed;
        [SerializeField, Range(0, 5)]
        private float _maxContinuousRollTime = 5;
        [SerializeField, Range(0, 5)]
        private float _minContinuousRollTime = 1;

        private EntityFabric _entityFabric;
        private EntityEgg _player1SpawnEgg;
        private EntityEgg _player2SpawnEgg;

        private PlayerStats _player1;
        private PlayerStats _player2;
        private Coroutine _rollCoroutine;

        private float _estimatedTime = 0f;
        private int _direction = 1;

        protected override void OnDisable()
        {
            base.OnDisable();

            if (_entityFabric != null)
            {
                _entityFabric.PlayerPreSpawn -= OnPlayerPreSpawn;
            }
            if (_player1 != null)
            {
                _player1.OnDestroyed -= OnPlayerDestroyed;
                _player1.Movement.InputEnabled = true;
            }
            if (_player2 != null)
            {
                _player2.OnDestroyed -= OnPlayerDestroyed;
                _player2.Movement.InputEnabled = true;
            }
            StopCoroutine(_rollCoroutine);
            _levelAudioSource.Stop();
        }

        private void Start()
        {
            _levelAudioSource.clip = SoundCollection.CurrentInstance.GetSound(SoundTypes.LevanPolka);
            _levelAudioSource.Play();
            _rollCoroutine = StartCoroutine(RollCoroutine());
        }

        private IEnumerator RollCoroutine()
        {
            _estimatedTime = Random.Range(_minContinuousRollTime, _maxContinuousRollTime);

            while (true)
            {
                if (_estimatedTime <= 0)
                {
                    _direction = -_direction;
                    _estimatedTime = Random.Range(_minContinuousRollTime, _maxContinuousRollTime);
                }
                _estimatedTime -= Time.deltaTime;

                Vector3 rotation = _playerPlaceContainer.rotation.eulerAngles;
                rotation.z += _rollSpeed * _direction * Time.deltaTime;
                _playerPlaceContainer.rotation = Quaternion.Euler(rotation);

                CorrectPlayerRotation(_player1);
                CorrectPlayerRotation(_player2);

                yield return new WaitForEndOfFrame();
            }
        }

        private void CorrectPlayerRotation(PlayerStats player)
        {
            if (player != null)
            {
                float globalPlayerRotation = MovementConstraints.GetRotationAngle(player.Movement.GetDirection());
                player.transform.rotation = Quaternion.Euler(0, 0, globalPlayerRotation);
            }
        }

        public override void Initialize(EntityFabric fabric)
        {
            _entityFabric = fabric;
            _entityFabric.PlayerPreSpawn += OnPlayerPreSpawn;
        }

        private void OnPlayerPreSpawn(EntityEgg entityEgg)
        {
            string playerObjectName = entityEgg.ContainingEntity.gameObject.name;
            if (playerObjectName.Contains("Player1"))
            {
                _player1SpawnEgg = entityEgg;
                _player1SpawnEgg.transform.SetParent(PlayerSpawnPoints[0]);
                _player1SpawnEgg.OnEntitySpawned += OnPlayerSpawned;
            }
            else if (playerObjectName.Contains("Player2"))
            {
                _player2SpawnEgg = entityEgg;
                _player2SpawnEgg.transform.SetParent(PlayerSpawnPoints[1]);
                _player2SpawnEgg.OnEntitySpawned += OnPlayerSpawned;
            }
        }

        private void OnPlayerSpawned(EntityStats entity)
        {
            string playerObjectName = entity.gameObject.name;
            if (playerObjectName.Contains("Player1"))
            {
                _player1SpawnEgg.OnEntitySpawned -= OnPlayerSpawned;
                _player1SpawnEgg = null;

                entity.gameObject.transform.SetParent(PlayerSpawnPoints[0]);
                _player1 = entity as PlayerStats;
                _player1.Movement.InputEnabled = false;
                _player1.OnDestroyed += OnPlayerDestroyed;
            }
            else if (playerObjectName.Contains("Player2"))
            {
                _player2SpawnEgg.OnEntitySpawned -= OnPlayerSpawned;
                _player2SpawnEgg = null;

                entity.gameObject.transform.SetParent(PlayerSpawnPoints[1]);
                _player2 = entity as PlayerStats;
                _player2.Movement.InputEnabled = false;
                _player2.OnDestroyed += OnPlayerDestroyed;
            }
        }

        private void OnPlayerDestroyed(EntityStats sender)
        {
            string name = sender.gameObject.name;
            if (name.Contains("Player1"))
            {
                _player1.OnDestroyed -= OnPlayerDestroyed;
                _player1 = null;
            }
            else if (name.Contains("Player2"))
            {
                _player2.OnDestroyed -= OnPlayerDestroyed;
                _player2 = null;
            }
        }
    }
}
