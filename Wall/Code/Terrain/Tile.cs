using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Wall {
    public class Tile {

        public readonly type tileType;
        public readonly Vector2 pos; // marks top-left location
        
        public Texture2D texture;
        private Rectangle atlasRect; // TODO:

        public const int pixelCount = 8;
        public const float pixelSize = 1/8F;

        public enum type {
            air, snow, ice
        }

        public static Dictionary<Color, int> genTable() {
            var table = new Dictionary<Color, int>();

            tableAdd(table, Color.White, type.snow);
            tableAdd(table, Color.Red, type.ice);
            
            return table;
        }

        private static void tableAdd(Dictionary<Color, int> dict, Color color, type tileType) {
            dict[color] = (int) tileType;
        }

        public Tile(type tileType, Vector2 pos) {

            this.tileType = tileType;
            this.pos = pos;

            //texture = findTexture(tileType);
        }

        public bool isSolid() {
            return tileType != type.air;
        }

        public void findTexture() {

            texture = (tileType == type.air) ? null : Textures.get(tileType.ToString());

            if (texture != null && texture.Width * texture.Height > 1)
                findAtlasRect(findAtlasIndex());
        }
        
        public void render(Camera camera, SpriteBatch spriteBatch) { // TODO: make more efficient
            
            if (tileType == type.air) return;
            
            var (x, y) = camera.toScreen(pos);
            int size = (int) camera.scale;
            Rectangle rect = new Rectangle((int) x, (int) y, size, size);
                
            spriteBatch.Draw(texture, rect, atlasRect, Color.White);
        }

        private void findAtlasRect(int index) {
            int rows = 3, cols = 3;

            int col = index % cols;
            int row = index / cols;

            int size = texture.Width / cols; // assumes square blocks

            atlasRect = new Rectangle(size * col, size * row, size, size);
        }

        private int findAtlasIndex() {
            if (airAbove() && !airBelow() && airLeft() && !airRight())
                return 0;
            if (airAbove() && !airBelow() && !airLeft() && !airRight())
                return 1;
            if (airAbove() && !airBelow() && !airLeft() && airRight())
                return 2;
            if (!airAbove() && !airBelow() && airLeft() && !airRight())
                return 3;
            if (!airAbove() && !airBelow() && !airLeft() && airRight())
                return 5;
            if (!airAbove() && airBelow() && airLeft() && !airRight())
                return 6;
            if (!airAbove() && airBelow() && !airLeft() && !airRight())
                return 7;
            if (!airAbove() && airBelow() && !airLeft() && airRight())
                return 8;
            
            return 4;
        }

        private static bool isAirAt(Vector2 pos) {
            return Wall.map.getRawTile(pos).tileType == type.air;
        }

        private bool airAbove() {
            return isAirAt(pos - Vector2.UnitY);
        }
        
        private bool airBelow() {
            return isAirAt(pos + Vector2.UnitY);
        }
        
        private bool airLeft() {
            return isAirAt(pos - Vector2.UnitX);
        }
        
        private bool airRight() {
            return isAirAt(pos + Vector2.UnitX);
        }
    }
}