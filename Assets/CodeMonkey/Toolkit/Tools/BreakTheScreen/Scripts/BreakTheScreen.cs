using CodeMonkey.Toolkit.TTakeScreenshot;
using UnityEngine;

namespace CodeMonkey.Toolkit.TBreakTheScreen {

    /// <summary>
    /// ** Break the Screen **
    /// 
    /// Fun effect where the screen shatters
    /// 
    /// It takes a screenshot, then applies that screnshot image to a bunch of meshes
    /// And explodes them with physics. Fun!
    /// </summary>
    public class BreakTheScreen : MonoBehaviour {


        private static BreakTheScreen instance;


        private static void Init() {
            if (instance == null) {
                BreakTheScreen breakTheScreen = Resources.Load<BreakTheScreen>(nameof(BreakTheScreen));
                if (breakTheScreen == null) {
                    Debug.LogError("Could not find BreakTheScreen in Resources! Is the prefab inside a folder named exactly 'Resources'? And is the prefab named exactly '" + nameof(BreakTheScreen) + "'?");
                    return;
                }
                instance = Instantiate(breakTheScreen);
            }
        }


        [SerializeField] private Material shatterMaterial;
        [SerializeField] private bool hideOnAwake = true;


        private void Awake() {
            instance = this;

            if (hideOnAwake) {
                gameObject.SetActive(false);
            }
        }


        private void Spawn_Instance() {
            if (Camera.main == null) {
                Debug.LogWarning("Camera does not exist!");
                return;
            }
            TakeScreenshot.TakeScreenshotTexture((Texture2D screenshotTexture2D) => {
                shatterMaterial.SetTexture("_BaseMap", screenshotTexture2D);

                // Hide main camera
                Camera.main.transform.gameObject.SetActive(false);

                // Enable this object to explode the pieces
                gameObject.SetActive(true);
            }, withUI: true);
        }

        public static void Spawn() {
            Init();
            instance.hideOnAwake = false;
            instance.Spawn_Instance();
        }

    }

}