using System;
using JetBrains.Annotations;
using UnityEngine;

namespace Game.Player
{
    public class PlayerAnimation : MonoBehaviour
    {
        [SerializeField] private PlayerState playerState;
        [SerializeField] private Animator animator;
        [SerializeField] private Transform playerSprite;
        [SerializeField] private Transform playerTarget;

        private Vector2 _initialPlayerSpritePos;

        private readonly int _animIsClimbingLedge = Animator.StringToHash("IsClimbingLedge");
        private readonly int _animIsCrouching = Animator.StringToHash("IsCrouching");
        private readonly int _animIsDead = Animator.StringToHash("IsDead");
        private readonly int _animIsGrabbingLedge = Animator.StringToHash("IsGrabbingLedge");
        private readonly int _animIsGrounded = Animator.StringToHash("IsGrounded");
        private readonly int _animIsJumping = Animator.StringToHash("IsJumping");
        private readonly int _animIsRubbingWall = Animator.StringToHash("IsRubbingWall");
        private readonly int _animIsSliding = Animator.StringToHash("IsSliding");
        private readonly int _animLookLeft = Animator.StringToHash("LookLeft");

        private readonly int _animVelX = Animator.StringToHash("VelX");
        private readonly int _animVelY = Animator.StringToHash("VelY");

        private void Start()
        {
            _initialPlayerSpritePos = playerSprite.localPosition;
        }

        private void Update()
        {
            animator.SetFloat(_animVelX, playerState.Rigidbody.linearVelocityX);
            animator.SetFloat(_animVelY, playerState.Rigidbody.linearVelocityY);
            animator.SetBool(_animIsGrounded, playerState.IsGrounded);
            animator.SetBool(_animIsCrouching, playerState.IsCrouching);
            animator.SetBool(_animIsJumping, playerState.IsJumping);
            animator.SetBool(_animIsSliding, playerState.IsSliding);
            animator.SetBool(_animLookLeft, playerState.LookDirection == LookDirection.Left);
            animator.SetBool(_animIsGrabbingLedge, playerState.IsGrabbingLedge);
            animator.SetBool(_animIsRubbingWall, playerState.HasWall);
            animator.SetBool(_animIsClimbingLedge, playerState.IsClimbingLedge);
            animator.SetBool(_animIsDead, playerState.IsDead);
        }

        #region AnimatorCallbacks

        // Called by Animator
        [PublicAPI]
        private void EndLedgeClimbingAnimation()
        {
            playerState.IsClimbingLedge = false;
            ApplyTargetPositionToSprite();
            ApplySpritePosition();
        }

        // Called by Animator
        [PublicAPI]
        private void ApplyTargetPositionToSprite()
        {
            playerSprite.position = playerTarget.position;
        }

        // Called by Animator
        [PublicAPI]
        private void ApplySpritePosition()
        {
            transform.position += playerSprite.localPosition - (Vector3)_initialPlayerSpritePos;
            playerSprite.localPosition = _initialPlayerSpritePos;
        }

        #endregion
    }
}