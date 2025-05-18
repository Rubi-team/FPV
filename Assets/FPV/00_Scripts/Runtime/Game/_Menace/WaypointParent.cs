using UnityEngine;

public class WaypointParent : MonoBehaviour
{
    [SerializeField] private string roomName;

    public string RoomName => roomName;

    private void OnDrawGizmos()
    {
        // Visualisation de la zone de la pièce dans l'éditeur
        Gizmos.color = new Color(0, 1, 0, 0.2f);
        Gizmos.DrawWireCube(transform.position, Vector3.one * 2f);
    }
}