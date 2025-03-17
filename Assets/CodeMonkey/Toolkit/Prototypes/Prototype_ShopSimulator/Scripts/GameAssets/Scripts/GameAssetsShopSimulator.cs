using System;
using System.Collections.Generic;
using UnityEngine;

namespace CodeMonkey.Toolkit.ShopSimulatorDemo {

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
    public class GameAssetsShopSimulator : MonoBehaviour {


        private static GameAssetsShopSimulator instance;


        public static GameAssetsShopSimulator Instance {
            get {
                if (instance == null) instance = Resources.Load<GameAssetsShopSimulator>(nameof(GameAssetsShopSimulator));
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
        public List<ObjectTypeBoxData> objectTypeBoxDataList;
        public List<InteractActionSprite> interactActionSpriteList;



        [Serializable]
        public class ObjectTypeBoxData {
            public ObjectType objectType;
            public Transform boxPrefab;
            public Sprite sprite;
        }

        [Serializable]
        public class InteractActionSprite {
            public IInteractable.InteractAction interactAction;
            public Sprite sprite;
        }


        public ObjectTypeBoxData GetObjectTypeBoxData(ObjectType objectType) {
            foreach (ObjectTypeBoxData objectTypeBoxData in objectTypeBoxDataList) {
                if (objectTypeBoxData.objectType == objectType) {
                    return objectTypeBoxData;
                }
            }
            return null;
        }

        public Sprite GetIconSprite(IInteractable.InteractAction interactAction) {
            foreach (InteractActionSprite interactActionSprite in interactActionSpriteList) {
                if (interactActionSprite.interactAction == interactAction) {
                    return interactActionSprite.sprite;
                }
            }
            return null;
        }

        public string GetPriceString(int price) {
            int dollars = Mathf.FloorToInt(price / 100f);
            int cents = (int)(price - (dollars * 100f));
            return "$" + dollars + "." + cents;
        }

    }


}