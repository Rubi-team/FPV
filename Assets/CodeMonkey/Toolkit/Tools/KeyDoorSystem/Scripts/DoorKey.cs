using UnityEngine;

namespace CodeMonkey.Toolkit.TKeyDoorSystem {

    /// <summary>
    /// Added to Key prefab, holds reference of the Key object
    /// </summary>
    public class DoorKey : MonoBehaviour {


        [Header("Door Key")]
        [Tooltip("The Key Scriptable Object")]
        [SerializeField] private Key key;


        public void DestroySelf() {
            // Destroy this Key
            Destroy(gameObject);
        }

        public Key GetKey() {
            return key;
        }

    }

}