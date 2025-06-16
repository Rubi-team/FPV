using System;
using System.Collections.Generic;
using FPV.Runtime.Shared;
using Unity.Netcode;
using UnityEngine;

namespace FPV.Runtime
{
    public class Furby : NetworkBehaviour, IInteractable
    {
        [Header("Push Settings")]
        [SerializeField] private float explosionRadius = 5f;
        [SerializeField] private float explosionForce = 10f;
        [SerializeField] private LayerMask affectedLayers;

        [SerializeField] private GameObject TrailEffect;
        [SerializeField] private GameObject ExplosionEffect;

        private bool pickedUp = false;
        private Rigidbody rb;
        private bool hasExploded = false;
        private Transform GraphToFollow;

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
            if (IsServer)
            {
                if (!NetworkObject.IsSpawned)
                    NetworkObject.Spawn();
            }
            else if (IsClient && !IsServer)
            {
                Destroy(gameObject);
            }
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (!IsServer || hasExploded || rb == null || rb.isKinematic || !pickedUp) return;

            var contact = collision.contacts[0];
            var impactDirection = (contact.point - transform.position).normalized;
            impactDirection.y += 1.0f;
            impactDirection = impactDirection.normalized;
            var adjustedImpactPosition = contact.point + contact.normal * -0.5f;

            Explode(adjustedImpactPosition, impactDirection);
        }

        private void Explode(Vector3 explosionPosition, Vector3 direction)
        {
            if (hasExploded) return;
            hasExploded = true;

            var hits = Physics.OverlapSphere(explosionPosition, explosionRadius, affectedLayers);
            foreach (var hit in hits)
            {
                var player = hit.GetComponent<PlayerApplication>();
                if (player != null)
                {
                    float force = Mathf.Lerp(explosionForce, 0,
                        Vector3.Distance(explosionPosition, hit.transform.position) / explosionRadius);
                    player.Controller.OnPlayerThrowMeRpc(direction, force, true);
                }

                var target = hit.GetComponent<Target>();
                if (target != null) target.DeactivateTarget();

                var sonographe = hit.GetComponentInParent<Sonographe>();
                if (sonographe != null) sonographe.ActivateSonographe();
            }

            AddExplosionEffectClientRpc();
            NetworkObject.Despawn(true);
        }

        public void Interact(IInteractable.InteractAction interactAction, Transform interactorTransform)
        {
            if (interactAction != IInteractable.InteractAction.Primary) return;
            var player = interactorTransform.GetComponentInParent<PlayerApplication>();
            if (player == null) return;

            HandlePickup(player.NetworkObjectId);
        }

        private void HandlePickup(ulong playerId)
        {
            if (!IsServer) return;
            pickedUp = true;

            if (NetworkManager.Singleton.SpawnManager.SpawnedObjects.TryGetValue(playerId, out var playerObject))
            {
                GraphToFollow = playerObject.GetComponent<PlayerApplication>().Model.Graph;
                rb.isKinematic = true;

                UpdatePickupPositionClientRpc(playerId);
                RemoveSecondMaterialClientRpc();
            }
        }

        [ClientRpc]
        private void UpdatePickupPositionClientRpc(ulong pickerId)
        {
            if (NetworkManager.Singleton.SpawnManager.SpawnedObjects.TryGetValue(pickerId, out var obj))
            {
                var pickerTransform = obj.transform.GetComponent<PlayerApplication>().Model.Graph;
                transform.position = pickerTransform.position + pickerTransform.forward * 1f + Vector3.up * 1f;
            }
        }

        [ClientRpc]
        private void RemoveSecondMaterialClientRpc()
        {
            var meshRenderer = GetComponentInChildren<MeshRenderer>();
            if (meshRenderer != null && meshRenderer.materials.Length > 1)
            {
                var materials = new Material[1];
                materials[0] = meshRenderer.materials[0];
                meshRenderer.materials = materials;
            }
        }

        public void Throw(Vector3 direction, float force)
        {
            if (IsOwner && !IsServer)
                RequestThrowServerRpc(direction, force);
            else if (IsServer)
                ApplyThrow(direction, force);
        }

        [ServerRpc]
        private void RequestThrowServerRpc(Vector3 direction, float force)
        {
            ApplyThrow(direction, force);
        }

        private void ApplyThrow(Vector3 direction, float force)
        {
            pickedUp = false;
            GraphToFollow = null;

            if (rb != null)
            {
                rb.isKinematic = false;
                transform.parent = null;
                rb.AddForce(direction.normalized * force + Vector3.up * 0.5f, ForceMode.Impulse);
                AddTrailEffectClientRpc();
            }
        }

        [ClientRpc]
        private void AddTrailEffectClientRpc()
        {
            if (TrailEffect != null)
            {
                var trail = Instantiate(TrailEffect, transform);
                trail.transform.SetParent(transform);
            }
        }

        [ClientRpc]
        private void AddExplosionEffectClientRpc()
        {
            if (ExplosionEffect != null)
            {
                var explosion = Instantiate(ExplosionEffect, transform.position, Quaternion.identity);
                explosion.transform.SetParent(null);
                Destroy(explosion, 3f);
            }
        }

        private void Update()
        {
            if (!IsServer || !pickedUp || rb == null) return;

            if (GraphToFollow != null && rb.isKinematic)
            {
                transform.position = GraphToFollow.position + GraphToFollow.forward * 1f + Vector3.up * 1f;
                transform.rotation = Quaternion.LookRotation(GraphToFollow.forward, Vector3.up);
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
    }
}
