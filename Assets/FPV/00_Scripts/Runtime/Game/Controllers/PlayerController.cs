using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.Netcode;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem.Composites;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

namespace FPV
{
#if ENABLE_INPUT_SYSTEM
    [RequireComponent(typeof(PlayerInput))]
#endif
    public class PlayerController : NetworkController<PlayerApplication>
    {
        internal PlayerModel Model => App.Model;

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


#if ENABLE_INPUT_SYSTEM
        internal PlayerInput _playerInput;
#endif
        internal CharacterController _controller;
        internal InputController _input;
        internal GameObject _mainCamera;

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
            if (_mainCamera == null) _mainCamera = GameObject.FindGameObjectWithTag("MainCamera");

            _controller = App.GetComponent<CharacterController>();

            // get references to our input
            _playerInput = GetComponent<PlayerInput>();
            _input = GetComponent<InputController>();
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

            GroundedCheck();

            if (Model.b_CanInteract.Value) Interact();

            if (Model.b_IsPickedUp.Value)
            {
                FollowPicker();
                return;
            }

            JumpAndGravity();
            Move();
        }

        private void LateUpdate()
        {
            if (!App.IsOwner) return;

            CameraRotation();
        }

        private void Interact()
        {
            if (!_input.interact) return;

            _input.interact = false;

            if (Model.b_IsCarryingPlayer.Value)
            {
                Model.CarriedPlayer.Controller.OnPlayerThrowMeRpc(App.transform.forward, Model.ThrowForce);
                Model.SetIsCarryingPlayerRpc(false);
                return;
            }


            var interactable = GetInteractableObject();
            if (interactable != null)
                if (interactable.GetTransform().GetComponent<PlayerApplication>() is { } player)
                    // On interagit avec un joueur
                    if (player != null)
                    {
                        if (Model.b_IsPickedUp.Value || Model.b_IsCarryingPlayer.Value)
                            return; // Already picked up or carrying someone
                        if (player.Model.b_IsPickedUp.Value || player.Model.b_IsCarryingPlayer.Value)
                            return; // Already picked up or carrying someone

                        Debug.Log($"Interact with {interactable.GetTransform().name}", this);
                        interactable.Interact(IInteractable.InteractAction.Primary, transform);

                        Model.SetIsCarryingPlayerRpc(true);
                        Model.CarriedPlayer = player;
                    }
        }

        private void GroundedCheck()
        {
            // set sphere position, with offset
            var spherePosition = new Vector3(App.transform.position.x, App.transform.position.y - Model.GroundedOffset,
                App.transform.position.z);
            Model.Grounded = Physics.CheckSphere(spherePosition, Model.GroundedRadius, Model.GroundLayers,
                QueryTriggerInteraction.Ignore);
        }

        private void CameraRotation()
        {
            // if there is an input
            if (_input.look.sqrMagnitude >= _threshold)
            {
                //Don't multiply mouse input by Time.deltaTime
                var deltaTimeMultiplier = IsCurrentDeviceMouse ? 1.0f : Time.deltaTime;

                _cinemachineTargetPitch += _input.look.y * Model.RotationSpeed * deltaTimeMultiplier;
                _rotationVelocity = _input.look.x * Model.RotationSpeed * deltaTimeMultiplier;

                // clamp our pitch rotation
                _cinemachineTargetPitch = ClampAngle(_cinemachineTargetPitch, Model.BottomClamp, Model.TopClamp);

                // Update Cinemachine camera target pitch
                Model.CinemachineCameraTarget.transform.localRotation =
                    Quaternion.Euler(_cinemachineTargetPitch, 0.0f, 0.0f);

                // rotate the player left and right
                App.transform.Rotate(Vector3.up * _rotationVelocity);
            }
        }

        private void Move()
        {
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
                // move
                inputDirection = App.transform.right * _input.move.x + App.transform.forward * _input.move.y;

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

        public IInteractable GetInteractableObject()
        {
            List<IInteractable> interactableList = new();
            var interactableHitPositionList = new List<Vector3>();
            var raycastHitArray = Physics.SphereCastAll(_mainCamera.transform.position, Model.InteractRadius,
                _mainCamera.transform.forward, Model.InteractDistance);
            foreach (var raycastHit in raycastHitArray)
                if (raycastHit.transform.TryGetComponent(out IInteractable interactable))
                {
                    interactableList.Add(interactable);
                    interactableHitPositionList.Add(raycastHit.point);
                }

            // Sort by closest
            IInteractable closestInteractable = null;
            Vector3 closestInteracableHitPosition = Vector2.zero;
            for (var i = 0; i < interactableList.Count; i++)
            {
                var interactable = interactableList[i];

                if (interactable.GetTransform() == App.transform) continue; // Ignore self

                Vector2 interactableHitPosition = interactableHitPositionList[i];
                if (closestInteractable == null)
                {
                    closestInteractable = interactable;
                    closestInteracableHitPosition = interactableHitPosition;
                }
                else
                {
                    if (Vector2.Distance(_mainCamera.transform.position, interactableHitPosition) <
                        Vector2.Distance(_mainCamera.transform.position, closestInteracableHitPosition))
                    {
                        // Closer
                        closestInteractable = interactable;
                        closestInteracableHitPosition = interactableHitPosition;
                    }
                }
            }

            return closestInteractable;
        }

        private void FollowPicker()
        {
            // Be at Model.PickerTransform position but 1 y unit above
            if (Model.PickerTransform == null)
            {
                Debug.LogError("Picker transform is null but I am picked up", this);
                return;
            }

            var targetPosition = Model.PickerTransform.position;
            targetPosition.y += 1f;
            App.transform.position = targetPosition;

            if (Model.RotateWithPicker) App.transform.rotation = Model.PickerTransform.rotation;
        }

        #region Throw

        [Rpc(SendTo.Owner)]
        internal void OnPlayerThrowMeRpc(Vector3 dir, float force)
        {
            if (!Model.b_IsPickedUp.Value)
            {
                Debug.LogError("OnPlayerThrowMeRpc called but I am not picked up", this);
                return;
            }

            if (Model.b_IsCarryingPlayer.Value)
            {
                Debug.LogError("ya 3 joueurs ??? montrez ça a thomas", this);
                return;
            }

            StartCoroutine(ThrowTrajectory(dir, force));
        }

        private IEnumerator ThrowTrajectory(Vector3 dir, float force)
        {
            var gravity = -Model.Gravity;
            var time = 0f;
            var start = App.transform.position;
            var velocity = dir * force;
            velocity.y = force * 0.5f; // donne un peu de hauteur


            while (!Model.Grounded) // TODO a fix ça bug 
            {
                time += Time.deltaTime;

                _controller.enabled = false; // Disable controller to avoid collision

                var displacement = velocity * time + 0.5f * Vector3.down * gravity * time * time;
                App.transform.position = start + displacement;

                yield return null;
            }

            // Arrivé au sol
            Model.SetIsPickedUpRpc(false);
            _controller.enabled = true; // Enable controller again
        }

        #endregion


        internal override void RemoveListeners()
        {
            // to add
        }
    }
}