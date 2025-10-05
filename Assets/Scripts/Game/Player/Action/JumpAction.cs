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
            playerState.IsJumping = true;
        }

        private void OnJumpCanceled(InputAction.CallbackContext ctx)
        {
            playerState.IsJumping = false;
            playerState.IsJumpingCanceled = true;
        }

        private void Update()
        {
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