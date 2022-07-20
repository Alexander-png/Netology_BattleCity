using BattleCity.Movement.Base;
using BattleCity.Projectiles;
using BattleCity.Stats;
using System.Collections;
using UnityEngine;

namespace BattleCity.Shooting
{
    public class ShootController : MonoBehaviour
    {
        [SerializeField]
        private Transform _projectileSpawn;
        [SerializeField]
        private ProjectileBehaviour _projectile;
        [SerializeField]
        private float _reloadTime = 0.3f;

        private bool _canShoot = true;
        private EntityStats _tankStats;

        public bool InputEnabled { get; set; } = true;

        private void Start()
        {
            _tankStats = GetComponent<EntityStats>();
        }

        public void OnShootPerform(Direction dir)
        {
            if (_canShoot && InputEnabled)
            {
                _canShoot = false;
                ProjectileBehaviour bullet = Instantiate(_projectile.gameObject, _projectileSpawn.position, Quaternion.identity).GetComponent<ProjectileBehaviour>();
                bullet.SetMoveDirection(dir);
                bullet.Side = _tankStats.Side;
                StartCoroutine(ReloadCoroutine());
            }
        }

        private IEnumerator ReloadCoroutine()
        {
            yield return new WaitForSeconds(_reloadTime);
            _canShoot = true;
        }
    }
}
