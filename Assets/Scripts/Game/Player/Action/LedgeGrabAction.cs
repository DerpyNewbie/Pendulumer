using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Game.Player.Action
{
    public class LedgeGrabAction : MonoBehaviour
    {
        [SerializeField]
        private PlayerState playerState;

        private InputAction _moveAction;
        private InputAction _jumpAction;

        private void Start()
        {
            _moveAction = InputSystem.actions.FindAction("Move");
            _jumpAction = InputSystem.actions.FindAction("Jump");
        }

        private void Update()
        {
            if (playerState.Immobile) return;

            if (playerState.CanClimb && !playerState.IsCrouching)
            {
                playerState.Rigidbody.linearVelocityY = 0;
                playerState.Rigidbody.gravityScale = 0;
                playerState.IsGrabbingLedge = true;

                var move = _moveAction.ReadValue<Vector2>();
                var doClimb = move.x switch
                {
                    > 0.01F => playerState.LookDirection == LookDirection.Right,
                    < -0.01F => playerState.LookDirection == LookDirection.Left,
                    _ => false
                };

                if (doClimb)
                {
                    playerState.IsClimbingLedge = true;
                }
            }
            else
            {
                playerState.Rigidbody.gravityScale = 1;
                playerState.IsGrabbingLedge = false;
                playerState.IsClimbingLedge = false;
            }
        }
    }
}