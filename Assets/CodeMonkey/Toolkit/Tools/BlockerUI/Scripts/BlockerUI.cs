using UnityEngine;

namespace CodeMonkey.Toolkit.TBlockerUI {

    /// <summary>
    /// ** Blocker UI **
    /// 
    /// Simple visual meant for blocking UI clicks
    /// Great for when you want to show something like an Input Window and block clicks behind it
    /// </summary>
    public class BlockerUI : MonoBehaviour {


        private static BlockerUI instance;


        private static void Init() {
            if (instance == null) {
                Canvas canvas = GameObject.FindFirstObjectByType<Canvas>();
                if (canvas == null) {
                    Debug.LogError("No Canvas was found in Scene! " + nameof(BlockerUI) + " needs a Canvas to work.");
                    return;
                }
                BlockerUI blockerUI = Resources.Load<BlockerUI>(nameof(BlockerUI));
                if (blockerUI == null) {
                    Debug.LogError("Could not find " + nameof(BlockerUI) + " in Resources! Is the prefab inside a folder named exactly 'Resources'? And is the prefab named exactly '" + nameof(BlockerUI) + "'?");
                    return;
                }
                instance = Instantiate(blockerUI, canvas.transform);
            }
        }



        private void Awake() {
            instance = this;

            Hide();
        }

        private void Show_Instance() {
            gameObject.SetActive(true);
            transform.SetAsLastSibling();
        }

        private void Hide_Instance() {
            gameObject.SetActive(false);
        }




        public static void Show() {
            Init();
            instance.Show_Instance();
        }

        public static void Hide() {
            Init();
            instance.Hide_Instance();
        }

    }

}
