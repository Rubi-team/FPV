using UnityEngine;

namespace CodeMonkey.Toolkit.TGameAssets {

    /// <summary>
    /// ** GameAssets **
    /// 
    /// This is a great way to easily reference any asset you want entirely through code.
    /// Just make public fields, drag references on the Prefab, and access them through the static instance
    /// Example: GameAssets.Instance.myPrefab;
    /// 
    /// The GameAssets class also has auto-initalization, 
    /// meaning you can either place it in the scene by default, 
    /// or when the static Instance is accessed it will grab the prefab from the Resources folder.
    /// 
    /// Just make sure the prefab is named exactly "GameAssets" and is placed on a 
    /// folder named exactly "Resources", otherwise it won't work.
    /// </summary>
    public class GameAssets : MonoBehaviour {


        private static GameAssets instance;


        public static GameAssets Instance {
            get {
                if (instance == null) instance = Resources.Load<GameAssets>(nameof(GameAssets));
                if (instance == null) {
                    Debug.LogError("Could not find " + nameof(GameAssets) + "! Make sure the prefab is named exactly " + nameof(GameAssets) + " and exists inside a folder named exactly Resources!");
                }
                return instance;
            }
            private set {
                instance = value;
            }
        }



        private void Awake() {
            Instance = this;
        }


        public Sprite codeMonkeySprite;
        public Transform codeMonkeySpritePrefab;
        // Add your own fields here


    }


}