using UnityEngine;

namespace FPV.Runtime
{
    public class LaserEmitter : MonoBehaviour
    {
        [Header("Laser Settings")] public LineRenderer lineRenderer;
        public float maxDistance = 100f;
        public LayerMask obstacleLayers;
        public LayerMask detectionLayers;
        public float checkInterval = 0.1f;
        public bool debugDraw = true;

        private float lastCheckTime;
        private RaycastHit[] hits = new RaycastHit[1];

        private void Update()
        {
            if (Time.time - lastCheckTime >= checkInterval)
            {
                lastCheckTime = Time.time;
                UpdateLaser();
            }
        }

        private void UpdateLaser()
        {
            var start = transform.position;
            var dir = transform.forward;

            var distance = maxDistance;
            var ray = new Ray(start, dir);
            var hitCount = Physics.RaycastNonAlloc(ray, hits, maxDistance, obstacleLayers | detectionLayers);

            if (hitCount > 0)
            {
                var hit = hits[0];
                distance = hit.distance;

                if (((1 << hit.collider.gameObject.layer) & detectionLayers) != 0)
                    if (hit.collider.TryGetComponent<PlayerApplication>(out _))
                        TriggerAlarm(hit.collider.gameObject);
            }

            if (lineRenderer != null)
            {
                lineRenderer.SetPosition(0, Vector3.zero);
                lineRenderer.SetPosition(1, Vector3.forward * distance);
            }

            if (debugDraw)
                Debug.DrawRay(start, dir * distance, Color.red, checkInterval);
        }

        private void TriggerAlarm(GameObject intruder)
        {
            Debug.LogWarning($"🚨 ALARME : {intruder.name} a été détecté par {gameObject.name}");
        }
    }
}