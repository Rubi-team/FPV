using System;
using System.Collections.Generic;
using UnityEngine;

namespace CodeMonkey.Toolkit.TKeyDoorSystem {

    /// <summary>
    /// Added to Player to Hold keys
    /// </summary>
    public class DoorKeyHolder : MonoBehaviour {


        public event EventHandler OnDoorKeyAdded;
        public event EventHandler OnDoorKeyUsed;


        [Header("Key Holder")]
        [Tooltip("List of Keys currently being held")]
        [SerializeField] private List<Key> doorKeyHoldingList = new List<Key>();


        private void OnTriggerEnter(Collider collider) {
            DoorKey doorKey = collider.GetComponent<DoorKey>();
            if (doorKey != null) {
                doorKeyHoldingList.Add(doorKey.GetKey());
                doorKey.DestroySelf();
                OnDoorKeyAdded?.Invoke(this, EventArgs.Empty);
            }

            DoorLock doorLock = collider.GetComponent<DoorLock>();
            if (doorLock != null) {
                if (doorKeyHoldingList.Contains(doorLock.GetKey())) {
                    // Has key! Open door!
                    doorLock.OpenDoor();
                    if (doorLock.ShouldRemoveKeyOnUse()) {
                        doorKeyHoldingList.Remove(doorLock.GetKey());
                    }
                    OnDoorKeyUsed?.Invoke(this, EventArgs.Empty);
                }
            }
        }

        public List<Key> GetDoorKeyHoldingList() {
            return doorKeyHoldingList;
        }

    }

}