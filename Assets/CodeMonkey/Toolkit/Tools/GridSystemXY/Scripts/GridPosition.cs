using System;

namespace CodeMonkey.Toolkit.TGridSystemXY {

    /// <summary>
    /// Represents one GridPosition with X and Y
    /// </summary>
    public struct GridPosition : IEquatable<GridPosition> {


        public int x;
        public int y;


        public GridPosition(int x, int y) {
            this.x = x;
            this.y = y;
        }

        public override bool Equals(object obj) {
            return obj is GridPosition position &&
                   x == position.x &&
                   y == position.y;
        }

        public bool Equals(GridPosition other) {
            return this == other;
        }

        public override int GetHashCode() {
            return HashCode.Combine(x, y);
        }

        public override string ToString() {
            return $"x: {x}; y: {y}";
        }

        public static bool operator ==(GridPosition a, GridPosition b) {
            return a.x == b.x && a.y == b.y;
        }

        public static bool operator !=(GridPosition a, GridPosition b) {
            return !(a == b);
        }

        public static GridPosition operator +(GridPosition a, GridPosition b) {
            return new GridPosition(a.x + b.x, a.y + b.y);
        }

        public static GridPosition operator -(GridPosition a, GridPosition b) {
            return new GridPosition(a.x - b.x, a.y - b.y);
        }

    }

}