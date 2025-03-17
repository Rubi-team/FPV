using UnityEngine;

namespace CodeMonkey.Toolkit.TTopDownCharacterController {

    /// <summary>
    /// ** Top Down Character Controller 2D **
    /// 
    /// Simple Character Controller from a Top Down Perspective<color=#0f0>(no gravity)</color>
    /// Move around, optionally with a dash or roll.
    /// Interacts with Physics2D
    /// </summary>
    public class CharacterController2D : MonoBehaviour {


        private const float MOVE_SPEED = 5f;
        private const float ROLL_SPEED_START = 25f;
        private const float ROLL_SPEED_MINIMUM = 5f;
        private const float ROLL_SPEED_DROP_MULTIPLIER = 5f;
        private const float TELEPORT_AMOUNT = 5f;


        private enum State {
            Normal,
            Rolling,
        }


        [SerializeField] private LayerMask teleportLayerMask;
        [SerializeField] private bool canTeleport = true;
        [SerializeField] private bool canRoll = true;


        private Rigidbody2D characterRigidbody2D;
        private Vector3 moveDir;
        private Vector3 rollDir;
        private Vector3 lastMoveDir;
        private float rollSpeed;
        private bool isTeleportButtonDown;
        private State state;

        
        private void Awake() {
            characterRigidbody2D = GetComponent<Rigidbody2D>();
            state = State.Normal;
        }

        private void Update() {
            switch (state) {
                case State.Normal:
                    float moveX = 0f;
                    float moveY = 0f;

                    if (Input.GetKey(KeyCode.W)) {
                        moveY = +1f;
                    }
                    if (Input.GetKey(KeyCode.S)) {
                        moveY = -1f;
                    }
                    if (Input.GetKey(KeyCode.A)) {
                        moveX = -1f;
                    }
                    if (Input.GetKey(KeyCode.D)) {
                        moveX = +1f;
                    }

                    moveDir = new Vector3(moveX, moveY).normalized;
                    if (moveX != 0 || moveY != 0) {
                        // Not idle
                        lastMoveDir = moveDir;
                    }

                    if (canTeleport && Input.GetKeyDown(KeyCode.T)) {
                        // Teleport
                        isTeleportButtonDown = true;
                    }

                    if (canRoll && Input.GetKeyDown(KeyCode.Space)) {
                        // Roll
                        rollDir = lastMoveDir;
                        rollSpeed = ROLL_SPEED_START;
                        state = State.Rolling;
                    }
                    break;
                case State.Rolling:
                    rollSpeed -= rollSpeed * ROLL_SPEED_DROP_MULTIPLIER * Time.deltaTime;

                    if (rollSpeed < ROLL_SPEED_MINIMUM) {
                        state = State.Normal;
                    }
                    break;
            }
        }

        private void FixedUpdate() {
            switch (state) {
                case State.Normal:
                    characterRigidbody2D.linearVelocity = moveDir * MOVE_SPEED;

                    if (isTeleportButtonDown) {
                        // Instant teleport, doesn't go through walls
                        Vector3 teleportPosition = transform.position + lastMoveDir * TELEPORT_AMOUNT;

                        RaycastHit2D[] raycastHit2dArray = Physics2D.RaycastAll(transform.position, lastMoveDir, TELEPORT_AMOUNT, teleportLayerMask);
                        foreach (RaycastHit2D raycastHit2D in raycastHit2dArray) {
                            if (raycastHit2D.transform == transform) {
                                // Hit self, ignore
                                continue;
                            }
                            if (raycastHit2D.collider != null) {
                                teleportPosition = raycastHit2D.point;
                            }
                        }

                        characterRigidbody2D.MovePosition(teleportPosition);
                        isTeleportButtonDown = false;
                    }
                    break;
                case State.Rolling:
                    characterRigidbody2D.linearVelocity = rollDir * rollSpeed;
                    break;
            }
        }

    }

}