using BattleCity.Movement.Base;
using BattleCity.Shooting;
using UnityEngine;
using UnityEngine.InputSystem;

namespace BattleCity.Movement.Input
{
    public class InputHandler : MonoBehaviour
    {
        [SerializeField]
        private HandableMovement _movement;
        [SerializeField]
        private ShootController _shooter;        

        private Vector2 LastInputAxis { get; set; }

        public void OnMove(InputValue input)
        {
            Vector2 newInput = input.Get<Vector2>();
            
            _movement.InputAxis = newInput;
        }

        public void OnShoot(InputValue input)
        {
            if (input.isPressed)
            {
                _shooter.OnShootPerform(_movement.GetDirection());
            }
        }
    }
}
