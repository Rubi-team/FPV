using System;
using Audio;
using FPV.Runtime;
using FPV.Runtime.Shared;
using Unity.Netcode;
using UnityEngine;

public class KeyKing : NetworkBehaviour
{
    private NetworkObject netObject;
    
    private void Awake()
    {
        CustomNetworkManager.Singleton.OnServerPrepareGame += Init;
    }
    
    public void Init()
    {
        netObject = GetComponent<NetworkObject>();

        if (!IsHost)
        {
            Debug.LogError("Key must be spawned on the server.");
            Destroy(gameObject);
            return;
        }

        if (!netObject.IsSpawned) netObject.Spawn();
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<PlayerApplication>(out var playerApp))
        {
            NetworkManager.SpawnManager.PlayerObjects[0].GetComponent<PlayerApplication>().OnPlayerHasKeyRpc();
            NetworkManager.SpawnManager.PlayerObjects[1].GetComponent<PlayerApplication>().OnPlayerHasKeyRpc();
            
            //détruire l'objet avec les feedbacks
            KeySoundRpc();
        }
    }
    
    [Rpc(SendTo.Everyone)]
    private void KeySoundRpc()
    {
        AudioManager.Instance.PlayOneShot(AudioManager.Instance.keyCollect, transform.position, NetworkManager.Singleton.LocalClientId, 3);
        DestroyObjectRpc();
    }

    [Rpc(SendTo.Server)]
    private void DestroyObjectRpc()
    {
        netObject.Despawn();
    }
    
    
}
