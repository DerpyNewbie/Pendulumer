using UnityEngine;

namespace Game
{
    public class HookShotLineUpdater : MonoBehaviour
    {
        [SerializeField] private Rigidbody2D hinge;
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
            if (hinge.simulated)
            {
                lineRenderer.SetPosition(0, hookShotBeginReference.position);
                lineRenderer.SetPosition(1, hookShotHitReference.position);
            }
            else
            {
                lineRenderer.SetPosition(0, Vector2.zero);
                lineRenderer.SetPosition(1, Vector2.zero);
            }
        }
    }
}