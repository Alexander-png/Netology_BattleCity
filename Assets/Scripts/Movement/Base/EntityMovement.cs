using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace BattleCity.Movement.Base
{
    public enum Direction : byte
    {
        None = 0,
        Up = 1,
        Down = 2,
        Left = 3,
        Right = 4,
    }

    public static class MovementConstraints
    {
        private static Dictionary<Direction, Vector2> _movements = new Dictionary<Direction, Vector2>()
        {
            { Direction.Up, new Vector2(0f, 1f) },
            { Direction.Down, new Vector2(0f, -1f) },
            { Direction.Left, new Vector2(-1f, 0f) },
            { Direction.Right, new Vector2(1f, 0f) },
        };

        public static Dictionary<Direction, Vector2> Movements
        {
            get 
            {
                Dictionary<Direction, Vector2> toReturn = new Dictionary<Direction, Vector2>(_movements);
                return toReturn;
            }
        }

        private static Dictionary<Direction, float> _rotations = new Dictionary<Direction, float>()
        {
            { Direction.Up, 0 },
            { Direction.Down, 180 },
            { Direction.Left, 90 },
            { Direction.Right, 270 },
        };

        public static float GetRotationAngle(Direction direction) => _rotations[direction];
        public static Direction GetDirectionFromRotation(float rotation) => _rotations.First(r => r.Value == rotation).Key;
        public static Vector2 GetVectorMovement(Direction direction) => _movements[direction];
    }

    [RequireComponent(typeof(BoxCollider2D), typeof(Rigidbody2D))]
    public abstract class EntityMovement : MonoBehaviour
    {
        [SerializeField]
        protected Rigidbody2D _body;

        [Space(15), SerializeField, Range(0f, 25f)]
        protected float _speed;

        protected Direction _direction = Direction.Up;

        public bool InputEnabledMaster { get; set; } = true;

        public Rigidbody2D Body => _body;

        public virtual void SetMoveDirection(Direction newDirection)
        {
            _direction = newDirection;
        }

        public Direction GetDirection() => _direction;

        protected abstract void MovementLogic();
    }

    public abstract class HandableMovement : EntityMovement
    {
        public Vector2 InputAxis { get; set; }

        protected virtual void UpdateDirection()
        { 
            if (InputAxis == Vector2.zero)
            {
                return;
            }

            Vector2 velocity = InputAxis;
            switch (velocity.x)
            {
                case > 0f when velocity.y == 0f:
                    _direction = Direction.Right;
                    break;
                case < 0f when velocity.y == 0f:
                    _direction = Direction.Left;
                    break;
                case 0f when velocity.y > 0f:
                    _direction = Direction.Up;
                    break;
                case 0f when velocity.y < 0f:
                    _direction = Direction.Down;
                    break;
            }
            _body.rotation = MovementConstraints.GetRotationAngle(_direction);
        }
    }
}

// TODO:
// UI (lives, score, etc)
// bonuses
// improve handling