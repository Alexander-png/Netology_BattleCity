using BattleCity.Movement.Base;
using BattleCity.Shooting;
using BattleCity.Stats;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BattleCity.AI
{
    public class EnemyAIBehaviour : MonoBehaviour
    {
        [SerializeField]
        private HandableMovement _movement;
        [SerializeField]
        private ShootController _shooter;
        [SerializeField]
        private EnemyStats _stats;

        [SerializeField, Range(0f, 10f)]
        private float _turnDelay = 5f;
        [SerializeField, Range(0f, 10f)]
        private float _wakeUpTime = 4f;
        [SerializeField, Range(0f, 10f)]
        private float _viewRange = 10;
        [SerializeField, Range(0f, 1f)]
        private float _shootFrequency = 0.5f;

        private bool _isWakedUp = false;
        public EnemyStats Stats => _stats;

        private Dictionary<Direction, Vector2> _moveVectors;

        private IEnumerator WakeUpCoroutine()
        {
            yield return new WaitForSeconds(_wakeUpTime);
            _isWakedUp = true;
        }

        private IEnumerator RandomChangeDirectionCoroutine()
        {
            while (true)
            {
                yield return new WaitForSeconds(_turnDelay);
                _movement.InputAxis = _moveVectors[(Direction)Convert.ToByte(UnityEngine.Random.Range(1, 5))];
            }            
        }

        private void Start()
        {
            _moveVectors = MovementConstraints.Movements;
            _movement.InputAxis = _moveVectors[(Direction)Convert.ToByte(UnityEngine.Random.Range(1, 5))];
            StartCoroutine(WakeUpCoroutine());
            StartCoroutine(RandomChangeDirectionCoroutine());
        }

        private void FixedUpdate()
        {
            ProcessBehaviour();
        }

        private void ProcessBehaviour()
        {
            if (_isWakedUp)
            {
                for (int i = 0; i < _moveVectors.Count; i++)
                {
                    Direction currentDirection = (Direction)Convert.ToByte(i + 1);
                    RaycastHit2D[] hit = Physics2D.RaycastAll(transform.position, _moveVectors[currentDirection], _viewRange);
                    for (int j = 0; j < hit.Length; j++)
                    {
                        if (hit[j].collider != null)
                        {
                            if (hit[j].collider.gameObject.TryGetComponent(out EntityStats stats))
                            {
                                if (_stats.Side != stats.Side)
                                {
                                    _movement.InputAxis = _moveVectors[currentDirection];
                                }
                            }
                        }
                    }
                }
                
            }
            if (UnityEngine.Random.Range(0f, 1f) <= _shootFrequency)
            {
                _shooter.OnShootPerform(_movement.GetDirection());
            }
        }
    }
}
