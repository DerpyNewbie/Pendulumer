using System;
using Unity.Cinemachine;
using UnityEngine;

namespace Game
{
    public class CameraController : MonoBehaviour
    {
        [SerializeField] private Rigidbody2D targetRigidbody;
        [SerializeField] private CinemachineCamera cinemachineCamera;
        [SerializeField] private float minMagnitude = 5.0f;
        [SerializeField] private float maxMagnitude = 50F;
        [SerializeField] private float baseSize = 10F;
        [SerializeField] private float maxSize = 20F;
        [SerializeField] private float smoothTime = 5F;
        [SerializeField] private float minDistance = 0.2f;

        private float _velocity;

        private void Update()
        {
            cinemachineCamera.Lens.OrthographicSize = Mathf.SmoothDamp(cinemachineCamera.Lens.OrthographicSize,
                Mathf.Lerp(baseSize, maxSize,
                    (Mathf.Clamp(targetRigidbody.linearVelocity.magnitude, minMagnitude, maxMagnitude) - minMagnitude) /
                    (maxMagnitude - minMagnitude)), ref _velocity, smoothTime);
        }
    }
}