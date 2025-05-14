using FPV;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.AI;

public class Menace : NetworkBehaviour
{
    private SoundPropagationSimulator MenaceListener;
    private NavMeshAgent _navMeshAgent;
}