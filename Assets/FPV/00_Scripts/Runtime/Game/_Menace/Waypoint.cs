using UnityEngine;

public class Waypoint : MonoBehaviour
{
    [SerializeField] private Waypoint nextWaypoint;

    public Waypoint GetNextWaypoint()
    {
        return nextWaypoint;
    }

    private void OnDrawGizmos()
    {
        // Visualisation des waypoints dans l'éditeur
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, 0.5f);

        if (nextWaypoint != null) Gizmos.DrawLine(transform.position, nextWaypoint.transform.position);
    }
}