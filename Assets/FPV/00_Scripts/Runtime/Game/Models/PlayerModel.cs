using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

namespace FPV
{
    public class PlayerModel : NetworkModel<PlayerApplication>
    {
        # region PlayerController

        [Header("Player")] [Tooltip("Move speed of the character in m/s")]
        public float MoveSpeed = 4.0f;

        [Tooltip("Sprint speed of the character in m/s")]
        public float SprintSpeed = 6.0f;

        [Tooltip("Rotation speed of the character")]
        public float RotationSpeed = 1.0f;

        [Tooltip("Acceleration and deceleration")]
        public float SpeedChangeRate = 10.0f;

        [Space(10)] [Tooltip("The height the player can jump")]
        public float JumpHeight = 1.2f;

        [Tooltip("The character uses its own gravity value. The engine default is -9.81f")]
        public float Gravity = -15.0f;

        [Space(10)]
        [Tooltip("Time required to pass before being able to jump again. Set to 0f to instantly jump again")]
        public float JumpTimeout = 0.1f;

        [Tooltip("Time required to pass before entering the fall state. Useful for walking down stairs")]
        public float FallTimeout = 0.15f;

        [Header("Player Grounded")]
        [Tooltip("If the character is grounded or not. Not part of the CharacterController built in grounded check")]
        public bool Grounded = true;

        [Tooltip("Useful for rough ground")] public float GroundedOffset = -0.14f;

        [Tooltip("The radius of the grounded check. Should match the radius of the CharacterController")]
        public float GroundedRadius = 0.5f;

        [Tooltip("What layers the character uses as ground")]
        public LayerMask GroundLayers;

        [Header("Interactions")] public float InteractRadius = 1f;
        public float InteractDistance = 2f;
        public float ThrowForce = 10f;

        [Tooltip("Does the player rotate with the picker?")]
        public bool RotateWithPicker = false;

        [Header("Cinemachine")]
        [Tooltip("The follow target set in the Cinemachine Virtual Camera that the camera will follow")]
        public GameObject CinemachineCameraFollow;

        public GameObject CinemachineCameraTarget;

        [Tooltip("How far in degrees can you move the camera up")]
        public float TopClamp = 90.0f;

        [Tooltip("How far in degrees can you move the camera down")]
        public float BottomClamp = -90.0f;

        #endregion

        [Header("Runtime Values")] public Transform PickerTransform;
        public NetworkObject _NetworkObject;
        public PlayerApplication CarriedPlayer;

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
            _NetworkObject = GetComponentInParent<NetworkObject>();
        }

        // Network variables
        public NetworkVariable<bool> b_IsPickedUp = new(false, NetworkVariableReadPermission.Everyone,
            NetworkVariableWritePermission.Owner);

        public NetworkVariable<bool> b_IsCarryingPlayer = new(false, NetworkVariableReadPermission.Everyone,
            NetworkVariableWritePermission.Owner);

        public NetworkVariable<bool> b_CanInteract = new(true, NetworkVariableReadPermission.Everyone,
            NetworkVariableWritePermission.Owner);

        [Rpc(SendTo.Owner)]
        internal void SetIsPickedUpRpc(bool isPickedUp)
        {
            b_IsPickedUp.Value = isPickedUp;
        }

        [Rpc(SendTo.Owner)]
        internal void SetIsCarryingPlayerRpc(bool isCarrying)
        {
            b_IsCarryingPlayer.Value = isCarrying;
        }
    }
}