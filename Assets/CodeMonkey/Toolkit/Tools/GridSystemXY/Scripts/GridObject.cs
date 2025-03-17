using System;

namespace CodeMonkey.Toolkit.TGridSystemXY {

    public class GridObject {


        public event EventHandler OnValueChanged;


        private GridSystemXY<GridObject> gridSystem;
        private GridPosition gridPosition;
        // Add more fields here to store whatever data you want on each GridObject
        private int value;


        public GridObject(GridSystemXY<GridObject> gridSystem, GridPosition gridPosition) {
            this.gridSystem = gridSystem;
            this.gridPosition = gridPosition;
        }

        public override string ToString() {
            return gridPosition.ToString() + "; v:" + value;
        }

        public void SetValue(int value) {
            this.value = value;
            OnValueChanged?.Invoke(this, EventArgs.Empty);
        }

        public int GetValue() {
            return value;
        }

    }

}