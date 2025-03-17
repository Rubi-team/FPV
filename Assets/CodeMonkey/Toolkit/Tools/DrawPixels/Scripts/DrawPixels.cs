using System;
using UnityEngine;
using CodeMonkey.Toolkit.TGridSystemXY;
using CodeMonkey.Toolkit.TMousePosition;

namespace CodeMonkey.Toolkit.TDrawPixels {

    /// <summary>
    /// ** Draw Pixels **
    /// 
    /// Grid where you can draw pixels individually
    /// With this you can allow the player to draw something
    /// Perhaps draw their own player icon
    /// Or draw some visual for their weapons
    /// 
    /// You can draw the pixels and then convert them into a Texture2D image
    /// </summary>
    public class DrawPixels : MonoBehaviour {


        public static DrawPixels Instance { get; private set; }


        public event EventHandler OnColorChanged;


        public enum CursorSize {
            Small,
            Medium,
            Large
        }


        [SerializeField] private Texture2D colorsTexture;


        private GridSystemXY<PixelGridObject> gridSystem;
        private float cellSize = 1f;
        private Vector2 colorUV;
        private CursorSize cursorSize;


        private void Awake() {
            Instance = this;

            gridSystem = new GridSystemXY<PixelGridObject>(100, 100, cellSize, (GridSystemXY<PixelGridObject> g, GridPosition gridPosition) => new PixelGridObject(g, gridPosition));
            colorUV = new Vector2(0.01f, 0.01f);
            cursorSize = CursorSize.Small;
        }

        private void Update() {
            if (Input.GetMouseButton(0)) {
                // Paint on grid
                Vector3 mouseWorldPosition = MousePosition2D.GetPosition();
                int cursorSize = GetCursorSizeInt();
                for (int x = 0; x < cursorSize; x++) {
                    for (int y = 0; y < cursorSize; y++) {
                        Vector3 gridWorldPosition = mouseWorldPosition + new Vector3(x, y) * cellSize;
                        if (gridSystem.IsValidGridPosition(gridWorldPosition)) {
                            PixelGridObject pixelGridObject = gridSystem.GetGridObject(gridWorldPosition);
                            if (pixelGridObject != null) {
                                pixelGridObject.SetColorUV(colorUV);
                            }
                        }
                    }
                }

                // Color picker
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out RaycastHit raycastHit, 999f)) {
                    colorUV = raycastHit.textureCoord;
                    OnColorChanged?.Invoke(this, EventArgs.Empty);
                }
            }
        }

        public GridSystemXY<PixelGridObject> GetGrid() {
            return gridSystem;
        }

        public Vector2 GetColorUV() {
            return colorUV;
        }

        public void SetCursorSize(CursorSize cursorSize) {
            this.cursorSize = cursorSize;
        }

        private int GetCursorSizeInt() {
            switch (cursorSize) {
                default:
                case CursorSize.Small:  return 1;
                case CursorSize.Medium: return 3;
                case CursorSize.Large:  return 7;
            }
        }

        public void SaveImage(Action<Texture2D> onSaveImage) {
            Texture2D texture2D = new Texture2D(gridSystem.GetWidth(), gridSystem.GetHeight(), TextureFormat.ARGB32, false);
            texture2D.filterMode = FilterMode.Point;

            for (int x = 0; x < gridSystem.GetWidth(); x++) {
                for (int y = 0; y < gridSystem.GetHeight(); y++) {
                    PixelGridObject gridObject = gridSystem.GetGridObject(x, y);
                    Vector2 pixelCoordinates = gridObject.GetColorUV();
                    pixelCoordinates.x *= colorsTexture.width;
                    pixelCoordinates.y *= colorsTexture.height;
                    texture2D.SetPixel(x, y, colorsTexture.GetPixel((int)pixelCoordinates.x, (int)pixelCoordinates.y));
                }
            }

            texture2D.Apply();

            onSaveImage(texture2D);
        }




        public class PixelGridObject {

            private GridSystemXY<PixelGridObject> grid;
            private GridPosition gridPosition;
            private Vector2 colorUV;

            public PixelGridObject(GridSystemXY<PixelGridObject> grid, GridPosition gridPosition) {
                this.grid = grid;
                this.gridPosition = gridPosition;
            }

            public void SetColorUV(Vector2 colorUV) {
                this.colorUV = colorUV;
                TriggerGridObjectChanged();
            }

            public Vector2 GetColorUV() {
                return colorUV;
            }

            private void TriggerGridObjectChanged() {
                grid.TriggerGridObjectChanged(gridPosition);
            }

            public override string ToString() {
                return colorUV.x.ToString();
            }

        }

    }

}