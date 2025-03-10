using System;
using System.Collections;
using System.Collections.Generic;
using FMODUnity;
using UnityEngine;
using KinematicCharacterController;
using KinematicCharacterController.Examples;
using Unity.Netcode;
using UnityEngine.Serialization;

namespace FPV
{
    public class ExamplePlayer : NetworkBehaviour
    {
        [FormerlySerializedAs("Character")] public ExampleCharacterController CharacterToInstantiate;
        [FormerlySerializedAs("CharacterCamera")] public ExampleCharacterCamera CharacterCameratoInstantiate;

        private const string MouseXInput = "Mouse X";
        private const string MouseYInput = "Mouse Y";
        private const string MouseScrollInput = "Mouse ScrollWheel";
        private const string HorizontalInput = "Horizontal";
        private const string VerticalInput = "Vertical";
        
        public ExampleCharacterController Character; 
        public ExampleCharacterCamera CharacterCamera;
        
        private void Start()
        {
            //TODO Refact
            if (IsSpawned && !IsOwner) return;
            
            CharacterCamera = Instantiate(CharacterCameratoInstantiate, Vector3.zero, Quaternion.identity, this.transform);
            
            //CharacterCamera.GetComponent<StudioListener>().AttenuationObject = Character.gameObject;
            
            if (IsSpawned)
            {
                SpawnPlayerRPC(OwnerClientId);
            }
            
            Cursor.lockState = CursorLockMode.Locked;

            // Tell camera to follow transform
            CharacterCamera.SetFollowTransform(Character.CameraFollowPoint);

            // Ignore the character's collider(s) for camera obstruction checks
            CharacterCamera.IgnoredColliders.Clear();
            CharacterCamera.IgnoredColliders.AddRange(Character.GetComponentsInChildren<Collider>());
        }

        private void Update()
        {
            if (IsSpawned && !IsOwner) return;
            
            if (Input.GetMouseButtonDown(0))
            {
                Cursor.lockState = CursorLockMode.Locked;
            }

            HandleCharacterInput();
        }

        private void LateUpdate()
        {
            if (IsSpawned && !IsOwner) return;
            
            // Handle rotating the camera along with physics movers
            if (CharacterCamera.RotateWithPhysicsMover && Character.Motor.AttachedRigidbody != null)
            {
                CharacterCamera.PlanarDirection = Character.Motor.AttachedRigidbody.GetComponent<PhysicsMover>().RotationDeltaFromInterpolation * CharacterCameratoInstantiate.PlanarDirection;
                CharacterCamera.PlanarDirection = Vector3.ProjectOnPlane(CharacterCamera.PlanarDirection, Character.Motor.CharacterUp).normalized;
            }

            HandleCameraInput();
        }

        [Rpc(SendTo.Server)]
        private void SpawnPlayerRPC(ulong ownerId)
        {
            //TODO Refact
            var netObj =
                NetworkManager.SpawnManager.InstantiateAndSpawn(
                    CharacterToInstantiate.GetComponent<NetworkObject>(),
                    ownerId);
            
            netObj.TrySetParent(transform);
        }
        
        

        private void HandleCameraInput()
        {
            // Create the look input vector for the camera
            float mouseLookAxisUp = Input.GetAxisRaw(MouseYInput);
            float mouseLookAxisRight = Input.GetAxisRaw(MouseXInput);
            Vector3 lookInputVector = new Vector3(mouseLookAxisRight, mouseLookAxisUp, 0f);

            // Prevent moving the camera while the cursor isn't locked
            if (Cursor.lockState != CursorLockMode.Locked)
            {
                lookInputVector = Vector3.zero;
            }

            // Input for zooming the camera
            float scrollInput = -Input.GetAxis(MouseScrollInput);

            // Apply inputs to the camera
            CharacterCamera.UpdateWithInput(Time.deltaTime, scrollInput, lookInputVector);

            // Handle toggling zoom level
            if (Input.GetMouseButtonDown(1))
            {
                CharacterCamera.TargetDistance = (CharacterCamera.TargetDistance == 0f) ? CharacterCamera.DefaultDistance : 0f;
                //TODO change default to FPV
            }
        }

        private void HandleCharacterInput()
        {
            PlayerCharacterInputs characterInputs = new PlayerCharacterInputs();

            // Build the CharacterInputs struct
            characterInputs.MoveAxisForward = Input.GetAxisRaw(VerticalInput);
            characterInputs.MoveAxisRight = Input.GetAxisRaw(HorizontalInput);
            characterInputs.CameraRotation = CharacterCamera.Transform.rotation;
            characterInputs.JumpDown = Input.GetKeyDown(KeyCode.Space);
            characterInputs.CrouchDown = Input.GetKeyDown(KeyCode.C);
            characterInputs.CrouchUp = Input.GetKeyUp(KeyCode.C);

            // Apply inputs to character
            CharacterToInstantiate.SetInputs(ref characterInputs);
        }
    }
}