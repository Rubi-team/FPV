using System;
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
                if (hit.CompareTag("Player"))
                {
                    var directionToTarget = (hit.transform.position - transform.position).normalized;

                    // Vérifie si l'angle est dans le champ de vision
                    var angleToTarget = Vector3.Angle(transform.forward, directionToTarget);
                    if (angleToTarget < fieldOfViewAngle / 2f)
                    {
                        // Vérifie s'il y a une ligne de vue
                        var distanceToTarget = Vector3.Distance(transform.position, hit.transform.position);
                        var origin = transform.position + Vector3.up * 1f;
                        RaycastHit hitInfo;

                        if (!Physics.Raycast(origin, directionToTarget, out hitInfo, distanceToTarget, obstructionMask))
                        {
                            if (showDebug) Debug.Log("Player détecté et visible !");
                            detectedPlayer = hit.transform;
                        }
                        else
                        {
                            if (showDebug)
                                Debug.Log(
                                    $"Player détecté mais hors de vue (obstruction par : {hitInfo.collider.gameObject.name})");

                            lastKnownPosition = detectedPlayer.position;
                            detectedPlayer = null;
                        }

                        if (showDebug) Debug.DrawLine(origin, hit.transform.position, Color.red);
                    }
                    else
                    {
                        if (showDebug) Debug.DrawLine(transform.position, hit.transform.position, Color.yellow);
                        Debug.Log("Player détecté mais hors de vue (Pas dans Champ de vision).");
                    }
                }
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(transform.position, detectionRadius);

            if (Application.isPlaying && showDebug)
            {
                var leftLimit = Quaternion.Euler(0, -fieldOfViewAngle / 2f, 0) * transform.forward;
                var rightLimit = Quaternion.Euler(0, fieldOfViewAngle / 2f, 0) * transform.forward;

                Gizmos.color = Color.blue;
                var origin = transform.position + Vector3.up * 1f;
                Gizmos.DrawRay(origin, leftLimit * detectionRadius);
                Gizmos.DrawRay(origin, rightLimit * detectionRadius);
            }
        }
    }
}