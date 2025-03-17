using UnityEngine;
using UnityEngine.UI;

namespace CodeMonkey.Toolkit.TTakeScreenshot {

    public class Demo : MonoBehaviour {


        [SerializeField] private RawImage rawImage;


        private void Awake() {
            rawImage.gameObject.SetActive(false);
        }

        private void Update() {
            if (Input.GetKeyDown(KeyCode.T)) {
                TakeScreenshot.TakeScreenshotTexture((Texture2D texture2D) => {
                    rawImage.texture = texture2D;
                },
                Application.dataPath + "/CodeMonkey/Toolkit/Tools/TakeScreenshot/Screenshot.png");
                rawImage.gameObject.SetActive(true);
            }
            if (Input.GetKeyDown(KeyCode.Y)) {
                TakeScreenshot.TakeScreenshotTexture((Texture2D texture2D) => {
                    rawImage.texture = texture2D;
                },
                Application.dataPath + "/CodeMonkey/Toolkit/Tools/TakeScreenshot/ScreenshotNoUI.png", false);
                rawImage.gameObject.SetActive(true);
            }
        }

    }

}