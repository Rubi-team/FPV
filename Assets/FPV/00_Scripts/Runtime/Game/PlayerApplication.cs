using System;
using System.Collections;
using System.Collections.Generic;
using FPV.Runtime.Shared;
using UnityEngine;
using Unity.Netcode;
using Unity.Netcode.Components;
using UnityEngine.InputSystem.Composites;

namespace FPV
{
    public class PlayerApplication : BaseNetworkApplication<PlayerModel, PlayerView, PlayerController>, IInteractable
    {
        protected override void Awake()
        {
            base.Awake();
            Debug.Log("PlayerApplication initialized for the local player.");
        }

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();

            OwnerCheck();
        }

        private void OwnerCheck()
        {
            if (!IsOwner)
            {
                View.Hide();

                if (Controller._playerInput != null)
                {
                    Controller._playerInput.enabled = false;
                    Controller._playerInput.DeactivateInput();
                }

                return;
            }

            View.Show();
            if (Controller._playerInput != null)
            {
                Controller._playerInput.enabled = true;
                Controller._playerInput.ActivateInput();
            }
        }


        [Rpc(SendTo.Everyone)]
        internal void OnClientPrepareGameClientRpc()
        {
            if (!IsLocalPlayer) return;
            if (MetagameApplication.Instance) MetagameApplication.Instance.Broadcast(new MatchEnteredEvent());
            Debug.Log("[Local client] Preparing game [Showing loading screen]");
            if (!IsServer) //the server already does this before asking clients to do the same
                CustomNetworkManager.Singleton.InstantiateGameApplication();
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
            if (Model.b_IsCarryingPlayer.Value) return;
            if (Model.b_IsPickedUp.Value) return;

            //TODO a changer 
            GetPickedUpRpc(interactorTransform.GetComponentInParent<NetworkObject>().NetworkObjectId);
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