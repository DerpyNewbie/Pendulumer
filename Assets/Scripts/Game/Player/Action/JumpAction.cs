using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Game.Player.Action
{
    public class JumpAction : MonoBehaviour
    {
        [SerializeField] private PlayerState playerState;

        [SerializeField] private float jumpForce = 7F;

        [SerializeField] private float jumpTime = 0.25F;

        [SerializeField] private float coyoteTime = 0.5F;

        private InputAction _jumpAction;
        private float _jumpTimer;
        private bool _doJump;

        private void OnEnable()
        {
            _jumpAction = InputSystem.actions.FindAction("Jump");
            _jumpAction.performed += OnJumpPerformed;
            _jumpAction.canceled += OnJumpCanceled;
        }

        private void OnDisable()
        {
            if (_jumpAction == null) return;

            _jumpAction.performed -= OnJumpPerformed;
            _jumpAction.canceled -= OnJumpCanceled;
        }

        private void OnJumpPerformed(InputAction.CallbackContext ctx)
        {
            if (playerState.LastGroundedTime + coyoteTime < Time.time)
                return;

            playerState.IsJumping = true;
        }

        private void OnJumpCanceled(InputAction.CallbackContext ctx)
        {
            playerState.IsJumping = false;
            playerState.IsJumpingCanceled = true;
        }

        private void Update()
        {
            if (playerState.Immobile) return;

            _jumpTimer += Time.deltaTime;

            if (playerState.IsGrounded)
            {
                _jumpTimer = 0;
                playerState.IsJumpingCanceled = false;
            }

            if (playerState.IsJumping && !playerState.IsJumpingCanceled)
                ApplyVelocity();
        }

        private void ApplyVelocity()
        {
            if (_jumpTimer >= jumpTime) return;

            playerState.Rigidbody.linearVelocityY = jumpForce;
        }
    }
}