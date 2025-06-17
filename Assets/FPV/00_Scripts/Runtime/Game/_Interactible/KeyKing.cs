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
            //modifier le bool clé pour que le jeu puisse par la suite ouvrir la porte du roi.
            Debug.Log($"{other.name}");
            
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
