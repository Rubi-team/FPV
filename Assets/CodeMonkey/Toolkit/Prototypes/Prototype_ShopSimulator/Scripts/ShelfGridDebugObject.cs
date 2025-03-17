using CodeMonkey.Toolkit.TGridSystem;
using UnityEngine;

namespace CodeMonkey.Toolkit.ShopSimulatorDemo {

    public class ShelfGridDebugObject : MonoBehaviour, IGridDebugObject {


        private Shelf.GridObject gridObject;


        public void SetGridObject(object gridObject) {
            this.gridObject = (Shelf.GridObject)gridObject;
        }

    }

}