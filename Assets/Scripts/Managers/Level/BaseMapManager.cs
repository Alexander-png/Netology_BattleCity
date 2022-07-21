using System;
using UnityEngine;

namespace BattleCity.Managers.Map
{
    public abstract class BaseMapManager : MonoBehaviour
    {
        [SerializeField, Min(0)]
        private byte _maxEnemiesOnMapOnePlayer = 4;
        [SerializeField, Min(0)]
        private byte _maxEnemiesOnMapTwoPlayer = 6;
        [SerializeField, Min(0)]
        private byte _enemiesOnLevel = 20;
        [SerializeField, Tooltip("Indexes of enemies, that contains a bonus (\",\" separated).")]
        private string _bonusEnemies = "4,11,18";
        [SerializeField]
        private Transform[] _spawnPoints;
        [SerializeField]
        private Transform[] _playerSpawnPoints;
        [SerializeField]
        private Transform[] _basePlaces;

        public byte MaxEnemiesOnMapOnePlayer => _maxEnemiesOnMapOnePlayer;
        public byte MaxEnemiesOnMapTwoPlayer => _maxEnemiesOnMapTwoPlayer;
        public byte EnemiesOnLevel => _enemiesOnLevel;
        public string BonusEnemies => _bonusEnemies;
        public Transform[] EnemySpawnPoints
        { 
            get 
            {
                Transform[] toReturn = new Transform[_spawnPoints.Length];
                Array.Copy(_spawnPoints, toReturn, _spawnPoints.Length);
                return toReturn;
            }
        }
        public Transform[] PlayerSpawnPoints
        {
            get
            {
                Transform[] toReturn = new Transform[_playerSpawnPoints.Length];
                Array.Copy(_playerSpawnPoints, toReturn, _playerSpawnPoints.Length);
                return toReturn;
            }
        }
        public Transform[] BasePlaces
        {
            get
            {
                Transform[] toReturn = new Transform[_basePlaces.Length];
                Array.Copy(_basePlaces, toReturn, _basePlaces.Length);
                return toReturn;
            }
        }

        private void OnDisable()
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                Destroy(transform.GetChild(i).gameObject);
            }
        }
    }
}
