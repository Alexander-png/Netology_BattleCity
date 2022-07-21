using BattleCity.Managers.Map;
using BattleCity.Stats;
using System;
using UnityEditor;
using UnityEngine;

namespace BattleCity.Managers.Game
{
    public class GameManager : MonoBehaviour
    {
        [SerializeField]
        private Transform _levelContainer;
        [SerializeField]
        private BaseMapManager[] _levels;
        [SerializeField]
        private int _startLevelIndex = 0;
        [SerializeField]
        private EntityFabric _entityFabric;

        [SerializeField, Space(15)]
        private int _livesLeft_1Player = 2;
        [SerializeField]
        private int _livesLeft_2Player = 2;

        private int _currentLevelIndex = -1;
        private BaseMapManager _instatiatedLevel;

        private void Start()
        {
            _entityFabric.OnPreInitialize();
            _entityFabric.OnPlayerDestroyed += OnPlayerDestroyed;
            _entityFabric.OnEnemyDestroyed += OnEnemyDestoyed;
            _entityFabric.OnEnemiesLeft += OnEnemiesLeft;
            if (_currentLevelIndex == -1)
            {
                StartGameFromLevel(_levels[_startLevelIndex]);
            }
        }

        private void OnDisable()
        {
            _entityFabric.OnPlayerDestroyed -= OnPlayerDestroyed;
            _entityFabric.OnEnemyDestroyed -= OnEnemyDestoyed;
            _entityFabric.OnEnemiesLeft -= OnEnemiesLeft;
        }

        private void OnEnemyDestoyed(EntityStats stats)
        {
            //int bounty = (stats as EnemyStats).Bounty;
        }

        private void StartGameFromLevel(BaseMapManager levelToLoad)
        {
            _currentLevelIndex = Array.IndexOf(_levels, levelToLoad);
            _instatiatedLevel = Instantiate(levelToLoad, _levelContainer);
            _instatiatedLevel.gameObject.SetActive(true);
            _entityFabric.OnStageStart(_instatiatedLevel);
        }

        private void LoadNextLevel()
        {
            _entityFabric.ResetEntities();
            _currentLevelIndex++;
            if (_currentLevelIndex >= _levels.Length)
            {
                Debug.Log("Todo: Game complete!");
                Debug.Break();
                return;
            }
            _instatiatedLevel = Instantiate(_levels[_currentLevelIndex], _levelContainer);
            _instatiatedLevel.gameObject.SetActive(true);
            _entityFabric.OnStageStart(_instatiatedLevel);
        }

        private void OnPlayerDestroyed(EntityStats stats)
        {
            if (stats.gameObject.name.Contains("Player1"))
            {
                _livesLeft_1Player -= 1;
                if (AreLivesLeft(_livesLeft_1Player))
                {
                    _entityFabric.RespawnPlayer("Player1");
                }
            }
            else if (stats.gameObject.name.Contains("Player2"))
            {
                _livesLeft_2Player -= 1;
                if (AreLivesLeft(_livesLeft_2Player))
                {
                    _entityFabric.RespawnPlayer("Player2");
                }
            }
        }

        private bool AreLivesLeft(int livesLeft) => livesLeft >= 0;

        private void OnEnemiesLeft()
        {
            Destroy(_instatiatedLevel.gameObject);
            _instatiatedLevel = null;
            LoadNextLevel();
        }
    }
}
