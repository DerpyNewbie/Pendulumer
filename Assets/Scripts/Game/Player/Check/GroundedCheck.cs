using System;
using UnityEngine;

namespace Game.Player.Check
{
    public class GroundedCheck : MonoBehaviour
    {
        [SerializeField] private PlayerState playerState;
        [SerializeField] private Rigidbody2D rb;
        [SerializeField] private LayerMask obstacleLayer = int.MaxValue;
        [SerializeField] private Vector2 groundCheckOffset = new Vector2(0, -1F);
        [SerializeField] private Vector2 groundCheckSize = new Vector2(0.2F, 0.2F);

        private void Update()
        {
            playerState.IsGrounded = Physics2D.OverlapBox(
                rb.position + groundCheckOffset,
                groundCheckSize,
                0,
                obstacleLayer
            );
        }
    }
}