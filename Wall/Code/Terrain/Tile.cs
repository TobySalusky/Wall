using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Wall {
    public class Tile {

        public readonly type tileType;
        public readonly Vector2 pos; // marks top-left location
        
        private Texture2D texture;
        
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

            texture = findTexture(tileType);
        }

        public bool isSolid() {
            return tileType != type.air;
        }

        private static Texture2D findTexture(type tileType) {

            return (tileType == type.air) ? null : Textures.get(tileType.ToString());
        }
        
        public void render(Camera camera, SpriteBatch spriteBatch) { // TODO: make more efficient
            
            if (tileType == type.air) return;
            
            var (x, y) = camera.toScreen(pos);
            int size = (int) camera.scale;
            Rectangle rect = new Rectangle((int) x, (int) y, size, size);
                
            spriteBatch.Draw(texture, rect, Color.White);
        }
    }
}