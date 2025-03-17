using UnityEngine;
using CodeMonkey.Toolkit.TMousePosition;

namespace CodeMonkey.Toolkit.TGridSystemHex {

    public class Demo : MonoBehaviour {


        [SerializeField] private Transform hexPrefabTransform;


        private GridHexXZ<GridObjectHex> gridHexXZ;
        private GridObjectHex lastGridObject;


        private class GridObjectHex {

            private int x;
            private int z;
            private HexVisual hexVisual;
            private int value;

            public GridObjectHex(int x, int z) {
                this.x = x;
                this.z = z;
            }

            public void SetValue(int value) {
                this.value = value;
                hexVisual.SetText(ToString());
                hexVisual.SetRedColor();
            }

            public void SetHexVisual(HexVisual hexVisual) {
                this.hexVisual = hexVisual;
                hexVisual.SetText(ToString());
            }

            public void Show() {
                hexVisual.ShowSelected();
            }

            public void Hide() {
                hexVisual.HideSelected();
            }

            public override string ToString() {
                return "x:" + x + ", z:" + z + ", v:" + value;
            }

        }

        private void Awake() {
            int width = 10;
            int height = 10;
            float cellSize = 1f;
            gridHexXZ =
                new GridHexXZ<GridObjectHex>(width, height, cellSize, Vector3.zero, (GridHexXZ<GridObjectHex> g, int x, int z) => new GridObjectHex(x, z));

            for (int x = 0; x < width; x++) {
                for (int z = 0; z < height; z++) {
                    Transform hexVisualTransform = Instantiate(hexPrefabTransform, gridHexXZ.GetWorldPosition(x, z), Quaternion.identity);
                    gridHexXZ.GetGridObject(x, z).SetHexVisual(hexVisualTransform.GetComponent<HexVisual>());
                }
            }

        }

        private void Update() {
            if (lastGridObject != null) {
                lastGridObject.Hide();
            }

            lastGridObject = gridHexXZ.GetGridObject(MousePositionPlane.GetPosition());

            if (lastGridObject != null) {
                lastGridObject.Show();

                if (Input.GetMouseButtonDown(0)) {
                    lastGridObject.SetValue(56);
                }
            }

        }
    }

}