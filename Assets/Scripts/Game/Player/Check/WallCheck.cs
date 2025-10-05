using System;
using UnityEngine;

namespace Game.Player.Check
{
    public class WallCheck : MonoBehaviour
    {
        [SerializeField] private PlayerState playerState;
        [SerializeField] private LayerMask obstacleLayer = int.MaxValue;

        [Header("Wall Check")]
        [SerializeField] private Vector2 wallCheckOffset = new Vector2(0.5F, 0F);

        [SerializeField] private Vector2 wallCheckSize = new Vector2(0.05F, 1.95F);

        [Header("Ledge Check")]
        [SerializeField] private Vector2 ledgeCheckOffset = new Vector2(0.5F, 1F);

        [SerializeField] private Vector2 ledgeCheckSize = new Vector2(0.5F, 0.1F);


        private void Update()
        {
            var offset = playerState.LookDirection == LookDirection.Left ? new Vector2(-1, 1) : new Vector2(1, 1);
            CheckWall(offset, out var hasWall, out var hasSpaceToClimb);

            playerState.HasWall = hasWall;
            playerState.CanClimb = hasSpaceToClimb;
        }

        private void CheckWall(Vector2 multiplier, out bool hasWall, out bool hasSpaceToClimb)
        {
            var pos = playerState.Rigidbody.position;
            hasWall = Physics2D.OverlapBox(pos + wallCheckOffset * multiplier, wallCheckSize * multiplier, 0, obstacleLayer);

            var ledgeSpaceOffset = new Vector2(0, ledgeCheckSize.y);
            var ledgeOffset = pos + ledgeCheckOffset * multiplier;
            var ledge = Physics2D.OverlapBox(ledgeOffset, ledgeCheckSize, 0, obstacleLayer);
            var ledgeSpace = Physics2D.OverlapBox(ledgeOffset + ledgeSpaceOffset, ledgeCheckSize, 0, obstacleLayer);

            hasSpaceToClimb = ledge && !ledgeSpace;
        }
    }
}