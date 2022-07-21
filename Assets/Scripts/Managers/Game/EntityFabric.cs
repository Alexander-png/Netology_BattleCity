using BattleCity.AI;
using BattleCity.Assistance.Static;
using BattleCity.Managers.Map;
using BattleCity.Spawners;
using BattleCity.Stats;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BattleCity.Managers.Game
{
    public class EntityFabric : MonoBehaviour
    {
        [SerializeField]
        private float _enemySpawnDelay = 1;

        [SerializeField]
        private EnemyStats[] _enemies;

        [SerializeField]
        private BaseStats _base;

        [SerializeField, Space(15)]
        private PlayerStats _player1;
        [SerializeField]
        private PlayerStats _player2;

        [SerializeField, Space(15)]
        private EntityEgg _entityEgg;

        [SerializeField]
        private SelectedGameMode _currentGameMode;

        private Transform[] _enemySpawnPoints;
        private Transform[] _playerSpawnPoints;

        private PlayerStats _currentPlayer1Object;
        private PlayerStats _currentPlayer2Object;
        private List<BaseStats> _currentBaseObjects;
        private int _baseHealth;

        private byte _enemiesLeft;
        private byte _maxAliveEnemies;
        private byte _currentAliveEnemies;
        private int _spawnedEnemyCount;
        private int[] _enemiesWithBonus;

        public event EnemySpawnEvents EnemiesLeft;
        public event EntityEvents PlayerDestroyed;
        public event EntityEvents EnemyDestroyed;
        public event EntityEvents BaseDestroyed;
        public event EntityPreSpawnEvents PlayerPreSpawn;

        private void OnDisable()
        {
            _player1.OnDestroyed -= OnPlayerDestroyedInternal;
            _player2.OnDestroyed -= OnPlayerDestroyedInternal;
            foreach (BaseStats baseItem in _currentBaseObjects)
            {
                baseItem.OnDestroyed -= OnBaseDestroyedInternal;
            }
            _currentBaseObjects.Clear();

            EnemiesLeft = null;
            PlayerDestroyed = null;
            EnemyDestroyed = null;
            BaseDestroyed = null;
            PlayerPreSpawn = null;
        }

        public void OnPreInitialize()
        {
            _currentBaseObjects = new List<BaseStats>();
            if (GameStaticVariables.GameMode != SelectedGameMode.None)
            {
                _currentGameMode = GameStaticVariables.GameMode;
            }
        }

        public void ResetEntities()
        {
            if (_currentPlayer1Object != null)
            {
                _currentPlayer1Object.OnDestroyed -= OnPlayerDestroyedInternal;
                Destroy(_currentPlayer1Object.gameObject);
            }
            if (_currentPlayer2Object != null)
            {
                _currentPlayer2Object.OnDestroyed -= OnPlayerDestroyedInternal;
                Destroy(_currentPlayer2Object.gameObject);
            }

            foreach (BaseStats baseItem in _currentBaseObjects)
            {
                baseItem.OnDestroyed -= OnBaseDestroyedInternal;
                Destroy(baseItem.gameObject);
            }
            _currentBaseObjects.Clear();
        }

        public void OnStageStart(BaseMapManager map)
        {
            map.Initialize(this);

            _enemySpawnPoints = map.EnemySpawnPoints;
            _playerSpawnPoints = map.PlayerSpawnPoints;
            _enemiesLeft = map.EnemiesOnLevel;
            _baseHealth = map.BaseHealth;

            switch (_currentGameMode)
            {
                case SelectedGameMode.Mode_1Player:
                    _maxAliveEnemies = map.MaxEnemiesOnMapOnePlayer;
                    break;
                case SelectedGameMode.Mode_2Player:
                    _maxAliveEnemies = map.MaxEnemiesOnMapTwoPlayer;
                    break;
            }

            string[] numbers = map.BonusEnemies.Split(',');
            _enemiesWithBonus = new int[numbers.Length];
            for (int i = 0; i < _enemiesWithBonus.Length; i++)
            {
                _enemiesWithBonus[i] = Convert.ToInt32(numbers[i]);
            }

            Transform[] _basePlaces = map.BasePlaces;
            for (int i = 0; i < _basePlaces.Length; i++)
            {
                SpawnEntity(_base, _basePlaces[i].position, false);
            }

            RespawnPlayers();
            StartCoroutine(EnemySpawnCoroutine());
        }

        private void RespawnPlayers()
        {
            SpawnEntity(_player1, _playerSpawnPoints[0].position);
            if (_currentGameMode == SelectedGameMode.Mode_2Player)
            {
                SpawnEntity(_player2, _playerSpawnPoints[1].position);
            }
        }

        public void RespawnPlayer(string player)
        {
            if (player.Contains("Player1"))
            {
                SpawnEntity(_player1, _playerSpawnPoints[0].position);
            }
            else if (player.Contains("Player2"))
            {
                SpawnEntity(_player2, _playerSpawnPoints[1].position);
            }
        }

        private void SpawnEntity(EntityStats entity, Vector2 position, bool doAnimation = true)
        {
            EntityEgg egg = Instantiate(_entityEgg, position, new Quaternion());
            egg.Initialize(entity);
            if (entity is PlayerStats)
            {
                PlayerPreSpawn?.Invoke(egg);
            }
            egg.PerformSpawn(this, doAnimation);
        }

        public void OnEntitySpawned(EntityStats entity)
        {
            if (entity is PlayerStats player)
            {
                if (player.gameObject.name.Contains("Player1"))
                {
                    _currentPlayer1Object = player;
                    _currentPlayer1Object.Initialize();
                    _currentPlayer1Object.EnableProtectionAfterSpawn();
                    _currentPlayer1Object.OnDestroyed += OnPlayerDestroyedInternal;
                }
                else if (player.gameObject.name.Contains("Player2"))
                {
                    _currentPlayer2Object = player;
                    _currentPlayer2Object.Initialize();
                    _currentPlayer2Object.EnableProtectionAfterSpawn();
                    _currentPlayer2Object.OnDestroyed += OnPlayerDestroyedInternal;
                }
            }
            else if (entity is EnemyStats enemy)
            {
                enemy.Initialize();
                enemy.OnDestroyed += OnEnemyDestroy;
                if (Array.IndexOf(_enemiesWithBonus, _spawnedEnemyCount) != -1)
                {
                    enemy.IsBonus = true;
                }
                _currentAliveEnemies++;
                _spawnedEnemyCount++;
                _enemiesLeft -= 1;
            }
            else if (entity is BaseStats newBase)
            {
                newBase.OnDestroyed += OnBaseDestroyedInternal;
                newBase.SetHealth(_baseHealth);
                _currentBaseObjects.Add(newBase);
            }
        }

        private IEnumerator EnemySpawnCoroutine()
        {
            while (true)
            {
                int enemyTypeIndex = UnityEngine.Random.Range(0, _enemies.Length);
                int spawnPointIndex = UnityEngine.Random.Range(0, _enemySpawnPoints.Length);

                if (_currentAliveEnemies < _maxAliveEnemies && _enemiesLeft > 0)
                {
                    SpawnEntity(_enemies[enemyTypeIndex], _enemySpawnPoints[spawnPointIndex].transform.position);
                }
                if (_currentAliveEnemies == 0 && _enemiesLeft == 0)
                {
                    break;
                }
                yield return new WaitForSeconds(_enemySpawnDelay);
            }
            ResetVariables();
            EnemiesLeft?.Invoke();
        }

        private void ResetVariables()
        {
            _spawnedEnemyCount = 0;
        }

        private void OnEnemyDestroy(EntityStats sender)
        {
            sender.OnDestroyed -= OnEnemyDestroy;
            _currentAliveEnemies--;
            EnemyDestroyed?.Invoke(sender);
        }

        private void OnPlayerDestroyedInternal(EntityStats sender)
        {
            sender.OnDestroyed -= OnPlayerDestroyedInternal;
            PlayerDestroyed?.Invoke(sender);
        }

        private void OnBaseDestroyedInternal(EntityStats sender)
        {
            sender.OnDestroyed -= OnBaseDestroyedInternal;
            _currentBaseObjects.Remove(sender as BaseStats);
            BaseDestroyed?.Invoke(sender);
        }

        public delegate void EnemySpawnEvents();
        public delegate void EntityEvents(EntityStats stats);
        public delegate void EntityPreSpawnEvents(EntityEgg egg);
    }
}
