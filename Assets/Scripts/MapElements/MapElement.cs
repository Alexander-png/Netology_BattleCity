using BattleCity.Projectiles;
using UnityEngine;

namespace BattleCity.Map.Base
{
    [RequireComponent(typeof(BoxCollider2D), typeof(Rigidbody2D), typeof(SpriteRenderer))]
    public class MapElement : MonoBehaviour
    {
        [SerializeField]
        private bool _destoryable = false;

        public bool Destoryable => _destoryable;

        public void OnProjectileCollision(ProjectileBehaviour projectile)
        {
            if (!Destoryable)
            {
                return;
            }
            Destroy(gameObject);
        }
    }
}
