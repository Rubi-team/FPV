using System;
using System.Collections.Generic;
using FPV.Runtime.Shared;
using Unity.Netcode;
using UnityEngine;

namespace FPV.Runtime
{
    public class Furby : NetworkBehaviour, IInteractable
    {
        [Header("Push Settings")] [SerializeField]
        private float explosionRadius = 5f;

        [SerializeField] private float explosionForce = 10f;
        [SerializeField] private LayerMask affectedLayers;

        // Synchroniser l'état picked up sur le réseau
        private NetworkVariable<bool> pickedUp = new NetworkVariable<bool>(false);
        private Rigidbody rb;
        private bool hasExploded = false;

        private void Awake()
        {
            CustomNetworkManager.Singleton.OnServerPrepareGame += Init;
            rb = GetComponent<Rigidbody>();
        }

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
            if (!IsServer) enabled = false;
        }

        public void Init()
        {
            if (!IsHost)
            {
                Destroy(gameObject);
                return;
            }

            if (!NetworkObject.IsSpawned) NetworkObject.Spawn();
        }

        private void OnCollisionEnter(Collision collision)
        {
            // Permettre la détection même si ce n'est pas le serveur mais que l'objet est owned
            if (hasExploded || rb == null || rb.isKinematic || pickedUp.Value) return;
            
            // Si on n'est pas le serveur, envoyer la collision au serveur
            if (!IsServer)
            {
                var contact = collision.contacts[0];
                var impactDirection = (contact.point - transform.position).normalized;
                impactDirection.y += 1.0f;
                impactDirection = impactDirection.normalized;
                var adjustedImpactPosition = contact.point + contact.normal * -0.5f;
                
                HandleCollisionServerRpc(adjustedImpactPosition, impactDirection);
                return;
            }

            // Code original pour le serveur
            var contactServer = collision.contacts[0];
            var impactDirectionServer = (contactServer.point - transform.position).normalized;
            impactDirectionServer.y += 1.0f;
            impactDirectionServer = impactDirectionServer.normalized;
            var adjustedImpactPositionServer = contactServer.point + contactServer.normal * -0.5f;

            ExplodeServerRpc(adjustedImpactPositionServer, impactDirectionServer);
        }

        [ServerRpc(RequireOwnership = false)]
        private void HandleCollisionServerRpc(Vector3 explosionPosition, Vector3 direction)
        {
            if (hasExploded) return;
            ExplodeServerRpc(explosionPosition, direction);
        }

        [ServerRpc(RequireOwnership = false)]
        private void ExplodeServerRpc(Vector3 explosionPosition, Vector3 direction)
        {
            if (hasExploded) return;
            hasExploded = true;

            // SphereCast pour détecter les objets à proximité
            var hits = Physics.OverlapSphere(explosionPosition, explosionRadius, affectedLayers);

            foreach (var hit in hits)
            {
                // Vérifie si c'est un joueur
                var player = hit.GetComponent<PlayerApplication>();
                if (player != null)
                {
                    // Appliquer la direction calculée avec une composante verticale
                    var force = Mathf.Lerp(explosionForce, 0,
                        Vector3.Distance(explosionPosition, hit.transform.position) / explosionRadius);
                    player.Controller.OnPlayerThrowMeRpc(direction, force, true);
                }

                // Vérifie si c'est une cible
                var target = hit.GetComponent<Target>();
                if (target != null) target.DeactivateTarget();

                // Vérifie si c'est un Sonographe
                var sonographe = hit.GetComponentInParent<Sonographe>();
                if (sonographe != null) sonographe.ActivateSonographe();
            }

            // Ajouter un effet d'explosion visuel
            AddExplosionEffectRpc();

            // Détruire ou désactiver le Furby après impact
            NetworkObject.Despawn(true);
        }

        public void Interact(IInteractable.InteractAction interactAction, Transform interactorTransform)
        {
            Debug.Log($"Furby interacted with action: {interactAction}");
            if (interactAction != IInteractable.InteractAction.Primary) return;
            Debug.Log("Furby picked up");

            var interactorPlayer = interactorTransform.GetComponentInParent<PlayerApplication>();
            if (interactorPlayer == null) return;

            // Transfer ownership to the picking player
            ChangeOwnershipServerRpc(interactorPlayer.NetworkObjectId);
            GetPickedUpRpc(interactorPlayer.NetworkObjectId);
        }

        [Rpc(SendTo.Server)]
        private void ChangeOwnershipServerRpc(ulong newOwnerId = 0, bool isThrown = false)
        {
            if (newOwnerId == 0)
            {
                // Si lancé, transférer l'ownership au serveur pour la physique
                NetworkObject.ChangeOwnership(NetworkManager.ServerClientId);
                transform.parent = null;
                GraphToFollow = null;
                
                // Mettre à jour l'état picked up
                pickedUp.Value = false;
            }
            else if (NetworkManager.Singleton.SpawnManager.SpawnedObjects.TryGetValue(newOwnerId, out var newOwnerObject))
            {
                NetworkObject.ChangeOwnership(newOwnerObject.OwnerClientId);
                if (!isThrown) 
                {
                    GetComponent<NetworkObject>().TrySetParent(newOwnerObject);
                    GraphToFollow = newOwnerObject.GetComponent<PlayerApplication>().Model.Graph;
                }
            }
        }

        private Transform GraphToFollow;

        private void Update()
        {
            // Si on est picked Up, update sa position par rapport au GraphToFollow
            if (pickedUp.Value && GraphToFollow != null && rb.isKinematic)
            {
                // Update position to be above and in front of the player
                transform.position = GraphToFollow.position +
                                     GraphToFollow.forward * 1f + Vector3.up * 1f;
                transform.rotation = Quaternion.LookRotation(GraphToFollow.forward, Vector3.up);
            }
        }

        [Rpc(SendTo.Everyone)]
        private void GetPickedUpRpc(ulong pickerObjectId)
        {
            rb.isKinematic = true;
            pickedUp.Value = true; // Utiliser NetworkVariable
            
            var pickerTransform = NetworkManager.Singleton.SpawnManager.SpawnedObjects[pickerObjectId].transform;
            transform.position = pickerTransform.GetComponent<PlayerApplication>().Model.Graph.position +
                                 pickerTransform.GetComponent<PlayerApplication>().Model.Graph.forward * 1f +
                                 Vector3.up * 1f;

            RemoveSecondMaterialRpc();
        }

        [Rpc(SendTo.Everyone)]
        private void RemoveSecondMaterialRpc()
        {
            var meshRenderer = GetComponentInChildren<MeshRenderer>();
            if (meshRenderer != null && meshRenderer.materials.Length > 1)
            {
                var materials = new Material[1];
                materials[0] = meshRenderer.materials[0];
                meshRenderer.materials = materials;
            }
        }

        public Dictionary<IInteractable.InteractAction, string> GetInteractTextDictionary()
        {
            return new Dictionary<IInteractable.InteractAction, string>
            {
                { IInteractable.InteractAction.Primary, "Pick up Furby" }
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

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, explosionRadius);
        }

        public void Throw(Vector3 direction, float force)
        {
            if (rb != null)
            {
                // D'abord changer l'ownership et l'état
                ChangeOwnershipServerRpc(0, true);
                
                // Puis appliquer la physique
                ThrowPhysicsServerRpc(direction, force);
                
                // Trail effect
                AddTrailEffectRpc();
            }
        }

        [ServerRpc(RequireOwnership = false)]
        private void ThrowPhysicsServerRpc(Vector3 direction, float force)
        {
            rb.isKinematic = false;
            rb.AddForce(direction.normalized * force + Vector3.up * 0.5f, ForceMode.Impulse);
        }

        [SerializeField] private GameObject TrailEffect;
        [SerializeField] private GameObject ExplosionEffect;

        [Rpc(SendTo.Everyone)]
        private void AddTrailEffectRpc()
        {
            if (TrailEffect != null)
            {
                var trail = Instantiate(TrailEffect, transform);
                trail.transform.SetParent(transform);
            }
        }

        [Rpc(SendTo.Everyone)]
        private void AddExplosionEffectRpc()
        {
            if (ExplosionEffect != null)
            {
                var explosion = Instantiate(ExplosionEffect, transform.position, Quaternion.identity);
                explosion.transform.SetParent(null);
                Destroy(explosion, 3f);
            }
        }
    }
}