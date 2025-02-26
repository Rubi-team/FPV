using System;
using UnityEngine;
using FMODUnity;
using Utils;

namespace FPV
{
    public class FMODEvents : BaseInstance<FMODEvents>
    {
        [field: Header("Player Controller")] 
        [field: SerializeField] public EventReference footSteep { get; private set; }
        [field: SerializeField] public EventReference jump { get; private set; }
        [field: SerializeField] public EventReference land { get; private set; }
    }
}
