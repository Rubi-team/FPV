using UnityEngine;

namespace CodeMonkey.Toolkit.TGameAssets.Demo {

    public class Demo : MonoBehaviour {


        private void Update() {
            if (Input.GetMouseButtonDown(0)) {
                Debug.Log("Click");
                Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                mouseWorldPosition.z = 0f;
                Instantiate(GameAssets.Instance.codeMonkeySpritePrefab, mouseWorldPosition, Quaternion.identity);
            }
        }

    }

}