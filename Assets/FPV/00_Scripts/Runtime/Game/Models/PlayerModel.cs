using UnityEngine;
using UnityEngine.InputSystem;

namespace FPV
{
    public class PlayerModel : Model<PlayerApplication>
    {
        public float MoveSpeed = 4.0f;
        public float SprintSpeed = 6.0f;
        public float RotationSpeed = 1.0f;
        public float SpeedChangeRate = 10.0f;
        public float JumpHeight = 1.2f;
        public float Gravity = -15.0f;
        public float JumpTimeout = 0.1f;
        public float FallTimeout = 0.15f;
        public bool Grounded = true;
        public float GroundedOffset = -0.14f;
        public float GroundedRadius = 0.5f;
        public LayerMask GroundLayers;
        public float TopClamp = 90.0f;
        public float BottomClamp = -90.0f;
    }
}