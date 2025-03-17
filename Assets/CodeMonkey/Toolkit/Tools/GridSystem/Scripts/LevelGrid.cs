using System;
using UnityEngine;

namespace CodeMonkey.Toolkit.TGridSystem {

    public class LevelGrid : MonoBehaviour {


        public static LevelGrid Instance { get; private set; }


        public event EventHandler OnAnyValueChanged;


        [SerializeField] private Transform gridDebugObjectPrefab;
        [SerializeField] private bool spawnGridDebugObjects;
        [SerializeField] private int width;
        [SerializeField] private int height;
        [SerializeField] private float cellSize;


        private GridSystem<GridObject> gridSystem;


        private void Awake() {
            if (Instance != null) {
                Debug.LogError("There's more than one LevelGrid! " + transform + " - " + Instance);
                Destroy(gameObject);
                return;
            }
            Instance = this;

            gridSystem = new GridSystem<GridObject>(width, height, cellSize,
                    (GridSystem<GridObject> g, GridPosition gridPosition) => new GridObject(g, gridPosition));

            if (spawnGridDebugObjects) {
                gridSystem.CreateDebugObjects(gridDebugObjectPrefab);
            }

            // Subscribe to GridObject events
            for (int x = 0; x < GetWidth(); x++) {
                for (int z = 0; z < GetHeight(); z++) {
                    GridPosition gridPosition = new GridPosition(x, z);
                    gridSystem.GetGridObject(gridPosition).OnValueChanged += (object sender, EventArgs e) => {
                        OnAnyValueChanged?.Invoke(this, EventArgs.Empty);
                    };
                }
            }
        }

        private GridSystem<GridObject> GetGridSystem() => gridSystem;

        public GridPosition GetGridPosition(Vector3 worldPosition) => GetGridSystem().GetGridPosition(worldPosition);

        public GridObject GetGridObject(Vector3 worldPosition) => GetGridSystem().GetGridObject(worldPosition);

        public GridObject GetGridObject(GridPosition gridPosition) => GetGridSystem().GetGridObject(gridPosition);

        public Vector3 GetWorldPosition(GridPosition gridPosition) => GetGridSystem().GetWorldPosition(gridPosition);

        public bool IsValidGridPosition(GridPosition gridPosition) => GetGridSystem().IsValidGridPosition(gridPosition);

        public int GetWidth() => GetGridSystem().GetWidth();

        public int GetHeight() => GetGridSystem().GetHeight();

        public void SetValue(Vector3 worldPosition, int value) => GetGridObject(worldPosition).SetValue(value);

    }

}