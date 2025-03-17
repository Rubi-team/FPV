using System;
using System.Collections.Generic;
using UnityEngine;

namespace CodeMonkey.Toolkit.TGridSystemHex {

    public class GridHexXZ<TGridObject> {


        private const float HEX_VERTICAL_OFFSET_MULTIPLIER = 0.75f;



        public event EventHandler<OnGridObjectChangedEventArgs> OnGridObjectChanged;
        public class OnGridObjectChangedEventArgs : EventArgs {
            public int x;
            public int z;
        }

        private int width;
        private int height;
        private float cellSize;
        private Vector3 originPosition;
        private TGridObject[,] gridArray;

        public GridHexXZ(int width, int height, float cellSize, Vector3 originPosition, Func<GridHexXZ<TGridObject>, int, int, TGridObject> createGridObject) {
            this.width = width;
            this.height = height;
            this.cellSize = cellSize;
            this.originPosition = originPosition;

            gridArray = new TGridObject[width, height];

            for (int x = 0; x < gridArray.GetLength(0); x++) {
                for (int z = 0; z < gridArray.GetLength(1); z++) {
                    gridArray[x, z] = createGridObject(this, x, z);
                }
            }

            bool showDebug = false;
            if (showDebug) {
                TextMesh[,] debugTextArray = new TextMesh[width, height];

                for (int x = 0; x < gridArray.GetLength(0); x++) {
                    for (int z = 0; z < gridArray.GetLength(1); z++) {
                        //debugTextArray[x, z] = UtilsClass.CreateWorldText(gridArray[x, z]?.ToString(), null, GetWorldPosition(x, z) + new Vector3(cellSize, 0, cellSize) * .5f, 40, Color.white, TextAnchor.MiddleCenter, TextAlignment.Center);
                        debugTextArray[x, z].transform.localScale = Vector3.one * .13f;
                        debugTextArray[x, z].transform.eulerAngles = new Vector3(90, 0, 0);
                        Debug.DrawLine(GetWorldPosition(x, z), GetWorldPosition(x, z + 1), Color.white, 100f);
                        Debug.DrawLine(GetWorldPosition(x, z), GetWorldPosition(x + 1, z), Color.white, 100f);
                    }
                }
                Debug.DrawLine(GetWorldPosition(0, height), GetWorldPosition(width, height), Color.white, 100f);
                Debug.DrawLine(GetWorldPosition(width, 0), GetWorldPosition(width, height), Color.white, 100f);

                OnGridObjectChanged += (object sender, OnGridObjectChangedEventArgs eventArgs) => {
                    debugTextArray[eventArgs.x, eventArgs.z].text = gridArray[eventArgs.x, eventArgs.z]?.ToString();
                };
            }
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

        public Vector3 GetWorldPosition(int x, int z) {
            return
                new Vector3(x, 0, 0) * cellSize +
                new Vector3(0, 0, z) * cellSize * HEX_VERTICAL_OFFSET_MULTIPLIER +
                ((Mathf.Abs(z) % 2) == 1 ? new Vector3(1, 0, 0) * cellSize * .5f : Vector3.zero) +
                originPosition;
        }

        public void GetXZ(Vector3 worldPosition, out int x, out int z) {
            int roughX = Mathf.RoundToInt((worldPosition - originPosition).x / cellSize);
            int roughZ = Mathf.RoundToInt((worldPosition - originPosition).z / cellSize / HEX_VERTICAL_OFFSET_MULTIPLIER);

            Vector3Int roughXZ = new Vector3Int(roughX, 0, roughZ);

            bool oddRow = roughZ % 2 == 1;

            List<Vector3Int> neighbourXZList = new List<Vector3Int> {
             roughXZ + new Vector3Int(-1, 0, 0),
             roughXZ + new Vector3Int(+1, 0, 0),

             roughXZ + new Vector3Int(oddRow ? +1 : -1, 0, +1),
             roughXZ + new Vector3Int(+0, 0, +1),

             roughXZ + new Vector3Int(oddRow ? +1 : -1, 0, -1),
             roughXZ + new Vector3Int(+0, 0, -1),
        };

            Vector3Int closestXZ = roughXZ;

            foreach (Vector3Int neighbourXZ in neighbourXZList) {
                if (Vector3.Distance(worldPosition, GetWorldPosition(neighbourXZ.x, neighbourXZ.z)) <
                    Vector3.Distance(worldPosition, GetWorldPosition(closestXZ.x, closestXZ.z))) {
                    // Closer than closest
                    closestXZ = neighbourXZ;
                }

            }

            x = closestXZ.x;
            z = closestXZ.z;
        }

        public void SetGridObject(int x, int z, TGridObject value) {
            if (x >= 0 && z >= 0 && x < width && z < height) {
                gridArray[x, z] = value;
                TriggerGridObjectChanged(x, z);
            }
        }

        public void TriggerGridObjectChanged(int x, int z) {
            OnGridObjectChanged?.Invoke(this, new OnGridObjectChangedEventArgs { x = x, z = z });
        }

        public void SetGridObject(Vector3 worldPosition, TGridObject value) {
            GetXZ(worldPosition, out int x, out int z);
            SetGridObject(x, z, value);
        }

        public TGridObject GetGridObject(int x, int z) {
            if (x >= 0 && z >= 0 && x < width && z < height) {
                return gridArray[x, z];
            } else {
                return default(TGridObject);
            }
        }

        public TGridObject GetGridObject(Vector3 worldPosition) {
            int x, z;
            GetXZ(worldPosition, out x, out z);
            return GetGridObject(x, z);
        }

        public Vector2Int ValidateGridPosition(Vector2Int gridPosition) {
            return new Vector2Int(
                Mathf.Clamp(gridPosition.x, 0, width - 1),
                Mathf.Clamp(gridPosition.y, 0, height - 1)
            );
        }

        public bool IsValidGridPosition(Vector2Int gridPosition) {
            int x = gridPosition.x;
            int z = gridPosition.y;

            if (x >= 0 && z >= 0 && x < width && z < height) {
                return true;
            } else {
                return false;
            }
        }

        public bool IsValidGridPositionWithPadding(Vector2Int gridPosition) {
            Vector2Int padding = new Vector2Int(2, 2);
            int x = gridPosition.x;
            int z = gridPosition.y;

            if (x >= padding.x && z >= padding.y && x < width - padding.x && z < height - padding.y) {
                return true;
            } else {
                return false;
            }
        }

    }

}