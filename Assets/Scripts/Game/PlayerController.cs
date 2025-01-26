using System;
using JetBrains.Annotations;

#if UNITY_EDITOR
using UnityEditor;
#endif

using UnityEngine;
using UnityEngine.InputSystem;

namespace Game
{
    public class PlayerController : MonoBehaviour
    {
        public enum EventContext
        {
            Begin,
            End
        }

        public enum DirectionalState
        {
            None = 0,
            Left = 1,
            Right = 2
        }

        public struct PlayerState
        {
            public DirectionalState Moving;
            public DirectionalState LedgeGrabbing;
            public bool IsDead;
            public bool IsJumping;
            public bool IsSliding;
            public bool IsGrounded;
            public bool IsCrouching;
            public bool IsClimbingLedge;
            public bool IsRubbingWall;

            public bool IsGrabbingLedge => LedgeGrabbing != DirectionalState.None;
            public bool IsMoving => Moving != DirectionalState.None;
        }

        public struct WallInfo
        {
            public bool HasWall;
            public bool HasSpaceToClimb;
        }

        [Header("Base")]
        [SerializeField] private GameManager gameManager;

        [SerializeField] private Animator animator;
        [SerializeField] private Transform playerSprite;
        [SerializeField] private Transform playerTarget;
        [SerializeField] private float speed = 7;
        [SerializeField] private float maxSpeed = 10;
        [SerializeField] private float airSpeedMultiplier = 2F;
        [SerializeField] private float slidingSpeedMultiplier = 1.1F;
        [SerializeField] private float smoothTime = 0.1F;
        [SerializeField] private LayerMask obstacleLayer = int.MaxValue;


        [Header("Jump")]
        [SerializeField] private float jumpForce = 5F;

        [SerializeField] private float jumpTime = 0.25F;

        [Header("Wall Jump")]
        [SerializeField] private Vector2 wallJumpVelocity = new(2, 5);

        [Header("Ground Check")]
        [SerializeField] private Vector2 groundCheckOffset = new(0, -1F);

        [SerializeField] private Vector2 groundCheckSize = new(0.2F, 0.2F);

        [Header("Wall Check")]
        [SerializeField] private Vector2 wallCheckOffset = new(0.5F, 0F);

        [SerializeField] private Vector2 wallCheckSize = new(0.05F, 1.95F);

        [Header("Ledge Check")]
        [SerializeField] private Vector2 ledgeCheckOffset = new(0.5F, 1F);

        [SerializeField] private Vector2 ledgeCheckSize = new(0.5F, 0.1F);

        private Rigidbody2D _rb;
        private Vector2 _initialPlayerSpritePos;
        private float _currentXVelocity;
        private float _jumpTimer;

        private PlayerState _playerState;
        private WallInfo _leftWall;
        private WallInfo _rightWall;

        private readonly int _animVelX = Animator.StringToHash("VelX");
        private readonly int _animVelY = Animator.StringToHash("VelY");
        private readonly int _animIsGrounded = Animator.StringToHash("IsGrounded");
        private readonly int _animIsCrouching = Animator.StringToHash("IsCrouching");
        private readonly int _animIsJumping = Animator.StringToHash("IsJumping");
        private readonly int _animIsGrabbingLedge = Animator.StringToHash("IsGrabbingLedge");
        private readonly int _animIsClimbingLedge = Animator.StringToHash("IsClimbingLedge");
        private readonly int _animLookLeft = Animator.StringToHash("LookLeft");
        private readonly int _animIsDead = Animator.StringToHash("IsDead");
        private readonly int _animIsSliding = Animator.StringToHash("IsSliding");
        private readonly int _animIsRubbingWall = Animator.StringToHash("IsRubbingWall");

        private InputAction _moveAction;
        private InputAction _jumpAction;
        private InputAction _crouchAction;

        public bool Immobile { get; set; }
        public PlayerState State => _playerState;

        public event Action<float> OnMove;
        public event Action<EventContext> OnJump;
        public event Action<EventContext> OnCrouch;
        public event Action<EventContext> OnSlide;
        public event Action<EventContext> OnWallSlide;
        public event Action OnLanding;
        public event Action OnWallJumping;


        private void Awake()
        {
            _rb = GetComponent<Rigidbody2D>();
            _initialPlayerSpritePos = playerSprite.localPosition;

            _moveAction = InputSystem.actions.FindAction("Move");
            _jumpAction = InputSystem.actions.FindAction("Jump");
            _crouchAction = InputSystem.actions.FindAction("Crouch");

            OnJump += (v) => { Debug.Log($"[PlayerController] OnJump: {v}"); };
            OnLanding += () => { Debug.Log("[PlayerController] OnLanding"); };
            OnCrouch += (v) => { Debug.Log($"[PlayerController] OnCrouch: {v}"); };
            OnSlide += (v) =>
            {
                Debug.Log($"[PlayerController] OnSlide: {v}");
                _playerState.IsSliding = v == EventContext.Begin;
                _rb.linearVelocityX *= slidingSpeedMultiplier;
            };
            OnWallSlide += (v) => { Debug.Log($"[PlayerController] OnWallSlide: {v}"); };
        }

        private void OnEnable()
        {
            _jumpAction.performed += OnJumpInputPerformed;
            _jumpAction.canceled += OnJumpInputCanceled;

            _crouchAction.performed += OnCrouchInputPerformed;
            _crouchAction.canceled += OnCrouchInputCanceled;
        }

        private void OnDisable()
        {
            _jumpAction.performed -= OnJumpInputPerformed;
            _jumpAction.canceled -= OnJumpInputCanceled;

            _crouchAction.performed -= OnCrouchInputPerformed;
            _crouchAction.canceled -= OnCrouchInputCanceled;
        }

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            Handles.BeginGUI();
            Handles.Label(transform.position + Vector3.up * 4,
                GizmosUtil.ColoredField("vel", _rb != null ? _rb.linearVelocity : Vector2.zero) +
                GizmosUtil.ColoredField("moving", _playerState.Moving) +
                GizmosUtil.ColoredField("grounded", _playerState.IsGrounded) +
                GizmosUtil.ColoredField("crouching", _playerState.IsCrouching) +
                GizmosUtil.ColoredField("sliding", _playerState.IsSliding) +
                GizmosUtil.ColoredField("jumping", _playerState.IsJumping) +
                GizmosUtil.ColoredField("jumpTimer", _jumpTimer) +
                GizmosUtil.ColoredField("ledgeGrab", _playerState.LedgeGrabbing) +
                GizmosUtil.ColoredField("ledgeClimb", _playerState.IsClimbingLedge) +
                GizmosUtil.ColoredField("wall_rub", _playerState.IsRubbingWall) +
                GizmosUtil.ColoredField("dead", _playerState.IsDead), GizmosUtil.GizmoTextStyle);

            Handles.EndGUI();

            var pos = (Vector2)transform.position;
            Gizmos.color = _playerState.IsGrounded ? Color.red : Color.green;
            Gizmos.DrawWireCube(pos + groundCheckOffset, groundCheckSize);

            Gizmos.color = _rightWall.HasWall ? Color.red : Color.green;
            Gizmos.DrawWireCube(pos + wallCheckOffset, wallCheckSize);

            Gizmos.color = _leftWall.HasWall ? Color.red : Color.green;
            Gizmos.DrawWireCube(pos + wallCheckOffset * new Vector2(-1, 1), wallCheckSize);

            Gizmos.color = _playerState.LedgeGrabbing == DirectionalState.Right ? Color.red : Color.green;
            Gizmos.DrawWireCube(pos + ledgeCheckOffset, ledgeCheckSize);

            Gizmos.color = _playerState.LedgeGrabbing == DirectionalState.Left ? Color.red : Color.green;
            Gizmos.DrawWireCube(pos + ledgeCheckOffset * new Vector2(-1, 1), ledgeCheckSize);
        }
#endif

        // Update is called once per frame
        private void Update()
        {
            if (_playerState.IsDead || gameManager.IsPaused)
            {
                return;
            }

            CheckGround();
            CheckWall();

            if (!Immobile)
            {
                HandleMovement();
                HandleWedgeGrabbing();
                HandleSliding();
                HandleJump();
            }

            UpdateAnimator();
        }

        public void OnDamaged(DamagingObject damagingObject)
        {
            _playerState.IsDead = true;
            UpdateAnimator();
        }

        #region AnimatorCallbacks

        // Called by Animator
        [PublicAPI]
        private void EndLedgeClimbingAnimation()
        {
            _playerState.IsClimbingLedge = false;
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

        private void CheckGround()
        {
            var lastGrounded = _playerState.IsGrounded;
            _playerState.IsGrounded = Physics2D.OverlapBox(
                (Vector2)transform.position + groundCheckOffset,
                groundCheckSize,
                0,
                obstacleLayer
            );

            if (lastGrounded != _playerState.IsGrounded && _playerState.IsGrounded)
            {
                OnLanding?.Invoke();
            }
        }

        private void CheckWall()
        {
            var pos = (Vector2)transform.position;
            _rightWall.HasWall = Physics2D.OverlapBox(pos + wallCheckOffset, wallCheckSize,
                0, obstacleLayer);
            _leftWall.HasWall = Physics2D.OverlapBox(pos + wallCheckOffset * new Vector2(-1, 1), wallCheckSize,
                0, obstacleLayer);

            var ledgeSpaceOffset = new Vector2(0, ledgeCheckSize.y);
            var rLedgeOffset = pos + ledgeCheckOffset;
            var rLedge = Physics2D.OverlapBox(rLedgeOffset, ledgeCheckSize, 0, obstacleLayer);
            var rLedgeSpace = Physics2D.OverlapBox(rLedgeOffset + ledgeSpaceOffset, ledgeCheckSize, 0, obstacleLayer);

            var lLedgeOffset = pos + ledgeCheckOffset * new Vector2(-1, 1);
            var lLedge = Physics2D.OverlapBox(lLedgeOffset, ledgeCheckSize, 0, obstacleLayer);
            var lLedgeSpace = Physics2D.OverlapBox(lLedgeOffset + ledgeSpaceOffset, ledgeCheckSize, 0, obstacleLayer);

            _rightWall.HasSpaceToClimb = rLedge && !rLedgeSpace;
            _leftWall.HasSpaceToClimb = lLedge && !lLedgeSpace;

            _playerState.LedgeGrabbing = _rightWall.HasSpaceToClimb && !_playerState.IsCrouching
                ? DirectionalState.Right
                : _leftWall.HasSpaceToClimb && !_playerState.IsCrouching
                    ? DirectionalState.Left
                    : DirectionalState.None;

            _playerState.IsRubbingWall =
                _rb.linearVelocityY < 0.01F &&
                !_playerState.IsGrounded &&
                (_playerState.Moving == DirectionalState.Right ? _rightWall.HasWall : _leftWall.HasWall);
        }

        private void HandleMovement()
        {
            var movement = _moveAction.ReadValue<Vector2>();

            if (_playerState.IsSliding || _playerState.IsCrouching) return;

            _playerState.Moving = movement.x switch
            {
                > 0.01F => DirectionalState.Right,
                < -0.01F => DirectionalState.Left,
                _ => _playerState.Moving
            };

            if (!_playerState.IsGrounded)
            {
                _rb.linearVelocityX += movement.x * airSpeedMultiplier * Time.deltaTime;
                return;
            }

            _rb.linearVelocityX =
                Mathf.SmoothDamp(_rb.linearVelocityX, movement.x * speed, ref _currentXVelocity, smoothTime);
        }

        private void HandleWedgeGrabbing()
        {
            var movement = _moveAction.ReadValue<Vector2>();

            _rb.gravityScale = _playerState.LedgeGrabbing != DirectionalState.None ? 0 : 1;
            if (_playerState.LedgeGrabbing != DirectionalState.None)
            {
                _rb.linearVelocityY = 0;

                switch (movement.x)
                {
                    case >= 0.5F when _playerState.LedgeGrabbing == DirectionalState.Right:
                    case <= -0.5F when _playerState.LedgeGrabbing == DirectionalState.Left:
                        _playerState.IsClimbingLedge = true;
                        break;
                }
            }
            else
            {
                _playerState.IsClimbingLedge = false;
            }
        }

        private void HandleSliding()
        {
            if (!_playerState.IsCrouching || !_playerState.IsGrounded || Mathf.Abs(_rb.linearVelocityX) < 0.05F)
            {
                if (!_playerState.IsSliding) return;

                OnSlide?.Invoke(EventContext.End);
                return;
            }

            if (_playerState.IsSliding) return;

            OnSlide?.Invoke(EventContext.Begin);
        }

        private void HandleJump()
        {
            if (!_playerState.IsJumping)
            {
                if (_playerState.IsGrounded)
                {
                    _jumpTimer = 0;
                }

                return;
            }

            if (_jumpTimer <= jumpTime)
            {
                _jumpTimer += Time.deltaTime;
                _rb.linearVelocityY = jumpForce;
                return;
            }

            _playerState.IsJumping = false;
        }

        private void UpdateAnimator()
        {
            animator.SetFloat(_animVelX, _rb.linearVelocityX);
            animator.SetFloat(_animVelY, _rb.linearVelocityY);
            animator.SetBool(_animIsGrounded, _playerState.IsGrounded);
            animator.SetBool(_animIsCrouching, _playerState.IsCrouching);
            animator.SetBool(_animIsJumping, _playerState.IsJumping);
            animator.SetBool(_animLookLeft,
                _playerState.IsGrabbingLedge
                    ? _playerState.LedgeGrabbing == DirectionalState.Left
                    : _playerState.Moving == DirectionalState.Left);
            animator.SetBool(_animIsGrabbingLedge, _playerState.IsGrabbingLedge);
            animator.SetBool(_animIsClimbingLedge, _playerState.IsClimbingLedge);
            animator.SetBool(_animIsSliding, _playerState.IsSliding);
            animator.SetBool(_animIsRubbingWall, _playerState.IsRubbingWall);
            animator.SetBool(_animIsDead, _playerState.IsDead);
        }

        private void OnJumpInputPerformed(InputAction.CallbackContext ctx)
        {
            switch (_playerState)
            {
                case { IsClimbingLedge: true } or { IsGrabbingLedge: true }:
                    _playerState.IsClimbingLedge = true;
                    return;
                case { IsGrounded: false, IsRubbingWall: true, IsJumping: false }:
                    _rb.linearVelocity = new Vector2(
                        wallJumpVelocity.x * (_playerState.Moving == DirectionalState.Right ? -1 : 1),
                        wallJumpVelocity.y
                    );
                    _playerState.Moving = _playerState.Moving == DirectionalState.Left
                        ? DirectionalState.Right
                        : DirectionalState.Left;
                    _jumpTimer = 0;
                    _playerState.IsJumping = true;
                    OnWallJumping?.Invoke();
                    return;
                case { IsGrounded: true }:
                    _jumpTimer = 0;
                    _playerState.IsJumping = true;
                    OnJump?.Invoke(EventContext.Begin);
                    break;
            }
        }

        private void OnJumpInputCanceled(InputAction.CallbackContext ctx)
        {
            if (!_playerState.IsJumping) return;

            _playerState.IsJumping = false;
            OnJump?.Invoke(EventContext.End);
        }

        private void OnCrouchInputPerformed(InputAction.CallbackContext ctx)
        {
            _playerState.IsCrouching = true;
            OnCrouch?.Invoke(EventContext.Begin);

            if (Mathf.Abs(_rb.linearVelocityX) < 0.05F || !_playerState.IsGrounded) return;
            OnSlide?.Invoke(EventContext.Begin);
        }

        private void OnCrouchInputCanceled(InputAction.CallbackContext ctx)
        {
            _playerState.IsCrouching = false;
            OnCrouch?.Invoke(EventContext.End);
        }
    }
}