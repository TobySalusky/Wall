using Microsoft.Xna.Framework;

namespace Wall {
    public class Array2DView<T> {

        private int row;
        private T[,] arr;

        public Array2DView(T[,] arr, int row) {
            this.arr = arr;
            this.row = row;
        }

        public T this[int i] {
            get => arr[i, row];
            set => arr[i, row] = value;
        }

        public int Length => arr.GetLength(0);
        
        
    }
}