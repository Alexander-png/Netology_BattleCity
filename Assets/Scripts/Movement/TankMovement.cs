using BattleCity.Movement.Base;
using UnityEngine;

namespace BattleCity.Movement
{
    public class TankMovement : HandableMovement
    {
        private void FixedUpdate()
        {
            UpdateDirection();
            MovementLogic();
        }

        protected override void MovementLogic()
        {
            if (!InputEnabled || InputAxis == Vector2.zero)
            {
                _body.velocity = Vector2.zero;
                return;
            }
            _body.velocity = MovementConstraints.GetVectorMovement(MovementConstraints.GetDirectionFromRotation(_body.rotation)) * _speed;
        }
    }
}
