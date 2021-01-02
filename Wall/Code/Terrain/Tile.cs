using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Wall {
    public class Tile {

        public readonly type tileType;
        public readonly Vector2 pos; // marks top-left location

        public static Texture2D fullAtlas;
        public Texture2D texture;
        private Rectangle atlasRect; // TODO:

        public const int pixelCount = 8;
        public const float pixelSize = 1/8F;

        public bool background;

        static Tile() {
            genAtlas();
        }

        public enum type {
            air, snow, ice, 
            snowBack
        }

        public static void genAtlas() {

            var types = Util.GetValues<type>();
            
            fullAtlas = new Texture2D(Wall.getGraphicsDevice(), 24, 24 * (types.Count() - 1));
            var colorArr = new Color[fullAtlas.Width * fullAtlas.Height];

            int i = 0;
            foreach (var tile in types) {
                if (tile == type.air) {
                    continue;
                }

                Texture2D atlas = Textures.get(tile.ToString());
                var atlasCol = Util.colorArray(atlas);
                
                for (int x = 0; x < atlas.Width; x++) {
                    for (int y = 0; y < atlas.Height; y++) {

                        int index = x + y * atlas.Width;
                        int fullIndex = x + (y + atlas.Height * i) * atlas.Width;

                        colorArr[fullIndex] = atlasCol[index];
                    }
                }

                i++;
            }

            fullAtlas.SetData(colorArr);
        }

        public static Dictionary<Color, int> genTileTable() {
            var table = new Dictionary<Color, int>();

            tableAdd(table, Color.White, type.snow);
            tableAdd(table, Color.Red, type.ice);
            
            return table;
        }
        
        public static Dictionary<Color, int> genBackTable() {
            var table = new Dictionary<Color, int>();

            tableAdd(table, Color.Red, type.snowBack);
            
            return table;
        }

        private static void tableAdd(Dictionary<Color, int> dict, Color color, type tileType) {
            dict[color] = (int) tileType;
        }

        public Tile(type tileType, Vector2 pos) {

            this.tileType = tileType;
            this.pos = pos;

            background = tileType.ToString().IndexOf("Back") != -1;
        }

        public bool isSolid() {
            return tileType != type.air;
        }

        public void findTexture() {

            //texture = (tileType == type.air) ? null : Textures.get(tileType.ToString());
            texture = fullAtlas;

            if (texture != null)
                findAtlasRect(findAtlasIndex());
        }
        
        public void render(Camera camera, SpriteBatch spriteBatch) { // TODO: make more efficient
            
            if (tileType == type.air) return;
            
            Vector2 screen = camera.toScreen(pos);
            int size = (int) camera.scale;
            Rectangle rect = new Rectangle((int) screen.X, (int) screen.Y, size, size);
                
            spriteBatch.Draw(texture, rect, atlasRect, Color.White);
        }

        public Rectangle textureAtlasRect() {
            return new Rectangle(0, 24 * (int) (tileType - 1), 24, 24);
        }

        private void findAtlasRect(int index) {
            int rows = 3, cols = 3;

            int col = index % cols;
            int row = index / cols;

            int size = texture.Width / cols; // assumes square blocks

            atlasRect = new Rectangle(size * col, size * row, size, size);
            atlasRect.Y += 24 * (int) (tileType - 1);
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

        private static bool isAirAt(Vector2 pos, bool background) { // TODO: unscuff

            if (background) {
                return Wall.map.getRawBack(pos).tileType == type.air;
            }

            return Wall.map.getRawTile(pos).tileType == type.air;
        }

        private bool airAbove() {
            return isAirAt(pos - Vector2.UnitY, background);
        }
        
        private bool airBelow() {
            return isAirAt(pos + Vector2.UnitY, background);
        }
        
        private bool airLeft() {
            return isAirAt(pos - Vector2.UnitX, background);
        }
        
        private bool airRight() {
            return isAirAt(pos + Vector2.UnitX, background);
        }
    }
}