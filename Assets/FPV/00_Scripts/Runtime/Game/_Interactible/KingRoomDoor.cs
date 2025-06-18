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
            // Check if the player has the key
            if (playerApp.hasKey)
            {
                // Trigger the door opening
                kingDoor.TriggerDoorServerRpc();
            }
            else
            {
                Debug.Log("Player does not have the key to open the door.");
                return;
            }
            
        }
    }
}
