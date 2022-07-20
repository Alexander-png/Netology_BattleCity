using BattleCity.Animations;
using System.Linq;
using UnityEngine;

namespace BattleCity.Managers.Game
{
    public enum LiteAnimationTypes : byte
    {
        ProjectileExplosion = 0,
        TankExplosion = 1,
        TankProtection = 2,
        TankSpawn = 3,

    }

    public class LiteAnimationCollection : MonoBehaviour 
    {
        [SerializeField]
        private LiteAnimation[] _animations;

        public static LiteAnimationCollection CurrentInstance { get; private set; }

        private void Awake()
        {
            CurrentInstance = this;
        }

        public void PlayAnimation(Vector2 coords, LiteAnimationTypes targetType)
        {
            Instantiate(_animations.First(a => a.Type == targetType)).transform.position = new Vector3(coords.x, coords.y);
        }

        public LiteAnimation GetAnimation(LiteAnimationTypes queryType)
        {
            return _animations.First(a => a.Type == queryType);
        }
    }
}
