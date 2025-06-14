using FPV.Runtime;
using UnityEngine;

namespace FPV._00_Scripts.Runtime.Game._Interactible
{
    public class TriggerSpaceRoom : MonoBehaviour
    {
        [SerializeField] private float NewSprintSpeed = 5.5f;
        [SerializeField] private float NewGravity = -6f;
        [SerializeField] private float NewJumpHeight = 3f;

        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent<PlayerApplication>(out var playerApplication))
            {
                // Change player properties when entering the zone
                playerApplication.Model.SprintSpeed = NewSprintSpeed;
                playerApplication.Model.Gravity = NewGravity;
                playerApplication.Model.JumpHeight = NewJumpHeight;
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.TryGetComponent<PlayerApplication>(out var playerApplication))
            {
                // Restore default player properties when leaving the zone
                playerApplication.Model.SprintSpeed = 7f; // Original SprintSpeed
                playerApplication.Model.Gravity = -15f; // Original Gravity
                playerApplication.Model.JumpHeight = 1.05f; // Original JumpHeight
            }
        }
    }
}