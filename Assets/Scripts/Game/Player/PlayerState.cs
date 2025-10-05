using System;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Game.Player
{
    public enum LookDirection
    {
        Left,
        Right
    }

    public class PlayerState : MonoBehaviour
    {
        [SerializeField] private Rigidbody2D rb;
        private bool _isGrounded;

        public bool IsDead { get; set; }

        public LookDirection LookDirection { get; set; }

        public bool IsGrounded
        {
            get => _isGrounded;
            set
            {
                if (_isGrounded == value) return;

                _isGrounded = value;
                if (!_isGrounded) return;

                LastGroundedTime = Time.time;
                LastGroundedFrame = Time.frameCount;
            }
        }

        public bool IsCrouching { get; set; }
        public bool IsJumping { get; set; }
        public bool IsJumpingCanceled { get; set; }
        public bool IsSliding { get; set; }
        public bool IsGrabbingLedge { get; set; }
        public bool IsClimbingLedge { get; set; }

        public bool HasWall { get; set; }
        public bool CanClimb { get; set; }

        public float LastGroundedTime { get; private set; }
        public long LastGroundedFrame { get; private set; }

        public Rigidbody2D Rigidbody => rb;

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            Handles.BeginGUI();
            Handles.Label(transform.position + Vector3.up * 4,
                GizmosUtil.ColoredField("vel", rb != null ? rb.linearVelocity : Vector2.zero) +
                GizmosUtil.ColoredField("look", LookDirection.ToString(), true) +
                GizmosUtil.ColoredField("grounded", IsGrounded) +
                GizmosUtil.ColoredField("jumping", IsJumping) +
                GizmosUtil.ColoredField("jump_canceled", IsJumpingCanceled) +
                GizmosUtil.ColoredField("crouching", IsCrouching) +
                GizmosUtil.ColoredField("sliding", IsSliding) +
                GizmosUtil.ColoredField("grounded_frame", LastGroundedFrame) +
                GizmosUtil.ColoredField("has_wall", HasWall) +
                GizmosUtil.ColoredField("can_climb", CanClimb), GizmosUtil.GizmoTextStyle);

            // GizmosUtil.ColoredField("autoSlide", _playerState.IsAutoSliding) +
            // GizmosUtil.ColoredField("jumping", _playerState.IsJumping) +
            // GizmosUtil.ColoredField("jumpTimer", _jumpTimer) +
            // GizmosUtil.ColoredField("ledgeGrab", _playerState.LedgeGrabbing) +
            // GizmosUtil.ColoredField("ledgeClimb", _playerState.IsClimbingLedge) +
            // GizmosUtil.ColoredField("wall_rub", _playerState.IsRubbingWall) +
            // GizmosUtil.ColoredField("dead", _playerState.IsDead), GizmosUtil.GizmoTextStyle);

            Handles.EndGUI();

            // var pos = (Vector2)transform.position;
            // Gizmos.color = _playerState.IsGrounded ? Color.red : Color.green;
            // Gizmos.DrawWireCube(pos + groundCheckOffset, groundCheckSize);
            //
            // Gizmos.color = _rightWall.HasWall ? Color.red : Color.green;
            // Gizmos.DrawWireCube(pos + wallCheckOffset, wallCheckSize);
            //
            // Gizmos.color = _leftWall.HasWall ? Color.red : Color.green;
            // Gizmos.DrawWireCube(pos + wallCheckOffset * new Vector2(-1, 1), wallCheckSize);
            //
            // Gizmos.color = _playerState.LedgeGrabbing == DirectionalState.Right ? Color.red : Color.green;
            // Gizmos.DrawWireCube(pos + ledgeCheckOffset, ledgeCheckSize);
            //
            // Gizmos.color = _playerState.LedgeGrabbing == DirectionalState.Left ? Color.red : Color.green;
            // Gizmos.DrawWireCube(pos + ledgeCheckOffset * new Vector2(-1, 1), ledgeCheckSize);
        }
#endif
    }
}