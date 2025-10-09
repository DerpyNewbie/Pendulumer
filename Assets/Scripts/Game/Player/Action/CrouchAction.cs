using UnityEngine;
using UnityEngine.InputSystem;

namespace Game.Player.Action
{
    public class CrouchAction : MonoBehaviour
    {
        [SerializeField] private PlayerState playerState;
        [SerializeField] private float crouchableMaxSpeed = 5F;

        private InputAction _crouchAction;
        private bool _doCrouch;

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
            _doCrouch = true;
        }

        private void OnCrouchCanceled(InputAction.CallbackContext ctx)
        {
            _doCrouch = false;
        }

        private void Update()
        {
            if (playerState.Immobile) return;

            var isStationary = playerState.Rigidbody.linearVelocity.sqrMagnitude <= crouchableMaxSpeed;
            playerState.IsCrouching = _doCrouch && isStationary;
        }
    }
}