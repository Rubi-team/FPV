using System;
using System.Collections.Generic;
using UnityEngine;

namespace CodeMonkey.Toolkit.ShopSimulatorDemo {

    public class PriceManager : MonoBehaviour {


        public static PriceManager Instance { get; private set; }


        public event EventHandler OnPriceChanged;


        private Dictionary<ObjectType, int> objectTypePriceDictionary;


        private void Awake() {
            Instance = this;

            objectTypePriceDictionary = new Dictionary<ObjectType, int>();

            objectTypePriceDictionary[ObjectType.Triangle] = 199;
            objectTypePriceDictionary[ObjectType.Rectangle] = 499;
            objectTypePriceDictionary[ObjectType.Circle] = 999;
        }

        public int GetPrice(ObjectType objectType) {
            return objectTypePriceDictionary[objectType];
        }

        public void SetPrice(ObjectType objectType, int price) {
            objectTypePriceDictionary[objectType] = price;

            OnPriceChanged?.Invoke(this, EventArgs.Empty);
        }


    }

}