using System;
using UnityEngine;
using FMODUnity;

namespace FPV
{
    public class FMODEvents : MonoBehaviour
    {
        public static FMODEvents Instance { get; private set; }

        private void Awake()
        {
            Instance = this;
        }

        [field: Header("Player Controller")] 
        [field: SerializeField] public EventReference footSteep { get; private set; }
        [field: SerializeField] public EventReference jump { get; private set; }
        [field: SerializeField] public EventReference land { get; private set; }
    }
}
