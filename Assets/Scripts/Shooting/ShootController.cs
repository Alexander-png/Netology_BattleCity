using BattleCity.Managers.Game;
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
        private SoundCollection _sounds;

        public bool InputEnabled { get; set; } = true;

        private void Start()
        {
            _tankStats = GetComponent<EntityStats>();
            _sounds = SoundCollection.CurrentInstance;
        }

        public void OnShootPerform(Direction dir)
        {
            if (_canShoot && InputEnabled)
            {
                _canShoot = false;
                ProjectileBehaviour bullet = Instantiate(_projectile.gameObject, _projectileSpawn.position, Quaternion.identity).GetComponent<ProjectileBehaviour>();
                bullet.SenderName = gameObject.name;
                bullet.SetMoveDirection(dir);
                bullet.Side = _tankStats.Side;
                PlayShootSound();
                StartCoroutine(ReloadCoroutine());
            }
        }

        private void PlayShootSound()
        {
            if (_tankStats.IsPlayer)
            {
                AudioSource.PlayClipAtPoint(_sounds.GetSound(SoundTypes.PlayerShoot), transform.position);
            }
        }

        private IEnumerator ReloadCoroutine()
        {
            yield return new WaitForSeconds(_reloadTime);
            _canShoot = true;
        }
    }
}
