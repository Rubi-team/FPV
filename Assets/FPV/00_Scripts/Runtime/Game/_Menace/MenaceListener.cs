using System;
using FPV.Runtime;
using UnityEngine;

namespace FPV
{
    public class MenaceListener : MonoBehaviour
    {
        [Header("Detection Settings")] public float detectionRadius = 10f;
        public float fieldOfViewAngle = 90f;
        public LayerMask detectionMask;
        public LayerMask obstructionMask;

        [Header("Detected Player")] public Transform detectedPlayer;
        public Vector3 lastKnownPosition;

        [Header("Debug")] public bool showDebug = true;

        private void Start()
        {
            detectedPlayer = null;
        }

        private void Update()
        {
            DetectPlayers();
        }


        private void DetectPlayers()
{
    var hits = Physics.OverlapSphere(transform.position, detectionRadius, detectionMask);

    foreach (var hit in hits)
    {
        if (!hit.CompareTag("Player")) continue;

        var directionToTarget = (hit.transform.position - transform.position).normalized;
        var angleToTarget = Vector3.Angle(transform.forward, directionToTarget);

        if (angleToTarget < fieldOfViewAngle / 2f)
        {
            var distanceToTarget = Vector3.Distance(transform.position, hit.transform.position);
            var origin = transform.position + transform.forward * 2f + Vector3.up * 1f;

            if (!Physics.Raycast(origin, directionToTarget, out var hitInfo, distanceToTarget - 2f, obstructionMask))
            {
                detectedPlayer = hit.transform;

                if (showDebug)
                {
                    Debug.DrawLine(origin, hit.transform.position, Color.green); // ligne de détection réussie
                    Debug.Log($"[DETECTION] Player détecté sans obstruction: {hit.transform.name}", hit.transform);
                }
            }
            else
            {
                if (detectedPlayer != null)
                    lastKnownPosition = detectedPlayer.position;

                detectedPlayer = null;

                if (showDebug)
                {
                    Debug.DrawLine(origin, hitInfo.point, Color.red); // ligne bloquée
                    Debug.Log($"[RAYCAST BLOCKED] Obstruction détectée: {hitInfo.collider.name}", hitInfo.collider);
                }
            }
        }
        else
        {
            if (showDebug)
            {
                Debug.DrawLine(transform.position, hit.transform.position, Color.yellow); // hors champ de vision
                Debug.Log($"[FOV] {hit.transform.name} est hors champ de vision ({angleToTarget:F1}°)", hit.transform);
            }
        }

        if (detectedPlayer != null && hit.TryGetComponent<PlayerApplication>(out var player))
        {
            if (player.CurrentLoudness > 0.2f)
            {
                detectedPlayer = player.transform;
                if (showDebug) Debug.Log($"[LOUDNESS] Joueur détecté par bruit: {player.CurrentLoudness:F2}");
            }
            else
            {
                detectedPlayer = null;
                if (showDebug) Debug.Log($"[LOUDNESS] Bruit insuffisant: {player.CurrentLoudness:F2}");
            }
        }
    }
}



        private void OnDrawGizmosSelected()
        {
            // Sphère de détection
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(transform.position, detectionRadius);

            // Affichage du champ de vision (en cône)
            var origin = transform.position + Vector3.up * 1f;

            var leftLimit = Quaternion.Euler(0, -fieldOfViewAngle / 2f, 0) * transform.forward;
            var rightLimit = Quaternion.Euler(0, fieldOfViewAngle / 2f, 0) * transform.forward;

            Gizmos.color = Color.blue;
            Gizmos.DrawRay(origin, leftLimit * detectionRadius);
            Gizmos.DrawRay(origin, rightLimit * detectionRadius);

            // Arc de cercle du champ de vision
            Gizmos.color = new Color(0, 0.5f, 1f, 0.2f); // bleu semi-transparent
            int segments = 30;
            float angleStep = fieldOfViewAngle / segments;
            Vector3 prevPoint = origin + (Quaternion.Euler(0, -fieldOfViewAngle / 2f, 0) * transform.forward) * detectionRadius;
            for (int i = 1; i <= segments; i++)
            {
                float angle = -fieldOfViewAngle / 2f + angleStep * i;
                Vector3 nextPoint = origin + (Quaternion.Euler(0, angle, 0) * transform.forward) * detectionRadius;
                Gizmos.DrawLine(prevPoint, nextPoint);
                prevPoint = nextPoint;
            }
        }

    }
}