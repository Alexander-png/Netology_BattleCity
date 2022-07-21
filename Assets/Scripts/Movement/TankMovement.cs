using BattleCity.Movement.Base;
using UnityEngine;

namespace BattleCity.Movement
{
    public class TankMovement : HandableMovement
    {
        public bool InputEnabled { get; set; } = true;

        private void FixedUpdate()
        {
            UpdateDirection();
            MovementLogic();
        }

        protected override void MovementLogic()
        {
            if (InputEnabledMaster && InputEnabled && InputAxis != Vector2.zero)
            {
                _body.velocity = MovementConstraints.GetVectorMovement(MovementConstraints.GetDirectionFromRotation(_body.rotation)) * _speed;
            }
            else
            {
                _body.velocity = Vector2.zero;
            }
        }
    }
}
