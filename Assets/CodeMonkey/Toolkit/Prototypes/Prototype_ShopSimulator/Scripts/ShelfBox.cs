using UnityEngine;

namespace CodeMonkey.Toolkit.ShopSimulatorDemo {

    public class ShelfBox : MonoBehaviour {


        [SerializeField] private ObjectType objectType;


        public ObjectType GetObjectType() {
            return objectType;
        }


    }

}