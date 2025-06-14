using System;
using FPV;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.AI;

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


    private bool _isCharging = false;
    private bool _hasExecutedCharge = false;

    public static Menace Instance { get; private set; }


    private void Awake()
    {
        _navMeshAgent = GetComponent<NavMeshAgent>();
        MenaceListener = GetComponentInChildren<MenaceListener>();
        _currentState = MenaceState.Roaming;
        _lastTarget = null;

        Instance = this;

        if (!IsServer)
        {
            _navMeshAgent.enabled = false; // Disable NavMeshAgent on non-server instances
            MenaceListener.enabled = false; // Disable MenaceListener on non-server instances
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
        if (!_navMeshAgent.pathPending && _navMeshAgent.remainingDistance <= _navMeshAgent.stoppingDistance)
            // Une fois arrivé à destination, retourner au mode Roaming
            _currentState = MenaceState.Patrolling;

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

        if (_isCharging && !_hasExecutedCharge && Time.time - _lastChargeTime >= chargeWarmupTime)
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
        _navMeshAgent.SetDestination(destination);
    }

    private void Charge()
    {
        // Vérifier le cooldown et si déjà en charge
        if (Time.time - _lastChargeTime < chargeCooldown || _isCharging)
            return;

        // Vérifier si on a une cible
        if (_lastTarget == null)
            return;

        StartCharge();
    }

    private void StartCharge()
    {
        _navMeshAgent.ResetPath();

        _isCharging = true;
        _hasExecutedCharge = false;
        _lastChargeTime = Time.time;

        // TODO Add Visual effect
    }

    private void ExecuteCharge()
    {
        var directionToTarget = (_lastTarget.position - transform.position).normalized;

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
        _isCharging = false;
        _lastChargeTime = Time.time;
        _navMeshAgent.speed = 3.5f;
        _lastTarget = null;
    }

    [ServerRpc(RequireOwnership = false)]
    public void DetectThreatServerRpc(ulong playerNetObjectId, int maxDistance)
    {
        // Trouver l'objet Player à partir de son playerNetObjectId
        var networkObject = NetworkManager.Singleton.SpawnManager.SpawnedObjects[playerNetObjectId];
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
                {
                    // Le joueur est visible et non obstrué
                    SetGoingTo(playerTransform.position);
                }
                else
                {
                    // Joueur obstrué par un obstacle
                }
            }
        }
        else
        {
            Debug.Log($"Player is too far: {distanceToPlayer} units.");
        }
    }
}