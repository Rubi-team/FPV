using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Audio;
using FMODUnity;
using Unity.Netcode;
using Unity.Netcode.Components;
using Unity.Services.Lobbies.Models;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem.Composites;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

namespace FPV.Runtime
{
#if ENABLE_INPUT_SYSTEM
    [RequireComponent(typeof(PlayerInput))]
#endif
    public class PlayerController : NetworkController<PlayerApplication>
    {
        internal PlayerModel Model => App.Model;
        internal PlayerView View => App.View;

        // cinemachine
        private float _cinemachineTargetPitch;

        // player
        private float _speed;
        private float _rotationVelocity;
        private float _verticalVelocity;
        private float _terminalVelocity = 53.0f;

        // timeout deltatime
        private float _jumpTimeoutDelta;
        private float _fallTimeoutDelta;

        internal Camera mainCamera;


#if ENABLE_INPUT_SYSTEM
        internal PlayerInput _playerInput;
#endif
        internal CharacterController _controller;
        internal InputController _input;

        private const float _threshold = 0.01f;

        private bool IsCurrentDeviceMouse
        {
            get
            {
#if ENABLE_INPUT_SYSTEM
                return _playerInput.currentControlScheme == "KeyboardMouse";
#else
				return false;
#endif
            }
        }

        private void Awake()
        {
            // get a reference to our main camera

            _controller = App.GetComponent<CharacterController>();

            // get references to our input
            _playerInput = GetComponent<PlayerInput>();
            _input = GetComponent<InputController>();

            if (FindFirstObjectByType<MainCamera>())
                mainCamera = FindFirstObjectByType<MainCamera>().GetComponent<Camera>();
        }

        private void Start()
        {
            // reset our timeouts on start
            _jumpTimeoutDelta = Model.JumpTimeout;
            _fallTimeoutDelta = Model.FallTimeout;
        }


        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
        }

        private void Update()
        {
            if (!App.IsOwner) return;
            if (App.IsDead.Value) return;
            if(PauseUI.Instance != null && PauseUI.Instance.pauseMenuActive) return;

            GroundedCheck();

            if (Model.b_IsPickedUp.Value)
            {
                FollowPicker();
                return;
            }

            if (Model.b_CanInteract.Value) Interact();

            JumpAndGravity();
            Move();

            HandleEmotes();
        }

        private void HandleEmotes()
        {
            if (_input.Emote1) View.PlayEmoteRpc(1);
            if (_input.Emote2) View.PlayEmoteRpc(2);
            if (_input.Emote3) View.PlayEmoteRpc(3);
            if (_input.Emote4) View.PlayEmoteRpc(4);
            if (_input.Emote5) View.PlayEmoteRpc(5);
            if (_input.Emote6) View.PlayEmoteRpc(6);
            if (_input.Emote7) View.PlayEmoteRpc(7);
            if (_input.Emote8) View.PlayEmoteRpc(8);
            if (_input.Emote9) View.PlayEmoteRpc(9);

            // Reset le state d'emotes à false
            _input.ResetEmoteInputs();
        }


        private void LateUpdate()
        {
            if (!App.IsOwner) return;

            CameraRotation();
        }

        private void Interact()
        {
            if (!_input.interact) return;
            if (Model.b_IsPickedUp.Value) return;

            // Reset le state d'interaction à false
            _input.interact = false;

            // Gestion de l'interaction avec un joueur porté
            if (Model.b_IsCarryingPlayer.Value)
            {
                Model.CarriedPlayer.Controller.OnPlayerThrowMeRpc(Model.CinemachineCameraTarget.transform.forward,
                    Model.ThrowForce);
                Model.SetIsCarryingPlayerRpc(false);
                Model.CarriedPlayer = null; // Réinitialiser la référence au joueur porté

                // APPELER LE SON
                View.PlaySoundOnPlayerThrown();

                // ANIMATOR TRIGGER
                View.SetAnimatorTrigger("ThrowPlayer");
                return;
            }

            if (Model.b_IsCarryingFurby.Value)
            {
                // Calcule la direction à partir de la caméra principale
                var furby = Model.CarriedFurby; // Une référence au Furby tenu
                var throwForce = Model.FurbyThrowForce; // Ajouter une valeur de force dans le modèle si nécessaire
                var throwDirection = Model.CinemachineCameraTarget.transform.forward;

                furby.Throw(throwDirection, throwForce);

                // Réinitialise l'état du joueur
                Model.SetIsCarryingFurbyRpc(false);
                Model.CarriedFurby = null;

                // APPELER LE SON
                View.PlaySoundOnFurbyThrown();

                // CALL ANIMATOR TRIGGER
                View.SetAnimatorTrigger("ThrowFurby");
                return;
            }

            // Obtenez l'objet interactable
            var interactable = GetInteractableObject();

            // Si aucun interactable n'est trouvé ou s'il est null/détruit, sortez immédiatement
            if (interactable == null || interactable.GetTransform() == null)
            {
                Debug.LogWarning("Aucun objet interactable valide trouvé.");
                return;
            }


            // Interaction avec un autre joueur si applicable
            if (interactable.GetTransform().GetComponent<PlayerApplication>() is { } player)
            {
                if (player.IsDead.Value)
                {
                    player.ReviveRpc();
                    return;
                }
                // Empêchez l'interaction si l'état ne le permet pas
                if (player == null || Model.b_IsPickedUp.Value || Model.b_IsCarryingPlayer.Value ||
                    player.Model.b_IsPickedUp.Value || player.Model.b_IsCarryingPlayer.Value)
                    return;

                Debug.Log($"Interagir avec : {interactable.GetTransform().name}", this);
                interactable.Interact(IInteractable.InteractAction.Primary, App.transform);

                Model.SetIsCarryingPlayerRpc(true);
                Model.CarriedPlayer = player;

                // APPELER LE SON
                View.PlaySoundOnPlayerPickedUp();

                // CALL ANIMATOR TRIGGER
                View.SetAnimatorTrigger("GrabPlayer");
            }
            else
            {
                interactable.Interact(IInteractable.InteractAction.Primary, App.transform);
                Model.SetIsCarryingFurbyRpc(true);
                Model.CarriedFurby = interactable.GetTransform().GetComponent<Furby>();

                // APPELER LE SON
                View.PlaySoundOnFurbyPickedUp();

                // CALL ANIMATOR TRIGGER
                View.SetAnimatorTrigger("GrabFurby");
            }
        }

        private void GroundedCheck()
        {
            // set sphere position, with offset
            var spherePosition = new Vector3(App.transform.position.x, App.transform.position.y - Model.GroundedOffset,
                App.transform.position.z);
            Model.Grounded = Physics.CheckSphere(spherePosition, Model.GroundedRadius, Model.GroundLayers,
                QueryTriggerInteraction.Ignore);

            // Set View.CurrentGroundedState to the floor i Hit getting Ground Component\
            if (Model.Grounded)
            {
                // Get the ground component
                var hit = Physics.OverlapSphere(spherePosition, Model.GroundedRadius, Model.GroundLayers,
                    QueryTriggerInteraction.Ignore);

                if (hit[0].gameObject.TryGetComponent<GroundType>(out var groundType))
                    View.currentGroundType = groundType;
                else
                    View.currentGroundType = null;
            }
        }

        private void CameraRotation()
        {
            if (_input.look.sqrMagnitude >= _threshold)
            {
                var deltaTimeMultiplier = IsCurrentDeviceMouse ? 1.0f : Time.deltaTime;

                _cinemachineTargetPitch += _input.look.y * Model.RotationSpeed * deltaTimeMultiplier * PauseUI.Instance.sensitivity;
                _rotationVelocity += _input.look.x * Model.RotationSpeed * deltaTimeMultiplier * PauseUI.Instance.sensitivity;

                // Clamp pitch
                _cinemachineTargetPitch = ClampAngle(_cinemachineTargetPitch, Model.BottomClamp, Model.TopClamp);

                // Applique les deux axes à la target
                var targetRotation = Quaternion.Euler(_cinemachineTargetPitch, _rotationVelocity, 0.0f);
                Model.CinemachineCameraTarget.transform.localRotation =
                    Quaternion.Slerp(Model.CinemachineCameraTarget.transform.localRotation, targetRotation,
                        Time.deltaTime * 20f);
            }
        }


        private Vector3 _throwVelocity = Vector3.zero;
        private bool _isBeingThrown = false;

        [Rpc(SendTo.Owner)]
        internal void OnPlayerThrowMeRpc(Vector3 dir, float force, bool calledByFurby = false)
        {
            if (!Model.b_IsPickedUp.Value && !calledByFurby)
            {
                Debug.LogError("OnPlayerThrowMeRpc called but I am not picked up", this);
                return;
            }

            // If called by a furby, teleport the player 0.2f units above the ground
            if (calledByFurby)
            {
                var groundPosition = App.transform.position;
                groundPosition.y += 0.2f; // Adjust the height above the ground
                App.transform.position = groundPosition;
            }

            // Initialiser la vélocité du lancer
            _throwVelocity = dir * force;
            _throwVelocity.y = force;
            _isBeingThrown = true;

            // Libérer immédiatement le joueur
            Model.SetIsPickedUpRpc(false);

            // Add a TRail 
            View.AddTrailEffectRpc();
        }

        private void Move()
        {
            if (Model.b_IsPickedUp.Value)
                return;

            if (_isBeingThrown)
            {
                // Appliquer la gravité à la vélocité du lancer
                _throwVelocity.y += Model.Gravity * Time.deltaTime;

                // Déplacer le joueur
                _controller.Move(_throwVelocity * Time.deltaTime);

                // Si on touche le sol, arrêter le lancer
                if (Model.Grounded)
                {
                    _isBeingThrown = false;
                    _throwVelocity = Vector3.zero;
                }

                return;
            }

            // set target speed based on move speed, sprint speed and if sprint is pressed
            var targetSpeed = _input.sprint ? Model.SprintSpeed : Model.MoveSpeed;

            // a simplistic acceleration and deceleration designed to be easy to remove, replace, or iterate upon

            // note: Vector2's == operator uses approximation so is not floating point error prone, and is cheaper than magnitude
            // if there is no input, set the target speed to 0
            if (_input.move == Vector2.zero) targetSpeed = 0.0f;

            // a reference to the players current horizontal velocity
            var currentHorizontalSpeed = new Vector3(_controller.velocity.x, 0.0f, _controller.velocity.z).magnitude;

            var speedOffset = 0.1f;
            var inputMagnitude = _input.analogMovement ? _input.move.magnitude : 1f;

            // accelerate or decelerate to target speed
            if (currentHorizontalSpeed < targetSpeed - speedOffset ||
                currentHorizontalSpeed > targetSpeed + speedOffset)
            {
                // creates curved result rather than a linear one giving a more organic speed change
                // note T in Lerp is clamped, so we don't need to clamp our speed
                _speed = Mathf.Lerp(currentHorizontalSpeed, targetSpeed * inputMagnitude,
                    Time.deltaTime * Model.SpeedChangeRate);

                // round speed to 3 decimal places
                _speed = Mathf.Round(_speed * 1000f) / 1000f;
            }
            else
            {
                _speed = targetSpeed;
            }

            // normalise input direction
            var inputDirection = new Vector3(_input.move.x, 0.0f, _input.move.y).normalized;

            // note: Vector2's != operator uses approximation so is not floating point error prone, and is cheaper than magnitude
            // if there is a move input rotate player when the player is moving
            if (_input.move != Vector2.zero)
            {
                var forward = mainCamera.transform.forward;
                var right = mainCamera.transform.right;

                forward.y = 0f; // On ne veut pas que le joueur saute quand on regarde en l'air
                right.y = 0f;

                forward.Normalize();
                right.Normalize();

                inputDirection = (right * _input.move.x + forward * _input.move.y).normalized;
            }

            if (inputDirection != Vector3.zero)
            {
                var targetRotation = Quaternion.LookRotation(inputDirection, Vector3.up);
                var euler = targetRotation.eulerAngles;
                euler.x = 0f;
                euler.z = 0f;
                targetRotation = Quaternion.Euler(euler);

                var t = Time.deltaTime / 0.2f;
                Model.Graph.rotation = Quaternion.Slerp(Model.Graph.rotation, targetRotation, t);
            }


            // move the player
            _controller.Move(inputDirection.normalized * (_speed * Time.deltaTime) +
                             new Vector3(0.0f, _verticalVelocity, 0.0f) * Time.deltaTime);
        }

        private void JumpAndGravity()
        {
            if (Model.Grounded)
            {
                // reset the fall timeout timer
                _fallTimeoutDelta = Model.FallTimeout;

                // stop our velocity dropping infinitely when grounded
                if (_verticalVelocity < 0.0f) _verticalVelocity = -2f;

                // Jump
                if (_input.jump && _jumpTimeoutDelta <= 0.0f)
                    // the square root of H * -2 * G = how much velocity needed to reach desired height
                    _verticalVelocity = Mathf.Sqrt(Model.JumpHeight * -2f * Model.Gravity);

                // jump timeout
                if (_jumpTimeoutDelta >= 0.0f) _jumpTimeoutDelta -= Time.deltaTime;
            }
            else
            {
                // reset the jump timeout timer
                _jumpTimeoutDelta = Model.JumpTimeout;

                // fall timeout
                if (_fallTimeoutDelta >= 0.0f) _fallTimeoutDelta -= Time.deltaTime;

                // if we are not grounded, do not jump
                _input.jump = false;
            }

            // apply gravity over time if under terminal (multiply by delta time twice to linearly speed up over time)
            if (_verticalVelocity < _terminalVelocity) _verticalVelocity += Model.Gravity * Time.deltaTime;
        }

        private static float ClampAngle(float lfAngle, float lfMin, float lfMax)
        {
            if (lfAngle < -360f) lfAngle += 360f;
            if (lfAngle > 360f) lfAngle -= 360f;
            return Mathf.Clamp(lfAngle, lfMin, lfMax);
        }

        public IInteractable GetInteractableObject()
        {
            List<IInteractable> interactableList = new();
            var interactableHitPositionList = new List<Vector3>();

            var raycastHitArray = Physics.SphereCastAll(
                mainCamera.transform.position,
                Model.InteractRadius,
                mainCamera.transform.forward,
                Model.InteractDistance
            );

            foreach (var raycastHit in raycastHitArray)
            {
                // Vérifiez si le transform est null
                if (raycastHit.transform == null)
                    continue;

                // Essayez de récupérer l'interactable uniquement si le transform n'est pas null
                if (raycastHit.transform.TryGetComponent(out IInteractable interactable))
                    // Assurez-vous que l'objet interactable n'est pas null ou détruit
                    if (interactable != null && interactable.GetTransform() != null)
                    {
                        interactableList.Add(interactable);
                        interactableHitPositionList.Add(raycastHit.point);
                    }
            }

            // Trier les objets interactables par proximité
            IInteractable closestInteractable = null;
            var closestInteracableHitPosition = Vector3.zero;

            for (var i = 0; i < interactableList.Count; i++)
            {
                var interactable = interactableList[i];

                // Ignorez les références à soi-même
                if (interactable.GetTransform() == App.transform)
                    continue;

                var interactableHitPosition = interactableHitPositionList[i];

                if (closestInteractable == null ||
                    Vector3.Distance(mainCamera.transform.position, interactableHitPosition) <
                    Vector3.Distance(mainCamera.transform.position, closestInteracableHitPosition))
                {
                    // Déterminer l'interactable le plus proche
                    closestInteractable = interactable;
                    closestInteracableHitPosition = interactableHitPosition;
                }
            }

            return closestInteractable;
        }

        private void FollowPicker()
        {
            // Always check if the picker transform is not picked up too, if so depickup
            if(Model.PickerTransform.GetComponent<PlayerApplication>().Model.b_IsPickedUp.Value)
            {
                Model.SetIsPickedUpRpc(false);
                return;
            }
            
            // Be at Model.PickerTransform position but 1 y unit above
            if (Model.PickerTransform == null)
            {
                Debug.LogError("Picker transform is null but I am picked up", this);
                return;
            }

            var targetPosition = Model.PickerTransform.position;
            targetPosition.y += 1f;
            App.transform.position = targetPosition;

            if (Model.RotateWithPicker) Model.Graph.transform.rotation = Model.PickerTransform.rotation;
        }


        internal override void RemoveListeners()
        {
            // to add
        }

        private void OnDrawGizmosSelected()
        {
            var transparentGreen = new Color(0.0f, 1.0f, 0.0f, 0.35f);
            var transparentRed = new Color(1.0f, 0.0f, 0.0f, 0.35f);

            if (Model.Grounded) Gizmos.color = transparentGreen;
            else Gizmos.color = transparentRed;

            // when selected, draw a gizmo in the position of, and matching radius of, the grounded collider
            Gizmos.DrawSphere(
                new Vector3(App.transform.position.x, App.transform.position.y - Model.GroundedOffset,
                    App.transform.position.z),
                Model.GroundedRadius);
        }

        private void OnDrawGizmos()
        {
            var spherePosition = Model.CinemachineCameraTarget.transform.position +
                                 Model.CinemachineCameraTarget.transform.forward * Model.InteractDistance;
            Gizmos.DrawSphere(spherePosition, Model.InteractRadius);
        }
    }
}