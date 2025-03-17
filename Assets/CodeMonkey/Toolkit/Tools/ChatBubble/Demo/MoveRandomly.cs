using CodeMonkey.Toolkit.TRandomData;
using UnityEngine;

namespace CodeMonkey.Toolkit.TChatBubble {

    public class MoveRandomly : MonoBehaviour {


        private enum State {
            MovingToTargetPosition,
            Waiting
        }


        private State state;
        private Vector3 targetPosition;
        private float waitTimer;


        private void Awake() {
            targetPosition = transform.position;
            state = State.MovingToTargetPosition;
        }

        private void Update() {
            switch (state) {
                default:
                case State.MovingToTargetPosition:
                    Vector3 moveDir = (targetPosition - transform.position).normalized;
                    float moveSpeed = 5f;
                    transform.position += moveDir * moveSpeed * Time.deltaTime;

                    float reachedTargetPositionDistance = .5f;
                    if (Vector3.Distance(transform.position, targetPosition) < reachedTargetPositionDistance) {
                        state = State.Waiting;
                        waitTimer = Random.Range(1, 4f);
                    }
                    break;
                case State.Waiting:
                    waitTimer -= Time.deltaTime;
                    if (waitTimer <= 0f) {
                        targetPosition = RandomData.GetRandomPositionWithinRectangle(-8, 8, -3, 1);
                        state = State.MovingToTargetPosition;
                    }
                    break;
            }
        }

    }

}