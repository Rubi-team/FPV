using System;
using System.Collections;
using System.Collections.Generic;
using Audio;
using FPV.Runtime.Shared;
using Unity.Cinemachine;
using UnityEngine;
using Unity.Netcode;
using Unity.Netcode.Components;
using Unity.Services.Vivox;
using UnityEngine.InputSystem.Composites;

namespace FPV.Runtime
{
    public class PlayerApplication : BaseNetworkApplication<PlayerModel, PlayerView, PlayerController>, IInteractable
    {
        [SerializeField] public float CurrentLoudness = 0f;
        [SerializeField] private MyVOIP _Voip;
        
        private GameObject _cinemachineCamera;

        public bool HasTakenAHit = false; 
        public NetworkVariable<bool> IsDead = new(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
        
        public bool hasKey = false;

        protected override void Awake()
        {
            base.Awake();
            Debug.Log("PlayerApplication initialized for the local player.");
        }

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
            Init();
            TpAtSpawn();
        }

        private void Update()
        {
            if (!IsOwner || !IsSpawned) return;
            CurrentLoudness = _Voip.GetLoudnessFromMicrophone();
            
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                PauseUI.Instance.ChangeUI(!PauseUI.Instance.pauseMenuActive);
            }
            
            if (PauseUI.Instance.pauseMenuActive)
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                // Disable input when the pause menu is active
                
            }
            else
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
                // Enable input when the pause menu is not active
                
            }
        }

        private void Init()
        {
            if (!IsOwner)
            {
                if (Controller._playerInput != null)
                {
                    Controller._playerInput.enabled = false;
                    Controller._playerInput.DeactivateInput();
                }

                return;
            }

            // If the player is the owner, we need to enable the input and set the camera

             _cinemachineCamera = Instantiate(Model.CinemachineCameraFollow, View.transform);
             _cinemachineCamera.GetComponent<CinemachineCamera>().Follow = Model.CinemachineCameraTarget.transform;

            if (Controller._playerInput != null)
            {
                Controller._playerInput.enabled = true;
                Controller._playerInput.ActivateInput();
            }

            // Disable our SkinnedMeshRenderer to avoid rendering the local player model
            if (Model.Mesh != null) Model.Mesh.enabled = false;

            // Join Vivox channel
            VivoxManager.Instance.VivoxInit();

            if (NetworkManager.LocalClientId == 1)
            {
                ChangeSkinRpc();
            }
        }

        [Rpc(SendTo.Owner)]
        public void OnPLayerHitRpc()
        {
            if (!IsOwner) return;
            if (HasTakenAHit && IsDead.Value) return;
            
            AudioManager.Instance.PlayOneShot(AudioManager.Instance.threatHit, View.Body.position,
                NetworkManager.Singleton.LocalClientId, -10);
            
            Debug.LogWarning("Player hit detected.");
            if (!HasTakenAHit)
            {
                HasTakenAHit = true;
            }
            else
            {
                // If the player has already taken a hit, we set IsDead to true
                IsDead.Value = true;
                Controller._input.enabled = false;
                // If we are playerid 0, look for player object of player 1 and vice versa
                if (NetworkManager.Singleton.SpawnManager.PlayerObjects[0].NetworkObjectId == NetworkObjectId)
                {
                    var playerObject = NetworkManager.Singleton.SpawnManager.PlayerObjects[1];
                    if (playerObject == null) return;
                    var player = playerObject.GetComponent<PlayerApplication>();
                    if (player != null)
                    {
                        _cinemachineCamera.GetComponent<CinemachineCamera>().Follow =
                            player.Model.CinemachineCameraTarget.transform;
                        player.Model.Mesh.enabled = false; // Disable the other player's mesh
                        OnDeathRpc();
                        //mute me on vivox
                        VivoxService.Instance.MuteInputDevice();
                    }
                }
                else
                {
                    var playerObject = NetworkManager.Singleton.SpawnManager.PlayerObjects[0];
                    if (playerObject == null) return;
                    var player = playerObject.GetComponent<PlayerApplication>();
                    if (player != null)
                    {
                        _cinemachineCamera.GetComponent<CinemachineCamera>().Follow =
                            player.Model.CinemachineCameraTarget.transform;
                        player.Model.Mesh.enabled = false; // Disable the other player's mesh
                        OnDeathRpc();
                    }
                }
            }
        }



        [Rpc(SendTo.Everyone)]
        private void OnDeathRpc()
        {
            Debug.LogWarning("Player has died.");
            View.SetAnimatorBoolRpc("IsDead", true);
            View.SetAnimatorBool("IsDead", true);
        }
        
        [Rpc(SendTo.Owner)]
        public void ReviveRpc()
        {
            if (!IsOwner) return;
            Debug.Log("Player has been revived.");
            HasTakenAHit = false;
            IsDead.Value = false;
            View.SetAnimatorBoolRpc("IsDead", false);
            View.SetAnimatorBool("IsDead", false);
            Controller._input.enabled = true;
            NetworkManager.SpawnManager.PlayerObjects[NetworkManager.LocalClientId == 0 ? 1 : 0].GetComponent<PlayerApplication>().Model.Mesh.enabled = true; // Re-enable the other player's mesh
            _cinemachineCamera.GetComponent<CinemachineCamera>().Follow = Model.CinemachineCameraTarget.transform; // Reset camera follow to the local player
            VivoxService.Instance.UnmuteInputDevice(); // Unmute the player on Vivox
        }
        
        [Rpc(SendTo.Owner)]
        public void OnPlayerHasKeyRpc()
        {
            if (!IsOwner) return;
            hasKey = true;
            Debug.Log("Player has the key now.");
        }
        
        [Rpc(SendTo.Everyone)]
        private void ChangeSkinRpc()
        {
            Model.Mesh.material = Model.Player2Material;
        }

        // Change spawn 
        private void TpAtSpawn()
        {
            if (!IsOwner) return;

            // TODO REPARER 

            // On récupère le composant NetworkTransform
            var networkTransform = GetComponent<AnticipatedNetworkTransform>();
            if (networkTransform != null)
                networkTransform.enabled = false; // Désactiver temporairement la synchronisation

            // Téléportation au PlayerStart
            var playerStart = FindFirstObjectByType<PlayerStart>();
            transform.position = playerStart.transform.position;
            Debug.Log($"Player téléporté à la position : {transform.position}");

            if (networkTransform != null)
                networkTransform.enabled = true; // Réactiver la synchronisation après la téléportation
        }

        private void OnCollisionEnter(Collision other)
        {
            Model.Grounded = true;
            // if we hit a target, we want to interact with it
            if (other.collider.TryGetComponent<Target>(out var target))
                target.DeactivateTarget();
        }


        [Rpc(SendTo.Everyone)]
        internal void OnClientPrepareGameClientRpc()
        {
            if (!IsLocalPlayer) return;
            if (MetagameApplication.Instance) MetagameApplication.Instance.Broadcast(new MatchEnteredEvent());
            Debug.Log("[Local client] Preparing game [Showing loading screen]");
            if (!IsServer) //the server already does this before asking clients to do the same
                CustomNetworkManager.Singleton.InstantiateGameApplication();
            transform.position = FindObjectsByType<PlayerStart>(FindObjectsSortMode.None)[0].transform.position;
            OnClientReadyToStart();
        }

        internal void OnClientReadyToStart()
        {
            Debug.Log("[Local client] Notifying server I'm ready");
            OnServerNotifiedOfClientReadinessServerRpc();
        }


        [ServerRpc]
        internal void OnServerNotifiedOfClientReadinessServerRpc()
        {
            Debug.Log("[Server] I'm ready");
            CustomNetworkManager.Singleton.OnServerPlayerIsReady(this);
        }

        [ClientRpc]
        internal void OnClientStartGameClientRpc()
        {
            if (!IsLocalPlayer) return;
            //GameApplication.Instance.Broadcast(new StartMatchEvent(false, true));
        }

        [ServerRpc]
        internal void OnPlayerAskedToWinServerRpc()
        {
            OnServerPlayerAskedToWin();
        }

        internal void OnServerPlayerAskedToWin()
        {
            GameApplication.Instance.Broadcast(new EndMatchEvent(this));
        }

        [Header("Push Settings")] public LayerMask pushLayers;
        public bool canPush;
        [Range(0.5f, 5f)] public float strength = 1.1f;

        #region Push Collider

        private void OnControllerColliderHit(ControllerColliderHit hit)
        {
            if (canPush) PushRigidBodies(hit);

            // if we hit a target, we want to interact with it
            if (hit.collider.TryGetComponent<Target>(out var target))
                target.DeactivateTarget();
        }

        private void PushRigidBodies(ControllerColliderHit hit)
        {
            // https://docs.unity3d.com/ScriptReference/CharacterController.OnControllerColliderHit.html

            // make sure we hit a non kinematic rigidbody
            var body = hit.collider.attachedRigidbody;
            if (body == null || body.isKinematic) return;

            // make sure we only push desired layer(s)
            var bodyLayerMask = 1 << body.gameObject.layer;
            if ((bodyLayerMask & pushLayers.value) == 0) return;

            // We dont want to push objects below us
            if (hit.moveDirection.y < -0.3f) return;

            // Calculate push direction from move direction, horizontal motion only
            var pushDir = new Vector3(hit.moveDirection.x, 0.0f, hit.moveDirection.z);

            // Apply the push and take strength into account
            body.AddForce(pushDir * strength, ForceMode.Impulse);
        }

        #endregion

        [Rpc(SendTo.Owner)]
        private void GetPickedUpRpc(ulong PickerObjectId)
        {
            Model.PickerTransform = NetworkManager.Singleton.SpawnManager.SpawnedObjects[PickerObjectId].transform;
            Model.SetIsPickedUpRpc(true);
        }


        public void Interact(IInteractable.InteractAction interactAction, Transform interactorTransform)
        {
            if (Model.b_IsCarryingPlayer.Value || Model.b_IsPickedUp.Value) return;

            var interactorPlayer = interactorTransform.GetComponentInParent<PlayerApplication>();

            // Prevent mutual pickup
            if (interactorPlayer.Model.b_IsPickedUp.Value || interactorPlayer.Model.b_IsCarryingPlayer.Value) return;

            // Une seule interaction possible ici
            GetPickedUpRpc(interactorPlayer.NetworkObjectId);
        }


        public Dictionary<IInteractable.InteractAction, string> GetInteractTextDictionary()
        {
            return new Dictionary<IInteractable.InteractAction, string>
            {
                { IInteractable.InteractAction.Primary, "Pickup Player" }
            };
        }

        public Transform GetTransform()
        {
            return transform;
        }

        public bool CanDoInteractAction(IInteractable.InteractAction interactAction)
        {
            return interactAction == IInteractable.InteractAction.Primary;
        }
    }
}