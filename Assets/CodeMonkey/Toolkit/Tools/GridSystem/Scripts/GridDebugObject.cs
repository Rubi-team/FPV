using UnityEngine;
using TMPro;

namespace CodeMonkey.Toolkit.TGridSystem {

    public class GridDebugObject : MonoBehaviour, IGridDebugObject {


        [SerializeField] private TextMeshPro textMeshPro;


        private object gridObject;

        public virtual void SetGridObject(object gridObject) {
            this.gridObject = gridObject;
        }

        protected virtual void Update() {
            textMeshPro.text = gridObject.ToString();
        }

    }

}