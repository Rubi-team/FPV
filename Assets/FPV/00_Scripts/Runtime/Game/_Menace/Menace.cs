using System;
using Audio;
using FPV;
using FPV.Runtime;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Rendering;

public enum MenaceState
{
    Roaming,
    Patrolling,
    GoingTo,
    Chasing
}

public class Menace : NetworkBehaviour
{
    private MenaceListener MenaceListener;
    private NavMeshAgent _navMeshAgent;
    
    [SerializeField] public Volume AlarmeVolume;

    [SerializeField] private MenaceState _currentState;

    [Header("State Machine")] private Transform _currentWaypoint;
    private WaypointParent _currentRoom;
    [SerializeField] private float minWaitTimeAtWaypoint = 1f;
    [SerializeField] private float maxWaitTimeAtWaypoint = 3f;
    private float _waitTimer;

    private Transform _lastTarget;
    private float _lastChargeTime;

    [Header("Charge Settings")] [SerializeField]
    private float chargeWarmupTime = 2f;

    [SerializeField] private float chargeSpeed = 5f;
    [SerializeField] private float chargeMaxDistance = 10f;
    [SerializeField] private float chargeCooldown = 10f;

    [Header("Animator")] [SerializeField] private Animator _animator;
    [SerializeField] private Transform Feet;
    [SerializeField] private LayerMask groundLayer;
    internal GroundType currentGroundType;


    internal NetworkVariable<bool> IsCharging = new();
    private bool _hasExecutedCharge = false;
    
    private float _stateEnteredTime;
    private MenaceState _previousState;
    
    private float _goingToStartTime;
    private const float GoingToTimeout = 10f;
    
    private float _lastMovedTime;
    private Vector3 _lastPosition;
    private const float TeleportIfIdleDuration = 12f;




    public static Menace Instance { get; private set; }


    private void Awake()
    {
        _navMeshAgent = GetComponent<NavMeshAgent>();
        MenaceListener = GetComponentInChildren<MenaceListener>();
        _currentState = MenaceState.Roaming;
        _lastTarget = null;
        _lastPosition = transform.position;
        _lastMovedTime = Time.time;


        Instance = this;
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        if (!NetworkManager.Singleton.IsHost)
        {
            _navMeshAgent.enabled = false; // Disable NavMeshAgent on non-server instances
            MenaceListener.enabled = false; // Disable MenaceListener on non-server instances
            this.enabled = false; // Disable this script on non-server instances
            return;
        }
    }

    private void Update()
    {
        if (!IsServer)
            return;

        if (!_navMeshAgent.enabled)
        {
            _navMeshAgent.enabled = true; // Enable NavMeshAgent if it was disabled
            return;
        }

        switch (_currentState)
        {
            case MenaceState.Roaming:
                HandleRoaming();
                break;
            case MenaceState.Patrolling:
                HandlePatrolling();
                break;
            case MenaceState.GoingTo:
                HandleGoingTo();
                break;
            case MenaceState.Chasing:
                HandleChasing();
                break;
        }
        
        // Vérifier si Menace n'a pas bougé depuis 10 secondes
        if (Vector3.Distance(transform.position, _lastPosition) > 0.1f)
        {
            _lastMovedTime = Time.time;
            _lastPosition = transform.position;
        }
        else if (Time.time - _lastMovedTime > TeleportIfIdleDuration)
        {
            var menaceStart = FindObjectOfType<MenaceStart>();
            if (menaceStart != null)
            {
                Debug.LogWarning("Menace bloquée, téléportation à MenaceStart.");
                _navMeshAgent.Warp(menaceStart.transform.position);
                _lastMovedTime = Time.time;
                _lastPosition = transform.position;
                _currentWaypoint = null;
            }
        }

    }

    private void FixedUpdate()
    {
        _animator.SetFloat("Speed", _navMeshAgent.velocity.magnitude);
    }

    private void HandleRoaming()
    {
        if (_currentWaypoint == null)
            SelectRandomWaypoint();
        else if (HasReachedCurrentWaypoint()) HandleWaypointWaiting();

        CheckForPlayerDetection();
    }

    private void SelectRandomWaypoint()
    {
        var allWaypoints = FindObjectsByType<Waypoint>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);
        if (allWaypoints.Length > 0)
        {
            _currentWaypoint = allWaypoints[UnityEngine.Random.Range(0, allWaypoints.Length)].transform;
            _navMeshAgent.SetDestination(_currentWaypoint.position);
        }
    }

    private bool HasReachedCurrentWaypoint()
    {
        return !_navMeshAgent.pathPending &&
               _navMeshAgent.remainingDistance <= _navMeshAgent.stoppingDistance;
    }

    private void HandleWaypointWaiting()
    {
        if (_waitTimer <= 0)
            ResetWaypointAndTimer();
        else
            _waitTimer -= Time.deltaTime;
    }

    private void ResetWaypointAndTimer()
    {
        _currentWaypoint = null;
        _waitTimer = UnityEngine.Random.Range(minWaitTimeAtWaypoint, maxWaitTimeAtWaypoint);
    }

    private void CheckForPlayerDetection()
    {
        if (MenaceListener.detectedPlayer != _lastTarget)
        {
            // Si un joueur est détecté, changer d'état
            _currentState = MenaceState.Chasing;
            _lastTarget = MenaceListener.detectedPlayer;
        }
    }

    private void HandlePatrolling()
    {
        if (_currentRoom == null)
        {
            // Rester dans la pièce actuelle si possible
            var nearestWaypoint = FindNearestWaypoint();
            if (nearestWaypoint != null) _currentRoom = nearestWaypoint.GetComponentInParent<WaypointParent>();
        }

        if (_currentWaypoint == null && _currentRoom != null)
        {
            // Choisir le prochain waypoint dans la pièce actuelle
            var roomWaypoints = _currentRoom.GetComponentsInChildren<Waypoint>();
            if (roomWaypoints.Length > 0)
            {
                _currentWaypoint = roomWaypoints[UnityEngine.Random.Range(0, roomWaypoints.Length)].transform;
                _navMeshAgent.SetDestination(_currentWaypoint.position);
            }
        }

        // Même logique que pour le roaming pour le changement de waypoint
        if (!_navMeshAgent.pathPending && _navMeshAgent.remainingDistance <= _navMeshAgent.stoppingDistance)
        {
            if (_waitTimer <= 0)
            {
                _currentWaypoint = null;
                _waitTimer = UnityEngine.Random.Range(minWaitTimeAtWaypoint, maxWaitTimeAtWaypoint);
            }
            else
            {
                _waitTimer -= Time.deltaTime;
            }
        }

        // Vérifier si un joueur est détecté
        if (MenaceListener.detectedPlayer != null)
        {
            _currentState = MenaceState.Chasing;
            _lastTarget = MenaceListener.detectedPlayer;
        }
    }

    private void HandleGoingTo()
    {
        if (!_navMeshAgent.pathPending && _navMeshAgent.hasPath && _navMeshAgent.remainingDistance < Mathf.Infinity)
        {
            if (_navMeshAgent.remainingDistance > 15)
            {
                _navMeshAgent.speed = 3.5f + _navMeshAgent.remainingDistance / 1.5f;
            }
            else
            {
                _navMeshAgent.speed = 3.5f;
            }

            if (_navMeshAgent.remainingDistance <= _navMeshAgent.stoppingDistance)
            {
                _currentState = MenaceState.Patrolling;
                return;
            }
        }

        // Timeout : si ça fait plus de 15s qu'on est en GoingTo, revenir à Roaming
        if (Time.time - _goingToStartTime >= GoingToTimeout)
        {
            Debug.Log("Timeout reached in GoingTo. Switching to Roaming.");
            _currentState = MenaceState.Roaming;
            return;
        }

        // Vérifier si un joueur est détecté
        if (MenaceListener.detectedPlayer != null)
        {
            _currentState = MenaceState.Chasing;
            _lastTarget = MenaceListener.detectedPlayer;
        }
    }



    private void HandleChasing()
    {
        if (_lastTarget == null)
        {
            Debug.Log("No target found in chasing state");
            _currentState = MenaceState.Patrolling;
            return;
        }

        if (_lastTarget == MenaceListener.detectedPlayer)
            Charge();

        if (IsCharging.Value && !_hasExecutedCharge && Time.time - _lastChargeTime >= chargeWarmupTime)
        {
            Debug.Log("Executing charge");
            ExecuteCharge();
            _hasExecutedCharge = true;
        }


        if (_hasExecutedCharge && !_navMeshAgent.pathPending &&
            _navMeshAgent.remainingDistance <= _navMeshAgent.stoppingDistance)
        {
            EndCharge();
            _currentState = MenaceState.Patrolling;
        }
    }

    private Waypoint FindNearestWaypoint()
    {
        Waypoint nearest = null;
        var nearestDistance = float.MaxValue;
        var waypoints = FindObjectsByType<Waypoint>(FindObjectsSortMode.None);

        foreach (var waypoint in waypoints)
        {
            var distance = Vector3.Distance(transform.position, waypoint.transform.position);
            if (distance < nearestDistance)
            {
                nearestDistance = distance;
                nearest = waypoint;
            }
        }

        return nearest;
    }

    public void SetGoingTo(Vector3 destination)
    {
        _currentState = MenaceState.GoingTo;
        _goingToStartTime = Time.time; // Enregistre le moment d'entrée dans l'état
        _navMeshAgent.SetDestination(destination);
    }


    private void Charge()
    {
        // Vérifier le cooldown et si déjà en charge
        if (Time.time - _lastChargeTime < chargeCooldown || IsCharging.Value)
            return;

        // Vérifier si on a une cible
        if (_lastTarget == null)
            return;

        StartCharge();
    }

    private void StartCharge()
    {
        _navMeshAgent.ResetPath();

        IsCharging.Value = true;
        _hasExecutedCharge = false;
        _lastChargeTime = Time.time;

        // TODO Add Visual effect
    }

    private void ExecuteCharge()
    {
        GetComponent<Rigidbody>().isKinematic = false; // Ensure the Rigidbody is not kinematic
        _animator.SetBool("IsDashing", true);
        var directionToTarget = (_lastTarget.position - transform.position).normalized;
        AudioManager.Instance.PlayOneShot(AudioManager.Instance.threatCharging, Feet.position,
            NetworkManager.Singleton.LocalClientId, -10);

        // Utiliser un Raycast pour détecter le premier obstacle dans la direction
        RaycastHit hit;
        if (Physics.Raycast(transform.position, directionToTarget, out hit))
        {
            // Utiliser le point d'impact comme destination si on touche quelque chose
            var chargeDestination = hit.point;

            // Désactiver l'évitement d'obstacles pour aller en ligne droite
            _navMeshAgent.obstacleAvoidanceType = ObstacleAvoidanceType.NoObstacleAvoidance;
            _navMeshAgent.speed = chargeSpeed;
            _navMeshAgent.SetDestination(chargeDestination);
        }
        else
        {
            // Si aucun obstacle n'est détecté, utiliser la distance maximale
            var chargeDestination = transform.position + directionToTarget * chargeMaxDistance;

            _navMeshAgent.obstacleAvoidanceType = ObstacleAvoidanceType.NoObstacleAvoidance;
            _navMeshAgent.speed = chargeSpeed;
            _navMeshAgent.SetDestination(chargeDestination);
        }
    }

    private void EndCharge()
    {
        GetComponent<Rigidbody>().isKinematic = true; // Ensure the Rigidbody is not kinematic
        IsCharging.Value = false;
        _lastChargeTime = Time.time;
        _navMeshAgent.speed = 3.5f;
        _lastTarget = null;
        _hasExecutedCharge = false;

        _animator.SetBool("IsDashing", false);
    }

    private void OnCollisionEnter(Collision other)
    {
        if (!IsServer || !IsCharging.Value || !_hasExecutedCharge) return;
        if (other.gameObject.TryGetComponent<PlayerApplication>(out var player))
        {
            
            if (player.IsDead.Value) return; // Ignore if player is dead
            
            // Calculate the direction and force to apply
            var direction = (other.transform.position - transform.position).normalized;
            var force = chargeSpeed / 2; // Adjust force multiplier as needed

            // Apply the force to the player
            player.Controller.OnPlayerThrowMeRpc(direction, force, true);
            
            player.OnPLayerHitRpc();

            // End the charge after hitting a player
            EndCharge();
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void DetectThreatServerRpc(ulong playerNetObjectId, int maxDistance)
    {
        NetworkObject networkObject = null;
        // Trouver l'objet Player à partir de son playerNetObjectId
        try
        {
            networkObject = NetworkManager.Singleton.SpawnManager.SpawnedObjects[playerNetObjectId];
        }
#pragma warning disable CS0168 // Variable is declared but never used
        catch (Exception _)
#pragma warning restore CS0168 // Variable is declared but never used
        {
            networkObject = NetworkManager.Singleton.SpawnManager.PlayerObjects[0];
        }
        
        if (networkObject == null || !networkObject.TryGetComponent(out Transform playerTransform))
        {
            Debug.LogWarning("Player object not found or does not have a Transform component.");
            return;
        }

        // Calculer la direction et vérifier la distance
        var directionToPlayer = (playerTransform.position - transform.position).normalized;
        var distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);

        if (distanceToPlayer <= maxDistance)
        {
            // Raycast pour vérifier s'il n'y a pas d'obstruction entre le Menace et le joueur
            var origin = transform.position + Vector3.up * 1f; // Ajuster la hauteur si nécessaire
            if (Physics.Raycast(origin, directionToPlayer, out var hitInfo, distanceToPlayer))
            {
                if (hitInfo.transform == playerTransform)
                    // Le joueur est visible et non obstrué
                    SetGoingTo(playerTransform.position);
                else
                    // Joueur obstrué par un obstacle
                    SetGoingTo(playerTransform.position);
            }
        }
        else
        {
            // Player too far
            return;
        }
    }

    private void GetGroundType()
    {
        var hit = Physics.OverlapSphere(Feet.position, 1, groundLayer,
            QueryTriggerInteraction.Ignore);

        if (hit[0].gameObject.TryGetComponent<GroundType>(out var groundType))
            currentGroundType = groundType;
        else
            currentGroundType = null;
    }

    public void AudioFootsteps()
    {
        GetGroundType();

        if (currentGroundType == null)
        {
            GetGroundType();

            if (currentGroundType == null)
            {
                AudioManager.Instance.PlayOneShot(AudioManager.Instance.runConcreteFootStep, Feet.position,
                    99, -10);
                return;
            }

            var index = (int)currentGroundType.groundType;
            if (currentGroundType == null) index = 0; // Default to 0 if no ground type is set

            // if controller input move is zero we return 
            if (_navMeshAgent.velocity.magnitude == 0)
                // If the player is not moving, we don't play any footsteps sound
                return;

            // switch case of index 
            switch (index)
            {
                case 0:
                    AudioManager.Instance.PlayOneShot(AudioManager.Instance.runConcreteFootStep, Feet.position,
                        NetworkManager.Singleton.LocalClientId, -10);
                    break;
                case 1:
                    AudioManager.Instance.PlayOneShot(AudioManager.Instance.runWoodFootStep, Feet.position,
                        NetworkManager.Singleton.LocalClientId, -10);
                    break;
                case 2:
                    AudioManager.Instance.PlayOneShot(AudioManager.Instance.runCarpetFootStep, Feet.position,
                        NetworkManager.Singleton.LocalClientId, -10);
                    break;
                case 3:
                    AudioManager.Instance.PlayOneShot(AudioManager.Instance.runMetalFootstep, Feet.position,
                        NetworkManager.Singleton.LocalClientId, -10);
                    break;
                default:
                    AudioManager.Instance.PlayOneShot(AudioManager.Instance.runConcreteFootStep, Feet.position,
                        NetworkManager.Singleton.LocalClientId, -10);
                    break;
            }
        }
    }
}