using System;
using UI;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

namespace Game
{
    public class HookShotAction : MonoBehaviour
    {
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

        private Action<InputAction.CallbackContext> _activateAction;
        private Action<InputAction.CallbackContext> _aimPreviewToggleAction;
        private InputAction _attackAction;
        private bool _controllable;
        private Action<InputAction.CallbackContext> _deactivateAction;
        private bool _hasCrosshairHit;
        private bool _isHookShotActive;

        private Camera _mainCamera;
        private InputAction _previewToggleAction;

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


        public bool IsPreviewVisible { get; private set; } = true;

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
            aimPreviewRenderer.colorGradient = inactiveGradient;
            _activateAction = _ =>
            {
                if (!Controllable) return;

                if ((!PlayerConfig.HookShot.ToggleShot || !_isHookShotActive) && HasCrosshairHit) Activate();
                else Deactivate();
            };

            _deactivateAction = _ =>
            {
                if (Controllable && !PlayerConfig.HookShot.ToggleShot) Deactivate();
            };

            _aimPreviewToggleAction = _ => { IsPreviewVisible = !IsPreviewVisible; };

            _attackAction = InputSystem.actions.FindAction("Attack");
            _previewToggleAction = InputSystem.actions.FindAction("Sprint");


            _attackAction.performed += _activateAction;
            _attackAction.canceled += _deactivateAction;
            _previewToggleAction.performed += _aimPreviewToggleAction;
        }

        // Update is called once per frame
        private void Update()
        {
            if (!Controllable) return;

            UpdateCrosshairPosition();
            UpdateVisuals();
        }

        private void OnEnable()
        {
            targetRigidbody.simulated = false;
            UpdateVisuals();
        }

        private void OnDestroy()
        {
            _attackAction.performed -= _activateAction;
            _attackAction.canceled -= _deactivateAction;
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

            var mousePoint = Mouse.current.position.ReadValue();
            var worldPoint =
                _mainCamera.ScreenToWorldPoint(new Vector3(mousePoint.x, mousePoint.y, _mainCamera.nearClipPlane));

            actualCrosshair.position = worldPoint;

            var hit = Physics2D.Raycast(
                playerAimReference.position,
                worldPoint - playerAimReference.position,
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
            Debug.Log("[HookShotAction] Activate");
            targetRigidbody.transform.position = physicalCrosshair.transform.position;
            targetRigidbody.MovePosition(physicalCrosshair.transform.position);
            targetRigidbody.simulated = true;
            _isHookShotActive = true;
            UpdateVisuals();
            OnActivated?.Invoke();
        }

        private void Deactivate()
        {
            Debug.Log("[HookShotAction] Deactivate");
            var hasStateChange = _isHookShotActive;
            targetRigidbody.simulated = false;
            _isHookShotActive = false;
            UpdateVisuals();

            if (hasStateChange) OnDeactivated?.Invoke();
        }
    }
}