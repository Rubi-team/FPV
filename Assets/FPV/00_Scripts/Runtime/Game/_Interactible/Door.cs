using System;
using Unity.Netcode;
using UnityEngine;

namespace FPV.Runtime
{
    [RequireComponent(typeof(Animation))]
    public class Door : MonoBehaviour
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
            _animation = GetComponent<Animation>();
        }

        /// <summary>
        /// Méthode publique pour déclencher l'ouverture de la porte.
        /// </summary>
        public void TriggerDoor()
        {
            if (_isDoorOpen) return;

            if (_requiresTwoAttempts)
            {
                if (Time.time - _lastAttemptTime > 1f)
                    // Réinitialiser les tentatives si plus de 1 seconde s'est écoulée depuis la dernière tentative
                    _attempts = 0;

                _attempts++;
                _lastAttemptTime = Time.time; // Met à jour le temps de la dernière tentative

                if (_attempts < 2) return;
            }

            OpenDoor();
        }

        /// <summary>
        /// Méthode pour gérer l'ouverture de la porte.
        /// </summary>
        private void OpenDoor()
        {
            if (NetworkManager.Singleton.IsHost)
            {
                _isDoorOpen = true;
                PlayAnimationOnClientsRpc();
                Debug.Log("La porte s'est ouverte !");
            }
        }

        /// <summary>
        /// RPC pour jouer l'animation sur tous les clients.
        /// </summary>
        [ClientRpc]
        private void PlayAnimationOnClientsRpc()
        {
            if (_animation != null)
                _animation.Play(); // Remplacez par le nom de l'animation si nécessaire (ex. "DoorOpen")
        }
    }
}