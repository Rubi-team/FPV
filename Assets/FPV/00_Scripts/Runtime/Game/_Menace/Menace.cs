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
    
    [Header("Charge Settings")] 
    [SerializeField] private float chargeWarmupTime = 2f;
    [SerializeField] private float chargeSpeed = 5f;
    [SerializeField] private float chargeMaxDistance = 10f;
    [SerializeField] private float chargeCooldown= 10f;
    
    

    private void Start()
    {
        _navMeshAgent = GetComponent<NavMeshAgent>();
        MenaceListener = GetComponentInChildren<MenaceListener>();
        
        _lastTarget = null;
    }

    private void Update()
    {
        if (_lastTarget != MenaceListener.detectedPlayer)
        {
            _lastTarget = MenaceListener.detectedPlayer;
            Charge();
        }
    }

    private void Charge()
    {
        
    }
}