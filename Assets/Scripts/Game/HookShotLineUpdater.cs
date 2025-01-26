using System;
using UnityEngine;

namespace Game
{
    public class HookShotLineUpdater : MonoBehaviour
    {
        [SerializeField] private LineRenderer lineRenderer;
        [SerializeField] private Transform hookShotHitReference;
        [SerializeField] private Transform hookShotBeginReference;

        private void Awake()
        {
            lineRenderer.SetPosition(0, hookShotBeginReference.position);
            lineRenderer.SetPosition(1, hookShotHitReference.position);
        }

        private void Update()
        {
            lineRenderer.SetPosition(0, hookShotBeginReference.position);
            lineRenderer.SetPosition(1, hookShotHitReference.position);
        }
    }
}
