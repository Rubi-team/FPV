using UnityEngine;
using UnityEngine.InputSystem;

namespace FPV.Runtime
{
    public class InputController : Controller<GameApplication>
    {
        [Header("Character Input Values")] public Vector2 move;
        public Vector2 look;
        public bool jump;
        public bool sprint;
        public bool interact;
        public bool Emote1;
        public bool Emote2;
        public bool Emote3;
        public bool Emote4;
        public bool Emote5;
        public bool Emote6;
        public bool Emote7;
        public bool Emote8;
        public bool Emote9;

        [Header("Movement Settings")] public bool analogMovement;

        [Header("Mouse Cursor Settings")] public bool cursorLocked = true;
        public bool cursorInputForLook = true;

#if ENABLE_INPUT_SYSTEM
        public void OnMove(InputValue value)
        {
            MoveInput(value.Get<Vector2>());
        }

        public void OnLook(InputValue value)
        {
            if (cursorInputForLook) LookInput(value.Get<Vector2>());
        }

        public void OnJump(InputValue value)
        {
            JumpInput(value.isPressed);
        }

        public void OnSprint(InputValue value)
        {
            SprintInput(value.isPressed);
        }

        private InputAction interactAction;
        private InputAction emote1Action;
        private InputAction emote2Action;
        private InputAction emote3Action;
        private InputAction emote4Action;
        private InputAction emote5Action;
        private InputAction emote6Action;
        private InputAction emote7Action;
        private InputAction emote8Action;
        private InputAction emote9Action;

        private void Start()
        {
            // Récupère l'action d'interaction depuis PlayerInput
            var playerInput = GetComponent<PlayerInput>();
            if (playerInput != null)
            {
                interactAction = playerInput.actions["Interact"];
                emote1Action = playerInput.actions["Emote1"];
                emote2Action = playerInput.actions["Emote2"];
                emote3Action = playerInput.actions["Emote3"];
                emote4Action = playerInput.actions["Emote4"];
                emote5Action = playerInput.actions["Emote5"];
                emote6Action = playerInput.actions["Emote6"];
                emote7Action = playerInput.actions["Emote7"];
                emote8Action = playerInput.actions["Emote8"];
                emote9Action = playerInput.actions["Emote9"];
            }
        }

        public void ResetEmoteInputs()
        {
            Emote1 = false;
            Emote2 = false;
            Emote3 = false;
            Emote4 = false;
            Emote5 = false;
            Emote6 = false;
            Emote7 = false;
            Emote8 = false;
            Emote9 = false;
        }

        private void Update()
        {
            if (interactAction != null && interactAction.WasPressedThisFrame()) InteractInput(true);

            if (emote1Action != null && emote1Action.WasPressedThisFrame()) Emote1Input(true);
            if (emote2Action != null && emote2Action.WasPressedThisFrame()) Emote2Input(true);
            if (emote3Action != null && emote3Action.WasPressedThisFrame()) Emote3Input(true);
            if (emote4Action != null && emote4Action.WasPressedThisFrame()) Emote4Input(true);
            if (emote5Action != null && emote5Action.WasPressedThisFrame()) Emote5Input(true);
            if (emote6Action != null && emote6Action.WasPressedThisFrame()) Emote6Input(true);
            if (emote7Action != null && emote7Action.WasPressedThisFrame()) Emote7Input(true);
            if (emote8Action != null && emote8Action.WasPressedThisFrame()) Emote8Input(true);
            if (emote9Action != null && emote9Action.WasPressedThisFrame()) Emote9Input(true);
        }


#endif


        public void MoveInput(Vector2 newMoveDirection)
        {
            move = newMoveDirection;
        }

        public void LookInput(Vector2 newLookDirection)
        {
            look = newLookDirection;
        }

        public void JumpInput(bool newJumpState)
        {
            jump = newJumpState;
        }

        public void SprintInput(bool newSprintState)
        {
            sprint = newSprintState;
        }

        public void InteractInput(bool newInteractState)
        {
            interact = newInteractState;
        }

        public void Emote1Input(bool newEmoteState)
        {
            Emote1 = newEmoteState;
        }

        public void Emote2Input(bool newEmoteState)
        {
            Emote2 = newEmoteState;
        }

        public void Emote3Input(bool newEmoteState)
        {
            Emote3 = newEmoteState;
        }

        public void Emote4Input(bool newEmoteState)
        {
            Emote4 = newEmoteState;
        }

        public void Emote5Input(bool newEmoteState)
        {
            Emote5 = newEmoteState;
        }

        public void Emote6Input(bool newEmoteState)
        {
            Emote6 = newEmoteState;
        }

        public void Emote7Input(bool newEmoteState)
        {
            Emote7 = newEmoteState;
        }

        public void Emote8Input(bool newEmoteState)
        {
            Emote8 = newEmoteState;
        }

        public void Emote9Input(bool newEmoteState)
        {
            Emote9 = newEmoteState;
        }

        private void OnApplicationFocus(bool hasFocus)
        {
            SetCursorState(cursorLocked);
        }

        private void SetCursorState(bool newState)
        {
            Cursor.lockState = newState ? CursorLockMode.Locked : CursorLockMode.None;
        }

        internal override void RemoveListeners()
        {
            throw new System.NotImplementedException();
        }
    }
}