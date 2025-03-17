using UnityEngine;
using CodeMonkey.Toolkit.TGridSystemXY;

namespace CodeMonkey.Toolkit.TDrawPixels {

    public class DrawPixelsVisual : MonoBehaviour {


        [SerializeField] private DrawPixels drawPixels;


        private GridSystemXY<DrawPixels.PixelGridObject> grid;
        private Mesh mesh;
        private bool updateMesh;


        private void Awake() {
            mesh = new Mesh();
            GetComponent<MeshFilter>().mesh = mesh;
        }

        private void Start() {
            SetGrid(drawPixels.GetGrid());
        }

        private void SetGrid(GridSystemXY<DrawPixels.PixelGridObject> grid) {
            this.grid = grid;
            UpdateVisual();

            grid.OnGridObjectChanged += Grid_OnGridValueChanged;
        }

        private void Grid_OnGridValueChanged(object sender, GridSystemXY<DrawPixels.PixelGridObject>.OnGridObjectChangedEventArgs e) {
            updateMesh = true;
        }

        private void LateUpdate() {
            if (updateMesh) {
                updateMesh = false;
                UpdateVisual();
            }
        }

        private void UpdateVisual() {
            MeshUtils.CreateEmptyMeshArrays(grid.GetWidth() * grid.GetHeight(), out Vector3[] vertices, out Vector2[] uv, out int[] triangles);

            for (int x = 0; x < grid.GetWidth(); x++) {
                for (int y = 0; y < grid.GetHeight(); y++) {
                    int index = x * grid.GetHeight() + y;

                    DrawPixels.PixelGridObject gridObject = grid.GetGridObject(x, y);
                    Vector3 quadSize = new Vector3(1, 1) * grid.GetCellSize();
                    Vector2 gridUV00, gridUV11;
                    gridUV00 = gridObject.GetColorUV();
                    gridUV11 = gridObject.GetColorUV();

                    MeshUtils.AddToMeshArrays(vertices, uv, triangles, index, grid.GetWorldPosition(new GridPosition(x, y)) + quadSize * .5f, 0f, quadSize, gridUV00, gridUV11);
                }
            }

            mesh.vertices = vertices;
            mesh.uv = uv;
            mesh.triangles = triangles;
        }

    }

}