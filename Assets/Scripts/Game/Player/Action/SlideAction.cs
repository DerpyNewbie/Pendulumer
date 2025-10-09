using UnityEngine;
using UnityEngine.InputSystem;

namespace Game.Player.Action
{
    public class SlideAction : MonoBehaviour
    {
        [SerializeField] private PlayerState playerState;

        [SerializeField] private float slideRequiredSpeed = 6F;
        [SerializeField] private float slideEndingSpeed = .05F;
        [SerializeField] private float slidingSpeedMultiplier = 1.1F;

        private InputAction _crouchAction;
        private bool _doSliding;
        private bool _isAutoSliding;

        private void OnEnable()
        {
            _crouchAction = InputSystem.actions.FindAction("Crouch");

            _crouchAction.performed += OnCrouchPerformed;
            _crouchAction.canceled += OnCrouchCanceled;
        }

        private void OnDisable()
        {
            if (_crouchAction == null) return;

            _crouchAction.performed -= OnCrouchPerformed;
            _crouchAction.canceled -= OnCrouchCanceled;
        }

        private void OnCrouchPerformed(InputAction.CallbackContext ctx)
        {
            _doSliding = true;
        }

        private void OnCrouchCanceled(InputAction.CallbackContext ctx)
        {
            _doSliding = false;
        }

        private void TryBeginSliding()
        {
            if (!CanSlide()) return;

            playerState.Rigidbody.linearVelocityX *= slidingSpeedMultiplier;
            playerState.IsSliding = true;
            _doSliding = false;
        }

        private bool CanSlide()
        {
            return !playerState.Immobile && playerState.IsGrounded && Mathf.Abs(playerState.Rigidbody.linearVelocityX) > slideRequiredSpeed;
        }

        private void Update()
        {
            if (playerState.Immobile) return;

            var groundedInFrame = Time.frameCount - playerState.LastGroundedFrame <= 1;
            var shouldAutoSlide = PlayerConfig.DoAutoSlide && !playerState.IsSliding && groundedInFrame;
            if ((_doSliding || shouldAutoSlide) && !playerState.IsSliding)
            {
                TryBeginSliding();
            }

            if (!playerState.IsSliding) return;

            playerState.IsSliding = !_crouchAction.WasCompletedThisFrame() &&
                                    !playerState.IsCrouching &&
                                    playerState.IsGrounded &&
                                    Mathf.Abs(playerState.Rigidbody.linearVelocityX) >= slideEndingSpeed;
        }
    }
}