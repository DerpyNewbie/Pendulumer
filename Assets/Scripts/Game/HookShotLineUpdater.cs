using UnityEngine;

namespace Game
{
    public class HookShotLineUpdater : MonoBehaviour
    {
        [SerializeField] private LineRenderer lineRenderer;
        [SerializeField] private Transform hookShotHitReference;
        [SerializeField] private Transform hookShotBeginReference;

        private void Update()
        {
            UpdateLineRenderer();
        }

        private void OnEnable()
        {
            UpdateLineRenderer();
        }

        private void UpdateLineRenderer()
        {
            lineRenderer.SetPosition(0, hookShotBeginReference.position);
            lineRenderer.SetPosition(1, hookShotHitReference.position);
        }
    }
}