using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Game.Player.Action
{
    public class WallJumpAction : MonoBehaviour
    {
        [SerializeField] private PlayerState playerState;
        [SerializeField] private Vector2 wallJumpVelocity = new Vector2(8, 8);

        private InputAction _jumpAction;

        private void OnEnable()
        {
            _jumpAction = InputSystem.actions.FindAction("Jump");
            _jumpAction.performed += OnJumpPerformed;
        }

        private void OnDisable()
        {
            if (_jumpAction == null) return;

            _jumpAction.performed -= OnJumpPerformed;
        }

        private void OnJumpPerformed(InputAction.CallbackContext obj)
        {
            if (playerState.Immobile || playerState.IsJumping || !playerState.HasWall || playerState.CanClimb)
                return;

            playerState.Rigidbody.linearVelocity = new Vector2(
                wallJumpVelocity.x * (playerState.LookDirection == LookDirection.Right ? -1 : 1),
                wallJumpVelocity.y
            );

            playerState.IsJumping = true;
        }
    }
}