using System;
using FPV.Runtime.Shared;
using FPV.Shared;
using Unity.Netcode;
using UnityEngine;

namespace FPV.Runtime
{
    [RequireComponent(typeof(NetworkObject), typeof(Animation))]
    public class Door : MonoBehaviour
    {
        private Animation _animation; // Référence au composant Animation

        private void Awake()
        {
            _animation = GetComponent<Animation>();
        }

        /// <summary>
        /// Méthode publique pour déclencher l'animation de la porte.
        /// </summary>
        public void TriggerDoorAnimation()
        {
            if (NetworkManager.Singleton.IsHost) PlayAnimationOnClientsRpc();
        }

        /// <summary>
        /// RPC pour jouer l'animation sur tous les clients.
        /// </summary>
        [ClientRpc]
        private void PlayAnimationOnClientsRpc()
        {
            if (_animation != null)
                _animation.Play(); // Remplace par le nom de ton animation si nécessaire, ex: "_animation.Play('DoorOpen')"
        }
    }
}