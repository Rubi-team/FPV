using System;
using FPV.Runtime.Shared;
using Unity.Netcode;
using UnityEngine;

namespace FPV.Runtime
{
    [RequireComponent(typeof(Animation), typeof(NetworkObject))]
    public class Door : NetworkBehaviour
    {
        private Animation _animation; // Référence au composant Animation
        private bool _isDoorOpen = false;

        [Tooltip("Indique si la porte s'ouvre automatiquement lorsqu'elle est déclenchée")] [SerializeField]
        public bool AutoOpenWhenStaffIsRecover = false;

        [Tooltip("Indique si la porte nécessite deux tentatives pour s'ouvrir")] [SerializeField]
        private bool _requiresTwoAttempts = false;

        private int _attempts = 0; // Compteur des tentatives
        private float _lastAttemptTime = -Mathf.Infinity; // Temps de la dernière tentative

        private void Awake()
        {
            CustomNetworkManager.Singleton.OnServerPrepareGame += Init;
        }

        public void Init()
        {
            var netObject = GetComponent<NetworkObject>();

            if (!IsHost)
            {
                Debug.LogError("Door must be spawned on the server.");
                Destroy(gameObject);
                return;
            }

            if (!netObject.IsSpawned) netObject.Spawn();
            _animation = GetComponent<Animation>();
        }

        /// <summary>
        /// Méthode publique pour déclencher l'ouverture de la porte.
        /// </summary>
        [Rpc(SendTo.Server)]
        public void TriggerDoorServerRpc()
        {
            if (_isDoorOpen) return;

            if (_requiresTwoAttempts)
            {
                if (Time.time - _lastAttemptTime > 3f)
                    // Réinitialiser les tentatives si plus de 3 seconde s'est écoulée depuis la dernière tentative
                    _attempts = 0;

                _attempts++;
                _lastAttemptTime = Time.time; // Met à jour le temps de la dernière tentative

                if (_attempts < 2) return;
            }

            OpenDoorRpc();
        }

        /// <summary>
        /// Méthode pour gérer l'ouverture de la porte.
        /// </summary>
        [Rpc(SendTo.Everyone)]
        private void OpenDoorRpc()
        {
            _isDoorOpen = true;
            var doorAnimation = GetComponent<Animation>();
            if (doorAnimation != null)
                doorAnimation.Play();
            Debug.Log("La porte s'est ouverte !");
        }
        
    }
}