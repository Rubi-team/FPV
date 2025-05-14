using FPV;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.AI;

public class Menace : NetworkBehaviour
{
    private SoundPropagationSimulator MenaceListener;
    private NavMeshAgent _navMeshAgent;
    
    public Transform goal;

    void Start () {
        NavMeshAgent agent = GetComponent<NavMeshAgent>();
        agent.destination = goal.position;
    }
    
    
}