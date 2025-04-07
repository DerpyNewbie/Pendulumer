using System;
using UI;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

namespace Game
{
    public class HookShotAction : MonoBehaviour
    {
        [SerializeField] private PlayerInput playerInput;
        [SerializeField] private Rigidbody2D targetRigidbody;
        [SerializeField] private LineRenderer lineRenderer;
        [SerializeField] private LineRenderer aimPreviewRenderer;

        [FormerlySerializedAs("crosshair")] [SerializeField]
        private Transform physicalCrosshair;

        [SerializeField] private Transform actualCrosshair;

        [FormerlySerializedAs("actualSpriteRendererColor")]
        [FormerlySerializedAs("actualCrosshairColor")]
        [FormerlySerializedAs("crosshairColor")] [SerializeField]
        private SpriteRendererColorChanger actualSpriteRendererColorChanger;

        [SerializeField] private Color farColor = Color.red;
        [SerializeField] private Color nearColor = Color.gray;
        [SerializeField] private Transform playerAimReference;
        [SerializeField] private Transform hookShotHitReference;
        [SerializeField] private Transform hookShotBeginReference;
        [SerializeField] private LayerMask layerMask;
        [SerializeField] private Gradient activeGradient;
        [SerializeField] private Gradient inactiveGradient;

        private InputAction _lookAction;
        private InputAction _attackAction;

        private bool _controllable;
        private bool _hasCrosshairHit;
        private bool _isHookShotActive;
        private bool _hasHookShotFixed;

        private Vector2 _lastLookInput;

        private Camera _mainCamera;
        private DistanceJoint2D _distanceJoint;
        private SpriteRenderer _jointRenderer;


        public bool Controllable
        {
            get => _controllable;
            set
            {
                aimPreviewRenderer.enabled = value;
                actualCrosshair.gameObject.SetActive(value);
                physicalCrosshair.gameObject.SetActive(value);
                if (_controllable != value)
                {
                    _controllable = value;
                    if (value)
                    {
                        OnControlActivated?.Invoke();
                    }
                    else
                    {
                        Deactivate();
                        OnControlDeactivated?.Invoke();
                    }
                }
            }
        }


        public bool IsPreviewVisible { get; } = true;

        public bool HasCrosshairHit
        {
            get => _hasCrosshairHit;
            private set
            {
                if (_hasCrosshairHit == value) return;
                _hasCrosshairHit = value;
                OnCrosshairHitChanged?.Invoke();
            }
        }


        // Start is called once before the first execution of Update after the MonoBehaviour is created
        private void Awake()
        {
            _mainCamera = Camera.main;
            _distanceJoint = targetRigidbody.GetComponent<DistanceJoint2D>();
            _jointRenderer = targetRigidbody.GetComponent<SpriteRenderer>();

            aimPreviewRenderer.colorGradient = inactiveGradient;

            _lookAction = playerInput.actions.FindAction("Look");
            _attackAction = playerInput.actions.FindAction("Attack");
            _attackAction.performed += OnAttackAction;
            _attackAction.canceled += OnAttackAction;
        }

        // Update is called once per frame
        private void Update()
        {
            if (!Controllable) return;

            UpdateCrosshairPosition();
            UpdateVisuals();
            UpdateJoint();
        }

        private void UpdateJoint()
        {
            if (_hasHookShotFixed) return;

            if (_distanceJoint.connectedBody.linearVelocityY > 0) return;

            _distanceJoint.distance =
                Vector2.Distance(_distanceJoint.transform.position, playerAimReference.position) + 0.5F;
            Debug.Log($"Hookshot has been fixed to {_distanceJoint.distance}m");
            _hasHookShotFixed = true;
        }

        private void OnEnable()
        {
            targetRigidbody.simulated = false;
            UpdateVisuals();
        }

        private void OnDestroy()
        {
            _attackAction.performed -= OnAttackAction;
            _attackAction.canceled -= OnAttackAction;
        }

        public event Action OnActivated;
        public event Action OnDeactivated;
        public event Action OnCrosshairHitChanged;
        public event Action OnControlActivated;
        public event Action OnControlDeactivated;

        private void UpdateCrosshairPosition()
        {
            bool IsInsideScreen(Vector3 vec)
            {
                return _mainCamera.pixelHeight > vec.y && 0 < vec.y &&
                       _mainCamera.pixelWidth > vec.x && 0 < vec.x;
            }

            if (playerInput.currentControlScheme == "Gamepad")
            {
                var viewportAimRef = _mainCamera.WorldToViewportPoint(playerAimReference.position);
                var look = _lookAction.ReadValue<Vector2>();

                if (look.magnitude < 0.5f)
                {
                    look = _lastLookInput;
                }
                else
                {
                    _lastLookInput = look;
                }

                viewportAimRef += (Vector3)look;
                actualCrosshair.position = _mainCamera.ViewportToWorldPoint(new Vector3(
                    Mathf.Clamp01(viewportAimRef.x),
                    Mathf.Clamp01(viewportAimRef.y),
                    Mathf.Clamp01(viewportAimRef.z))
                ) + (Vector3)look;
            }
            else
            {
                var mousePoint = Mouse.current.position.ReadValue();
                actualCrosshair.position =
                    _mainCamera.ScreenToWorldPoint(new Vector3(mousePoint.x, mousePoint.y, _mainCamera.nearClipPlane));
            }


            var hit = Physics2D.Raycast(
                playerAimReference.position,
                actualCrosshair.position - playerAimReference.position,
                100F,
                layerMask
            );

            HasCrosshairHit = hit.collider != null && IsInsideScreen(_mainCamera.WorldToScreenPoint(hit.point));
            if (HasCrosshairHit) physicalCrosshair.transform.position = hit.point;
        }

        private void UpdateVisuals()
        {
            aimPreviewRenderer.colorGradient = HasCrosshairHit ? activeGradient : inactiveGradient;
            aimPreviewRenderer.SetPosition(0, (Vector2)hookShotBeginReference.position);
            aimPreviewRenderer.SetPosition(1,
                (Vector2)(HasCrosshairHit ? physicalCrosshair.position : actualCrosshair.position));

            if (_isHookShotActive)
            {
                lineRenderer.SetPosition(0, hookShotBeginReference.transform.position);
                lineRenderer.SetPosition(1, hookShotHitReference.transform.position);
            }

            _jointRenderer.enabled = _isHookShotActive;

            var visibleAndControllable = Controllable && IsPreviewVisible;

            lineRenderer.gameObject.SetActive(Controllable && _isHookShotActive);
            aimPreviewRenderer.gameObject.SetActive(visibleAndControllable);
            physicalCrosshair.gameObject.SetActive(visibleAndControllable && HasCrosshairHit);

            var actualFurther = !HasCrosshairHit ||
                                Vector2.Distance(hookShotBeginReference.position, physicalCrosshair.position) <
                                Vector2.Distance(hookShotBeginReference.position, actualCrosshair.position);

            actualCrosshair.gameObject.SetActive(visibleAndControllable);
            actualSpriteRendererColorChanger.SetColor(actualFurther ? farColor : nearColor);
        }

        private void Activate()
        {
            // Debug.Log("[HookShotAction] Activate");
            targetRigidbody.transform.position = physicalCrosshair.transform.position;
            targetRigidbody.MovePosition(physicalCrosshair.transform.position);
            targetRigidbody.simulated = true;
            _isHookShotActive = true;
            _hasHookShotFixed = false;
            UpdateVisuals();
            OnActivated?.Invoke();
        }

        private void Deactivate()
        {
            // Debug.Log("[HookShotAction] Deactivate");
            var hasStateChange = _isHookShotActive;
            targetRigidbody.simulated = false;
            _isHookShotActive = false;
            UpdateVisuals();

            if (hasStateChange) OnDeactivated?.Invoke();
        }

        private void OnAttackAction(InputAction.CallbackContext ctx)
        {
            if (ctx.performed)
            {
                if (!Controllable) return;

                if ((!PlayerConfig.ToggleHookShot || !_isHookShotActive) && HasCrosshairHit) Activate();
                else Deactivate();

                return;
            }

            if (ctx.canceled)
                if (Controllable && !PlayerConfig.ToggleHookShot)
                    Deactivate();
        }
    }
}