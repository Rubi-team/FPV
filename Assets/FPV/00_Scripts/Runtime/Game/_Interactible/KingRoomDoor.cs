using System;
using FPV.Runtime;
using UnityEngine;

public class KingRoomDoor : MonoBehaviour
{
    [SerializeField] private Door kingDoor;

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<PlayerApplication>(out var playerApp))
        {
            // si les joueurs ont récupéré la clef, la porte s'ouvre.
            kingDoor.TriggerDoorServerRpc();
        }
    }
}
