using System.Collections.Generic;
using UnityEngine;

namespace FPV.Runtime
{
    public class Laser : MonoBehaviour
    {
#if UNITY_EDITOR
        [Header("Editor Settings")] public GameObject beamPrefab;
        [Range(1, 10)] public int editorLaserCount = 1;

        public bool useSpacing = false;
        public float beamSpacing = 0.5f;
        public Vector3 spacingAxis = Vector3.forward;
#endif
        [Header("Laser Settings")] public bool isRotating = false;
        public float rotationSpeed = 30f;
        public Transform rotationPivot;

        [Header("Detection Settings")] public LayerMask obstacleLayers;
        public LayerMask detectionLayers;

        [Header("Performance Settings")] public float checkInterval = 0.1f;
        public float visibilityCheckInterval = 0.5f;
        public bool debugDrawRay = true;

        private float lastCheckTime = 0f;
        private float lastVisibilityCheck = 0f;
        private bool isVisible = true;

        private RaycastHit[] raycastHits = new RaycastHit[1];
        private List<LineRenderer> lineRenderers = new();

        private void Start()
        {
            foreach (Transform child in transform)
            {
                var lr = child.GetComponentInChildren<LineRenderer>();
                if (lr != null)
                    lineRenderers.Add(lr);
            }
        }

        private void Update()
        {
            if (isRotating && rotationPivot != null)
                transform.RotateAround(rotationPivot.position, Vector3.up, rotationSpeed * Time.deltaTime);

            CheckVisibility();

            if (!isVisible)
                return;

            if (Time.time - lastCheckTime >= checkInterval)
            {
                lastCheckTime = Time.time;
                foreach (var lr in lineRenderers)
                    UpdateLaserBeam(lr);
            }
        }

        private void CheckVisibility()
        {
            if (Time.time - lastVisibilityCheck < visibilityCheckInterval)
                return;

            lastVisibilityCheck = Time.time;
            isVisible = false;

            foreach (var lr in lineRenderers)
            {
                var renderer = lr.GetComponent<Renderer>();
                if (renderer != null && renderer.isVisible)
                {
                    isVisible = true;
                    break;
                }
            }
        }

        private void UpdateLaserBeam(LineRenderer lr)
        {
            var start = lr.transform.position;
            var direction = lr.transform.forward;
            var maxDistance = 100f;

            var ray = new Ray(start, direction);
            var hitCount = Physics.RaycastNonAlloc(ray, raycastHits, maxDistance, obstacleLayers | detectionLayers);

            if (hitCount > 0)
            {
                var hit = raycastHits[0];
                maxDistance = hit.distance;

                if (((1 << hit.collider.gameObject.layer) & detectionLayers) != 0)
                    if (hit.collider.TryGetComponent<PlayerApplication>(out _))
                        TriggerAlarm(hit.collider.gameObject);
            }

            if (debugDrawRay)
                Debug.DrawRay(start, direction * maxDistance, Color.red, checkInterval);

            if (lr != null)
            {
                lr.SetPosition(0, Vector3.zero);
                lr.SetPosition(1, Vector3.forward * maxDistance);
            }
        }

        private void TriggerAlarm(GameObject intruder)
        {
            Debug.LogWarning($"🚨 ALARME : {intruder.name} a traversé un laser !");
        }

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            if (rotationPivot != null)
            {
                Gizmos.color = Color.cyan;
                Gizmos.DrawWireSphere(rotationPivot.position, 0.25f);
                Gizmos.DrawLine(transform.position, rotationPivot.position);
            }
        }
#endif
    }
}