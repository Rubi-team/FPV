using System;
using FPV;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.AI;

public class Menace : NetworkBehaviour
{
    private MenaceListener MenaceListener;
    private NavMeshAgent _navMeshAgent;

    private Transform _lastTarget;
    private float _lastChargeTime;

    [Header("Charge Settings")] [SerializeField]
    private float chargeWarmupTime = 2f;

    [SerializeField] private float chargeSpeed = 5f;
    [SerializeField] private float chargeMaxDistance = 10f;
    [SerializeField] private float chargeCooldown = 10f;


    private bool _isCharging = false;
    private float _chargeStartTime;
    private bool _hasExecutedCharge = false;


    private void Awake()
    {
        _navMeshAgent = GetComponent<NavMeshAgent>();
        MenaceListener = GetComponentInChildren<MenaceListener>();

        _lastTarget = null;
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
        _isCharging = true;
        _chargeStartTime = Time.time;
        _hasExecutedCharge = false;


        // Vous pourriez ajouter ici des effets visuels de préparation
        // Par exemple: StartCoroutine(ChargeWarmupEffect());
    }

    private void Update()
    {
        // Vérifier si une nouvelle cible est détectée
        if (_lastTarget != MenaceListener.detectedPlayer)
        {
            _lastTarget = MenaceListener.detectedPlayer;
            Charge();
        }

        // Gestion de la charge en cours
        if (_isCharging)
        {
            var elapsed = Time.time - _chargeStartTime;

            if (elapsed < chargeWarmupTime)
            {
                // Phase de warmup
                _navMeshAgent.speed = 0;
                return;
            }

            if (!_hasExecutedCharge)
            {
                ExecuteCharge();
                _hasExecutedCharge = true;
                return; // Attendre le prochain frame pour vérifier la destination
            }

            // Vérifie si la menace est arrivée à destination
            // et s'assure que la charge a bien commencé
            if (_hasExecutedCharge && !_navMeshAgent.pathPending &&
                _navMeshAgent.remainingDistance <= _navMeshAgent.stoppingDistance)
                EndCharge();
        }
    }

    private void ExecuteCharge()
    {
        var directionToTarget = (_lastTarget.position - transform.position).normalized;
        var distanceToTarget = Vector3.Distance(transform.position, _lastTarget.position);
        var chargeDestination = transform.position + directionToTarget * Mathf.Min(distanceToTarget, chargeMaxDistance);

        Debug.Log($"Executing charge towards: {chargeDestination}, Speed: {chargeSpeed}");

        _navMeshAgent.speed = chargeSpeed;
        _navMeshAgent.SetDestination(chargeDestination);
    }

    private void EndCharge()
    {
        _isCharging = false;
        _lastChargeTime = Time.time;
        _navMeshAgent.speed = 1f;
        _lastTarget = null;
    }
}