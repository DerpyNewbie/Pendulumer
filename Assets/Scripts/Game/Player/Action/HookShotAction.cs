using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Game.Player.Action
{
    public class HookShotAction : MonoBehaviour
    {
        [SerializeField]
        private PlayerState playerState;

        [SerializeField]
        private Rigidbody2D hingeTarget;

        [SerializeField]
        private DistanceJoint2D distanceJoint;

        [SerializeField]
        private LineRenderer ropeRenderer;

        [SerializeField]
        private GameObject mousePointer;

        [SerializeField]
        private GameObject crosshair;

        [SerializeField]
        private Transform aimOrigin;

        [SerializeField]
        private LayerMask obstacleLayer;

        [SerializeField]
        private AimControlType aimControlType;

        [SerializeField]
        private float distanceShrinkThreshold = 2;


        private Camera _mainCamera;
        private InputAction _fireAction;

        public bool Controllable { get; set; }
        public bool IsHookShotActive { get; private set; }
        public bool HasCrosshairHit { get; private set; }
        public Vector3 HitPosition { get; private set; }

        public event System.Action OnHookShotActivated;
        public event System.Action OnHookShotDeactivated;

        private void OnEnable()
        {
            _mainCamera = Camera.main;
            _fireAction = InputSystem.actions.FindAction("Attack");
            _fireAction.performed += OnFireActionPerformed;
            _fireAction.canceled += OnFireActionCanceled;
        }

        private void OnDisable()
        {
            _fireAction.performed -= OnFireActionPerformed;
            _fireAction.canceled -= OnFireActionCanceled;
        }

        private void Update()
        {
            if (playerState.Immobile) return;

            bool IsInsideScreen(Vector3 vec)
            {
                return _mainCamera.pixelHeight > vec.y && 0 < vec.y &&
                       _mainCamera.pixelWidth > vec.x && 0 < vec.x;
            }

            var aimDir = GetAimingDirection(aimControlType);

            var hit = Physics2D.Raycast(
                aimOrigin.position,
                aimDir,
                100F,
                obstacleLayer
            );

            HasCrosshairHit = hit.collider != null && IsInsideScreen(_mainCamera.WorldToScreenPoint(hit.point));
            HitPosition = hit.point;

            crosshair.transform.position = HitPosition;
            mousePointer.transform.position = _mainCamera.ScreenToWorldPoint(Mouse.current.position.ReadValue()) +
                                              Vector3.forward * 10;

            crosshair.SetActive(HasCrosshairHit);

            {
                var crosshairDistance = Vector2.Distance(crosshair.transform.position, aimOrigin.position);
                var mousePointerDistance = Vector2.Distance(mousePointer.transform.position, aimOrigin.position);

                mousePointer.SetActive(Controllable &&
                                       aimControlType == AimControlType.Mouse &&
                                       (mousePointerDistance > crosshairDistance || !HasCrosshairHit));
            }

            if (IsHookShotActive)
            {
                var currentDistance =
                    Vector2.Distance(distanceJoint.transform.position, playerState.Rigidbody.position);
                if (currentDistance + distanceShrinkThreshold < distanceJoint.distance)
                    distanceJoint.distance = currentDistance + distanceShrinkThreshold;
            }
        }

        private void OnFireActionPerformed(InputAction.CallbackContext obj)
        {
            if (!Controllable) return;

            if ((PlayerConfig.ToggleHookShot || !IsHookShotActive) && HasCrosshairHit)
            {
                if (playerState.HasWall && !playerState.CanClimb)
                    return;

                AttachHinge(HitPosition);
            }
            else
            {
                DetachHinge();
            }
        }

        private void OnFireActionCanceled(InputAction.CallbackContext obj)
        {
            if (!Controllable || PlayerConfig.ToggleHookShot) return;

            DetachHinge();
        }

        private void AttachHinge(Vector3 worldPos)
        {
            distanceJoint.distance = Vector2.Distance(worldPos, playerState.Rigidbody.transform.position);

            hingeTarget.transform.position = worldPos;
            hingeTarget.MovePosition(worldPos);
            hingeTarget.position = worldPos;
            hingeTarget.simulated = true;

            IsHookShotActive = true;
            OnHookShotActivated?.Invoke();
        }

        private void DetachHinge()
        {
            var wasActive = IsHookShotActive;

            hingeTarget.simulated = false;
            IsHookShotActive = false;

            if (wasActive) OnHookShotDeactivated?.Invoke();
        }

        private Vector3 GetAimingDirection(AimControlType controlType)
        {
            return controlType switch
            {
                AimControlType.Mouse => _mainCamera.ScreenToWorldPoint(Mouse.current.position.ReadValue()) -
                                        aimOrigin.position,
                AimControlType.Keyboard => playerState.LookDirection == LookDirection.Left
                    ? new Vector2(-1, 1)
                    : new Vector2(1, 1),
                _ => Vector3.forward
            };
        }
    }

    public enum AimControlType
    {
        Mouse,
        Keyboard
    }
}