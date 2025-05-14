using UnityEngine;

namespace FPV
{
    public class SoundPropagationSimulator : MonoBehaviour
    {
        public Transform soundSource;
        public Transform listener;
        public int raysPerSweep = 30;
        public float coneAngle = 45f;
        public int maxRebounds = 2;
        public float maxDistance = 40f;
        public LayerMask obstacleMask;
        public LayerMask listenerMask;

        private void Update()
        {
            if (soundSource == null || listener == null) return;

            if (TryDetectSound(out var usedRebounds, out var totalDistance))
                Debug.Log($"→ Son entendu ! Rebonds : {usedRebounds}, Distance : {totalDistance:F2}");
        }

        private bool TryDetectSound(out int bestRebounds, out float bestDistance)
        {
            var origin = soundSource.position;
            var toListener = (listener.position - origin).normalized;

            bestRebounds = -1;
            bestDistance = float.MaxValue;
            var heard = false;

            for (var i = 0; i < raysPerSweep; i++)
            {
                // Génère une direction aléatoire dans un cône
                var direction = RandomDirectionInCone(toListener, coneAngle);

                if (ReflectiveRaycast(origin, direction, listener.position, maxRebounds, out var rebounds,
                        out var totalDist))
                    if (totalDist < bestDistance)
                    {
                        bestDistance = totalDist;
                        bestRebounds = rebounds;
                        heard = true;
                    }
            }

            return heard;
        }

        private bool ReflectiveRaycast(Vector3 origin, Vector3 direction, Vector3 listenerPos, int maxRebounds,
            out int rebounds, out float totalDistance)
        {
            totalDistance = 0f;
            rebounds = 0;

            for (var i = 0; i <= maxRebounds; i++)
                if (Physics.Raycast(origin, direction, out var hit, maxDistance - totalDistance,
                        obstacleMask | listenerMask))
                {
                    totalDistance += hit.distance;

                    if (((1 << hit.collider.gameObject.layer) & listenerMask) != 0)
                        // Touché le listener
                        return true;

                    // Rebond : recalcul direction
                    origin = hit.point + hit.normal * 0.01f; // petite marge pour éviter collision immédiate
                    direction = Vector3.Reflect(direction, hit.normal);
                    rebounds++;
                }
                else
                {
                    break;
                }

            return false;
        }

        private Vector3 RandomDirectionInCone(Vector3 forward, float angle)
        {
            var randomRot = Quaternion.AngleAxis(Random.Range(-angle / 2f, angle / 2f), Vector3.up) *
                            Quaternion.AngleAxis(Random.Range(-angle / 2f, angle / 2f), Vector3.right);
            return randomRot * forward;
        }

#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            if (soundSource == null || listener == null) return;

            var origin = soundSource.position;
            var toListener = (listener.position - origin).normalized;

            // Affiche les rayons de test dans le cône
            for (var i = 0; i < raysPerSweep; i++)
            {
                var dir = RandomDirectionInCone(toListener, coneAngle);
                DrawReflectiveRay(origin, dir, maxRebounds);
            }
        }

        private void DrawReflectiveRay(Vector3 origin, Vector3 direction, int maxRebounds)
        {
            var remainingDist = maxDistance;
            var currentOrigin = origin;
            var currentDir = direction;

            Gizmos.color = Color.yellow;

            for (var i = 0; i <= maxRebounds; i++)
                if (Physics.Raycast(currentOrigin, currentDir, out var hit, remainingDist, obstacleMask | listenerMask))
                {
                    Gizmos.DrawLine(currentOrigin, hit.point);
                    remainingDist -= Vector3.Distance(currentOrigin, hit.point);

                    if (((1 << hit.collider.gameObject.layer) & listenerMask) != 0)
                    {
                        Gizmos.color = Color.green;
                        Gizmos.DrawSphere(hit.point, 0.1f);
                        return;
                    }

                    currentOrigin = hit.point + hit.normal * 0.01f;
                    currentDir = Vector3.Reflect(currentDir, hit.normal);
                    Gizmos.color = Color.cyan;
                }
                else
                {
                    Gizmos.DrawRay(currentOrigin, currentDir * remainingDist);
                    return;
                }
        }
#endif
    }
}