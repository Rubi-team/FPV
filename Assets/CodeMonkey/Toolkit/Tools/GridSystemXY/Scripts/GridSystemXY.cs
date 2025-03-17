using System;
using UnityEngine;

namespace CodeMonkey.Toolkit.TGridSystemXY {

    public class GridSystemXY<TGridObject> {


        public event EventHandler<OnGridObjectChangedEventArgs> OnGridObjectChanged;
        public class OnGridObjectChangedEventArgs : EventArgs {
            public GridPosition gridPosition;
        }


        private int width;
        private int height;
        private float cellSize;
        private TGridObject[,] gridObjectArray;


        public GridSystemXY(int width, int height, float cellSize, Func<GridSystemXY<TGridObject>, GridPosition, TGridObject> createGridObject) {
            this.width = width;
            this.height = height;
            this.cellSize = cellSize;

            gridObjectArray = new TGridObject[width, height];

            for (int x = 0; x < width; x++) {
                for (int y = 0; y < height; y++) {
                    GridPosition gridPosition = new GridPosition(x, y);
                    gridObjectArray[x, y] = createGridObject(this, gridPosition);
                }
            }
        }

        public Vector3 GetWorldPosition(GridPosition gridPosition) {
            return
                new Vector3(gridPosition.x, gridPosition.y, 0) * cellSize;
        }

        public GridPosition GetGridPosition(Vector3 worldPosition) {
            return new GridPosition(
                Mathf.RoundToInt(worldPosition.x / cellSize),
                Mathf.RoundToInt(worldPosition.y / cellSize)
            );
        }

        public void CreateDebugObjects(Transform debugPrefab) {
            for (int x = 0; x < width; x++) {
                for (int y = 0; y < height; y++) {
                    GridPosition gridPosition = new GridPosition(x, y);

                    Transform debugTransform = GameObject.Instantiate(debugPrefab, GetWorldPosition(gridPosition), Quaternion.identity);
                    GridDebugObject gridDebugObject = debugTransform.GetComponent<GridDebugObject>();
                    gridDebugObject.SetGridObject(GetGridObject(gridPosition));
                }
            }
        }

        public TGridObject GetGridObject(int x, int y) {
            return gridObjectArray[x, y];
        }

        public TGridObject GetGridObject(GridPosition gridPosition) {
            return gridObjectArray[gridPosition.x, gridPosition.y];
        }

        public TGridObject GetGridObject(Vector3 worldPosition) {
            GridPosition gridPosition = GetGridPosition(worldPosition);
            return gridObjectArray[gridPosition.x, gridPosition.y];
        }

        public void TriggerGridObjectChanged(GridPosition gridPosition) {
            OnGridObjectChanged?.Invoke(this, new OnGridObjectChangedEventArgs { gridPosition = gridPosition });
        }

        public bool IsValidGridPosition(Vector3 worldPosition) {
            return IsValidGridPosition(GetGridPosition(worldPosition));
        }

        public bool IsValidGridPosition(GridPosition gridPosition) {
            return gridPosition.x >= 0 &&
                    gridPosition.y >= 0 &&
                    gridPosition.x < width &&
                    gridPosition.y < height;
        }

        public int GetWidth() {
            return width;
        }

        public int GetHeight() {
            return height;
        }

        public float GetCellSize() {
            return cellSize;
        }

    }

}