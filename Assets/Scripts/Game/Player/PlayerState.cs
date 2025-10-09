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
        private float _lastGroundedTime;
        private long _lastGroundedFrame;
        private bool _isGrounded;
        private bool _isJumping;
        private bool _immobile;

        public bool Immobile
        {
            get => _immobile;
            set
            {
                _immobile = value;
                Rigidbody.simulated = !value;
            }
        }

        public bool IsDead { get; set; }

        public LookDirection LookDirection { get; set; }

        public bool IsGrounded
        {
            get => _isGrounded;
            set
            {
                if (_isGrounded == value) return;

                _isGrounded = value;
                LastGroundedTime = Time.time;
                LastGroundedFrame = Time.frameCount;

                if (_isGrounded)
                {
                    OnLand?.Invoke();
                }
            }
        }

        public bool IsCrouching { get; set; }

        public bool IsJumping
        {
            get => _isJumping;
            set
            {
                _isJumping = value;
                if (value)
                {
                    OnJump?.Invoke();
                }
            }
        }

        public bool IsJumpingCanceled { get; set; }
        public bool IsSliding { get; set; }
        public bool IsGrabbingLedge { get; set; }
        public bool IsClimbingLedge { get; set; }

        public bool HasWall { get; set; }
        public bool CanClimb { get; set; }

        public float LastGroundedTime
        {
            get => IsGrounded ? Time.time : _lastGroundedTime;
            private set => _lastGroundedTime = value;
        }

        public long LastGroundedFrame
        {
            get => IsGrounded ? Time.frameCount : _lastGroundedFrame;
            private set => _lastGroundedFrame = value;
        }

        public Rigidbody2D Rigidbody => rb;

        public event System.Action OnJump;
        public event System.Action OnLand;

        public void OnDamaged()
        {
            IsDead = true;
        }

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            Handles.BeginGUI();
            Handles.Label(transform.position + Vector3.up * 4,
                GizmosUtil.ColoredField("immobile", Immobile) +
                GizmosUtil.ColoredField("dead", IsDead) +
                GizmosUtil.ColoredField("look", LookDirection.ToString(), true) +
                GizmosUtil.ColoredField("grounded", IsGrounded) +
                GizmosUtil.ColoredField("crouching", IsCrouching) +
                GizmosUtil.ColoredField("jumping", IsJumping) +
                GizmosUtil.ColoredField("jump_canceled", IsJumpingCanceled) +
                GizmosUtil.ColoredField("sliding", IsSliding) +
                GizmosUtil.ColoredField("ledge_grab", IsGrabbingLedge) +
                GizmosUtil.ColoredField("ledge_climb", IsClimbingLedge) +
                GizmosUtil.ColoredField("has_wall", HasWall) +
                GizmosUtil.ColoredField("can_climb", CanClimb) +
                GizmosUtil.ColoredField("vel", rb != null ? rb.linearVelocity : Vector2.zero) +
                GizmosUtil.ColoredField("grounded_time", LastGroundedTime) +
                GizmosUtil.ColoredField("grounded_frame", LastGroundedFrame),
                GizmosUtil.GizmoTextStyle);

            Handles.EndGUI();
        }
#endif
    }
}