using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

namespace FPV
{
    /// <summary>
    /// Main controller for the  <see cref="PlayerApplication"></see>
    /// </summary>
    public class PlayerController : Controller<PlayerApplication>
    {
        
        private CharacterController _controller;
        private InputController _input;
        private PlayerInput _playerInput;
        private float _cinemachineTargetPitch;
        private float _rotationVelocity;
        private float _speed;
        private float _verticalVelocity;
        private float _terminalVelocity = 53.0f;
        private float _jumpTimeoutDelta;
        private float _fallTimeoutDelta;
        
        internal void Init()
        {
            _controller = App.GetComponent<CharacterController>();
            _input = App.GetComponent<InputController>();
#if ENABLE_INPUT_SYSTEM
            _playerInput = App.GetComponent<PlayerInput>();
#endif
            _jumpTimeoutDelta = App.Model.JumpTimeout;
            _fallTimeoutDelta = App.Model.FallTimeout;
        }

        private void Update()
        {
            JumpAndGravity();
            GroundedCheck();
            Move();
        }

        private void LateUpdate()
        {
            CameraRotation();
        }

        private void GroundedCheck()
        {
            Vector3 spherePosition = new Vector3(App.transform.position.x,
                App.transform.position.y - App.Model.GroundedOffset, App.transform.position.z);
            App.Model.Grounded = Physics.CheckSphere(spherePosition, App.Model.GroundedRadius,
                App.Model.GroundLayers, QueryTriggerInteraction.Ignore);
        }

        private void CameraRotation()
        {
            if (_input.look.sqrMagnitude >= 0.01f)
            {
                float deltaTimeMultiplier =
                    (_playerInput?.currentControlScheme == "KeyboardMouse") ? 1.0f : Time.deltaTime;
                _cinemachineTargetPitch += _input.look.y * App.Model.RotationSpeed * deltaTimeMultiplier;
                _rotationVelocity = _input.look.x * App.Model.RotationSpeed * deltaTimeMultiplier;
                _cinemachineTargetPitch = Mathf.Clamp(_cinemachineTargetPitch, App.Model.BottomClamp,
                    App.Model.TopClamp);
                App.View.CinemachineCameraTarget.transform.localRotation =
                    Quaternion.Euler(_cinemachineTargetPitch, 0.0f, 0.0f);
                App.transform.Rotate(Vector3.up * _rotationVelocity);
            }
        }

        private void Move()
        {
            float targetSpeed = _input.sprint ? App.Model.SprintSpeed : App.Model.MoveSpeed;
            if (_input.move == Vector2.zero) targetSpeed = 0.0f;
            float currentHorizontalSpeed = new Vector3(_controller.velocity.x, 0.0f, _controller.velocity.z).magnitude;
            _speed = Mathf.Lerp(currentHorizontalSpeed, targetSpeed,
                Time.deltaTime * App.Model.SpeedChangeRate);
            Vector3 inputDirection = new Vector3(_input.move.x, 0.0f, _input.move.y).normalized;
            if (_input.move != Vector2.zero)
                inputDirection = App.transform.right * _input.move.x +
                                 App.transform.forward * _input.move.y;
            _controller.Move(inputDirection * (_speed * Time.deltaTime) +
                             new Vector3(0.0f, _verticalVelocity, 0.0f) * Time.deltaTime);
        }

        private void JumpAndGravity()
        {
            if (App.Model.Grounded)
            {
                _fallTimeoutDelta = App.Model.FallTimeout;
                if (_verticalVelocity < 0.0f)
                    _verticalVelocity = -2f;
                if (_input.jump && _jumpTimeoutDelta <= 0.0f)
                    _verticalVelocity = Mathf.Sqrt(App.Model.JumpHeight * -2f * App.Model.Gravity);
                if (_jumpTimeoutDelta >= 0.0f)
                    _jumpTimeoutDelta -= Time.deltaTime;
            }
            else
            {
                _jumpTimeoutDelta = App.Model.JumpTimeout;
                if (_fallTimeoutDelta >= 0.0f)
                    _fallTimeoutDelta -= Time.deltaTime;
                _input.jump = false;
            }

            if (_verticalVelocity < _terminalVelocity)
                _verticalVelocity += App.Model.Gravity * Time.deltaTime;
        }

        internal override void RemoveListeners()
        {
            // Implémenter la gestion des événements si nécessaire
        }
        
    }
}