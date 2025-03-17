using CodeMonkey.Toolkit.TTextPopup;
using System;
using UnityEngine;

namespace CodeMonkey.Toolkit.ShopSimulatorDemo {

    public class Checkout : MonoBehaviour {


        public static Checkout Instance { get; private set; }


        public event EventHandler<OnObjectScannedEventArgs> OnObjectScanned;
        public class OnObjectScannedEventArgs : EventArgs {
            public ObjectType objectType;
        }


        [SerializeField] private Transform customerPositionTranform;
        [SerializeField] private Transform itemPositionTransform;


        private Customer customer;
        private Transform spawnedObjectTypeTransform;
        private ObjectType spawnedObjectType;


        private void Awake() {
            Instance = this;
        }


        public Vector3 GetCustomerPosition() {
            return customerPositionTranform.position;
        }

        public bool IsFree() {
            return customer == null;
        }

        public void SetCustomer(Customer customer) {
            this.customer = customer;
        }

        public void AddObject(ObjectType objectType) {
            spawnedObjectType = objectType;
            spawnedObjectTypeTransform = Instantiate(GameAssetsShopSimulator.Instance.GetObjectTypeBoxData(objectType).boxPrefab, itemPositionTransform.position, itemPositionTransform.rotation);
        }

        public bool HasObjectWaitingToScan() {
            return spawnedObjectTypeTransform != null;
        }

        public void ScanObject() {
            string priceString = GameAssetsShopSimulator.Instance.GetPriceString(PriceManager.Instance.GetPrice(spawnedObjectType));
            TextPopupWorld.Create(itemPositionTransform.position + Vector3.up * .4f, "<color=#0f0>+" + priceString + "</color>", .03f, 2f);
            Destroy(spawnedObjectTypeTransform.gameObject);
            ObjectType scannedObjectType = spawnedObjectType;
            spawnedObjectType = ObjectType.None;
            customer.LeaveShop();

            OnObjectScanned?.Invoke(this, new OnObjectScannedEventArgs {
                objectType = scannedObjectType,
            });
        }

    }

}