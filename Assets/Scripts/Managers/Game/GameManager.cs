using BattleCity.Assistance.Static;
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
        [SerializeField]
        private SpriteRenderer _gameOverSprite;

        [SerializeField, Space(15)]
        private int _livesLeft_1Player = 2;
        [SerializeField]
        private int _livesLeft_2Player = 2;

        private int _currentLevelIndex = -1;
        private bool _gameOverLogicExecuted;
        private BaseMapManager _instatiatedLevel;

        private void Start()
        {
            _entityFabric.OnPreInitialize();
            _entityFabric.PlayerDestroyed += OnPlayerDestroyed;
            _entityFabric.EnemyDestroyed += OnEnemyDestoyed;
            _entityFabric.EnemiesLeft += OnEnemiesLeft;
            _entityFabric.BaseDestroyed += OnBaseDestroyed;
            if (_currentLevelIndex == -1)
            {
                StartGameFromLevel(_levels[_startLevelIndex]);
            }
        }

        private void OnDisable()
        {
            _entityFabric.PlayerDestroyed -= OnPlayerDestroyed;
            _entityFabric.EnemyDestroyed -= OnEnemyDestoyed;
            _entityFabric.EnemiesLeft -= OnEnemiesLeft;
            _entityFabric.BaseDestroyed -= OnBaseDestroyed;
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
#if DEBUG
                Debug.Log("Todo: Game complete!");
                Debug.Break();
                return;
#elif UNITY_STANDALONE
                Application.Quit(0);
#endif
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
            if (CheckGameOver())
            {
                GameOverLogic();
            }
        }

        private void OnBaseDestroyed(EntityStats stats)
        {
            _entityFabric.BaseDestroyed -= OnBaseDestroyed;
            GameOverLogic();
        }

        private bool AreLivesLeft(int livesLeft) => livesLeft >= 0;

        private bool CheckGameOver()
        {
            switch (GameStaticVariables.GameMode)
            {
                case SelectedGameMode.Mode_1Player:
                    if (!AreLivesLeft(_livesLeft_1Player))
                    {
                        return true;
                    }
                    break;
                case SelectedGameMode.Mode_2Player:
                    if (!AreLivesLeft(_livesLeft_1Player) && !AreLivesLeft(_livesLeft_2Player))
                    {
                        return true;
                    }
                    break;
            }
            return false;
        }

        private void GameOverLogic()
        {
            if (_gameOverLogicExecuted)
            {
                return;
            }
            _gameOverSprite.enabled = true;

#if DEBUG
            Debug.Log("Game over!");
            Debug.Break();
            return;
#elif UNITY_STANDALONE
                Application.Quit(0);
#endif

        }

        private void OnEnemiesLeft()
        {
            Destroy(_instatiatedLevel.gameObject);
            _instatiatedLevel = null;
            LoadNextLevel();
        }
    }
}
