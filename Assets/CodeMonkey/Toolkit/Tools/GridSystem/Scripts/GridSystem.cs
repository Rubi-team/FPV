using System;
using UnityEngine;

namespace CodeMonkey.Toolkit.TGridSystem {

    public class GridSystem<TGridObject> {


        public event EventHandler<OnGridObjectChangedEventArgs> OnGridObjectChanged;
        public class OnGridObjectChangedEventArgs : EventArgs {
            public GridPosition gridPosition;
        }


        private int width;
        private int height;
        private float cellSize;
        private TGridObject[,] gridObjectArray;
        private Vector3 origin;
        private Func<GridPosition, Vector3> customGetWorldPositionFunc;
        private Func<Vector3, GridPosition> customGetGridPositionFunc;


        public GridSystem(int width, int height, float cellSize, Func<GridSystem<TGridObject>, GridPosition, TGridObject> createGridObject, Vector3? origin = null) {
            this.width = width;
            this.height = height;
            this.cellSize = cellSize;

            if (origin != null) {
                this.origin = origin.Value;
            }

            gridObjectArray = new TGridObject[width, height];

            for (int x = 0; x < width; x++) {
                for (int z = 0; z < height; z++) {
                    GridPosition gridPosition = new GridPosition(x, z);
                    gridObjectArray[x, z] = createGridObject(this, gridPosition);
                }
            }
        }

        public void SetCustomGetWorldPositionFunc(Func<GridPosition, Vector3> customGetWorldPositionFunc) {
            this.customGetWorldPositionFunc = customGetWorldPositionFunc;
        }

        public void SetCustomGetGridPositionFunc(Func<Vector3, GridPosition> customGetGridPositionFunc) {
            this.customGetGridPositionFunc = customGetGridPositionFunc;
        }

        public Vector3 GetWorldPosition(GridPosition gridPosition) {
            if (customGetWorldPositionFunc != null) {
                return customGetWorldPositionFunc(gridPosition);
            }

            return
                new Vector3(gridPosition.x, 0, gridPosition.z) * cellSize + origin;
        }

        public GridPosition GetGridPosition(Vector3 worldPosition) {
            if (customGetGridPositionFunc != null) {
                return customGetGridPositionFunc(worldPosition);
            }

            worldPosition -= origin;
            return new GridPosition(
                Mathf.RoundToInt(worldPosition.x / cellSize),
                Mathf.RoundToInt(worldPosition.z / cellSize)
            );
        }

        public void CreateDebugObjects(Transform debugPrefab) {
            for (int x = 0; x < width; x++) {
                for (int z = 0; z < height; z++) {
                    GridPosition gridPosition = new GridPosition(x, z);

                    Transform debugTransform = GameObject.Instantiate(debugPrefab, GetWorldPosition(gridPosition), Quaternion.identity);
                    IGridDebugObject gridDebugObject = debugTransform.GetComponent<IGridDebugObject>();
                    if (gridDebugObject == null) {
                        Debug.LogError(debugTransform + " object does not have a IGridDebugObject component!");
                    }
                    gridDebugObject.SetGridObject(GetGridObject(gridPosition));
                }
            }
        }

        public TGridObject GetGridObject(GridPosition gridPosition) {
            return gridObjectArray[gridPosition.x, gridPosition.z];
        }

        public TGridObject GetGridObject(Vector3 worldPosition) {
            GridPosition gridPosition = GetGridPosition(worldPosition);
            return gridObjectArray[gridPosition.x, gridPosition.z];
        }

        public void TriggerGridObjectChanged(GridPosition gridPosition) {
            OnGridObjectChanged?.Invoke(this, new OnGridObjectChangedEventArgs { gridPosition = gridPosition });
        }

        public bool IsValidGridPosition(Vector3 worldPosition) {
            return IsValidGridPosition(GetGridPosition(worldPosition));
        }

        public bool IsValidGridPosition(GridPosition gridPosition) {
            return gridPosition.x >= 0 &&
                    gridPosition.z >= 0 &&
                    gridPosition.x < width &&
                    gridPosition.z < height;
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