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
        private GameObject crosshair;

        [SerializeField]
        private Transform aimOrigin;

        [SerializeField]
        private LayerMask obstacleLayer;

        private Camera _mainCamera;
        private InputAction _fireAction;
        private float _shortestHingeDistance;

        public bool IsHookShotActive { get; private set; }
        public bool HasCrosshairHit { get; private set; }
        public Vector3 HitPosition { get; private set; }

        private void OnEnable()
        {
            _mainCamera = Camera.main;
            _fireAction = InputSystem.actions.FindAction("Jump");
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
            bool IsInsideScreen(Vector3 vec)
            {
                return _mainCamera.pixelHeight > vec.y && 0 < vec.y &&
                       _mainCamera.pixelWidth > vec.x && 0 < vec.x;
            }

            var aimDir = playerState.LookDirection == LookDirection.Left ? new Vector2(-1, 1) :new Vector2(1, 1);

            var hit = Physics2D.Raycast(
                aimOrigin.position,
                aimDir,
                100F,
                obstacleLayer
            );

            HasCrosshairHit = hit.collider != null && IsInsideScreen(_mainCamera.WorldToScreenPoint(hit.point));
            HitPosition = hit.point;

            crosshair.SetActive( HasCrosshairHit);
            crosshair.transform.position = HitPosition;

            if (IsHookShotActive)
            {
                var currentDistance = Vector2.Distance(distanceJoint.transform.position, distanceJoint.attachedRigidbody.position); 
                if (currentDistance < distanceJoint.distance)
                    distanceJoint.distance = currentDistance;
            }
        }

        private void OnFireActionPerformed(InputAction.CallbackContext obj)
        {
            if ((PlayerConfig.ToggleHookShot || !IsHookShotActive) && !playerState.IsGrounded && HasCrosshairHit)
            {
                AttachHinge(HitPosition);
            }
            else
            {
                DetachHinge();
            }
        }

        private void OnFireActionCanceled(InputAction.CallbackContext obj)
        {
            if (PlayerConfig.ToggleHookShot) return;

            DetachHinge();
        }

        private void AttachHinge(Vector3 worldPos)
        {
            distanceJoint.distance = 10000;
            _shortestHingeDistance = 10000;

            hingeTarget.transform.position = worldPos;
            hingeTarget.MovePosition(worldPos);
            hingeTarget.position = worldPos;
            hingeTarget.simulated = true;

            IsHookShotActive = true;
        }

        private void DetachHinge()
        {
            hingeTarget.simulated = false;

            IsHookShotActive = false;
        }
    }
}