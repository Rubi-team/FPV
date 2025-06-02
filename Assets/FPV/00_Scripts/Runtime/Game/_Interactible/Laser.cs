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
        public Transform rotationPivot; // Pivot custom assignable

        [Header("Detection Settings")] public LayerMask obstacleLayers;
        public LayerMask detectionLayers;
        public float detectionRadius = 0.2f;

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
                // ✅ Rotation autour du pivot spécifié
                transform.RotateAround(rotationPivot.position, Vector3.up, rotationSpeed * Time.deltaTime);

            foreach (var lr in lineRenderers)
                UpdateLaserBeam(lr);
        }

        private void UpdateLaserBeam(LineRenderer lr)
        {
            var start = lr.transform.position;
            var direction = lr.transform.forward;

            var ray = new Ray(start, direction);
            RaycastHit hit;
            var maxDistance = 100f;

            if (Physics.Raycast(ray, out hit, maxDistance, obstacleLayers))
                maxDistance = hit.distance;

            // Debug
            Debug.DrawRay(start, direction * maxDistance, Color.red);
            var sphereSteps = Mathf.CeilToInt(maxDistance / 2f);
            for (var i = 0; i <= sphereSteps; i++)
            {
                var t = (float)i / sphereSteps;
                var point = start + direction * (maxDistance * t);
                DebugDrawWireSphere(point, detectionRadius, Color.yellow);
            }

            var hits = Physics.SphereCastAll(ray, detectionRadius, maxDistance, detectionLayers);
            foreach (var h in hits)
                if (h.collider.GetComponent<PlayerApplication>() != null)
                    TriggerAlarm(h.collider.gameObject);

            lr.SetPosition(0, Vector3.zero);
            lr.SetPosition(1, Vector3.forward * maxDistance);
        }


        private void TriggerAlarm(GameObject intruder)
        {
            Debug.LogWarning($"🚨 ALARME : {intruder.name} a traversé un laser !");
        }

        private void DebugDrawWireSphere(Vector3 position, float radius, Color color)
        {
#if UNITY_EDITOR
            var step = 10f;
            for (float theta = 0; theta < 360; theta += step)
            {
                var rad = Mathf.Deg2Rad * theta;
                var nextRad = Mathf.Deg2Rad * (theta + step);

                var p1 = position + new Vector3(Mathf.Cos(rad), 0, Mathf.Sin(rad)) * radius;
                var p2 = position + new Vector3(Mathf.Cos(nextRad), 0, Mathf.Sin(nextRad)) * radius;
                Debug.DrawLine(p1, p2, color);

                var p3 = position + new Vector3(0, Mathf.Cos(rad), Mathf.Sin(rad)) * radius;
                var p4 = position + new Vector3(0, Mathf.Cos(nextRad), Mathf.Sin(nextRad)) * radius;
                Debug.DrawLine(p3, p4, color);

                var p5 = position + new Vector3(Mathf.Cos(rad), Mathf.Sin(rad), 0) * radius;
                var p6 = position + new Vector3(Mathf.Cos(nextRad), Mathf.Sin(nextRad), 0) * radius;
                Debug.DrawLine(p5, p6, color);
            }
#endif
        }

        // Affichage du pivot dans la scène
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