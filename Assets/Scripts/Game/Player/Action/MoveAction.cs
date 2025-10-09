using UnityEngine;
using UnityEngine.InputSystem;

namespace Game.Player.Action
{
    public class MoveAction : MonoBehaviour
    {
        [SerializeField] private PlayerState playerState;

        [SerializeField] private float smoothTime = 0.1F;
        [SerializeField] private float speed = 7;
        [SerializeField] private float maxSpeed = 10;
        [SerializeField] private float airSpeedMultiplier = 2F;

        private float _currentXVelocity;
        private InputAction _moveAction;

        private void Start()
        {
            _moveAction = InputSystem.actions.FindAction("Move");
        }

        private void Update()
        {
            if (playerState.Immobile || playerState.IsCrouching || playerState.IsSliding)
            {
                return;
            }

            var move = _moveAction.ReadValue<Vector2>();
            playerState.LookDirection = move.x switch
            {
                > 0.01F => LookDirection.Right,
                < -0.01F => LookDirection.Left,
                _ => playerState.LookDirection
            };

            if (playerState.HasWall)
            {
                return;
            }

            if (!playerState.IsGrounded)
            {
                playerState.Rigidbody.linearVelocityX += move.x * airSpeedMultiplier * Time.deltaTime;
                return;
            }

            var velX = playerState.Rigidbody.linearVelocityX;
            var nextVelX = Mathf.SmoothDamp(velX, move.x * speed, ref _currentXVelocity, smoothTime);
            playerState.Rigidbody.linearVelocityX = nextVelX;
        }
    }
}