using CodeMonkey.Toolkit.TTextPopup;
using UnityEngine;

namespace CodeMonkey.Toolkit.ShopSimulatorDemo {

    public class ShopSimulatorUI : MonoBehaviour {



        private void Start() {
            Checkout.Instance.OnObjectScanned += Checkout_OnObjectScanned;
            CustomerManager.Instance.OnCustomerSpawned += CustomerManager_OnCustomerSpawned;
        }

        private void CustomerManager_OnCustomerSpawned(object sender, System.EventArgs e) {
            TextPopupUI.Create(new Vector2(0, 400), "New Customer entered!", 2f, 2f);
        }

        private void Checkout_OnObjectScanned(object sender, Checkout.OnObjectScannedEventArgs e) {
            string priceString = GameAssetsShopSimulator.Instance.GetPriceString(PriceManager.Instance.GetPrice(e.objectType));
            TextPopupUI.Create(new Vector2(0, 400), "Sold " + e.objectType + " <color=#0f0>+" + priceString + "</color>", 2f, 2f);
        }

    }
}