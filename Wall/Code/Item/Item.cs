using Microsoft.Xna.Framework.Graphics;

namespace Wall.Item {
    public class Item {

        public type itemType;
        public int count;

        public float useDelay = 1F;
        public float useTimer;

        private Texture2D texture;

        public enum type {
            iceSword, bow
        }

        public Item(type itemType, int count) {
            this.itemType = itemType;
            this.count = count;

            texture = Textures.get(itemType.ToString());
        }
    }
}