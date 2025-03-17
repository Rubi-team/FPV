using UnityEngine;

namespace CodeMonkey.Toolkit.TFPSCounter.Demo {

    public class Demo : MonoBehaviour {


        [SerializeField] private Transform prefab;


        private float timer;


        private void Update() {
            if (Input.GetMouseButton(0)) {
                timer -= Time.deltaTime;
                if (timer <= 0f) {
                    timer += .025f;
                    Vector3 spawnPosition =
                        new Vector3(0, 3, 0) +
                        new Vector3(Random.Range(-1f, +1f), 0, Random.Range(-1f, +1f));
                    Instantiate(prefab, spawnPosition, Quaternion.identity);
                }
            }
        }

    }

}