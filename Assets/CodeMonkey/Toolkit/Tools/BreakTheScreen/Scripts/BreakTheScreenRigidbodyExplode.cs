using UnityEngine;

namespace CodeMonkey.Toolkit.TBreakTheScreen {

    public class BreakTheScreenRigidbodyExplode : MonoBehaviour {


        [SerializeField] private Transform explodingScreenTransform;


        private void Awake() {
            TriggerExplosion();
        }

        private void TriggerExplosion() {
            Vector3 explosionPositionBase = explodingScreenTransform.position + new Vector3(0, -1f, .5f);
            Vector3 explosionPosition = explosionPositionBase +
                new Vector3(Random.Range(-.7f, +.7f), Random.Range(-.0f, +.8f), 0f);

            foreach (Transform child in transform) {
                if (child.TryGetComponent(out Rigidbody rigidbody)) {
                    rigidbody.AddExplosionForce(700f, explosionPosition, 10f);
                }
            }
        }

    }

}