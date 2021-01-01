using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Wall {
    public class Chunk {

        public static int[,] mapData; // TODO: do this in chunks or something (perhaps load on the fly) because this can be a huge memory-use
        
        public const int chunkSize = 8;
        public readonly Tile[,] tiles = new Tile[chunkSize, chunkSize];

        private readonly Vector2 indices; // should always be ints

        public bool loaded;
        
        public Chunk(Vector2 indices) {
            this.indices = indices;

            genTiles();
        }

        public static void loadMapData() { // TODO: change colors to ints here

            Texture2D texture = Textures.get("mapData");
            
            var colorData = new Color[texture.Width * texture.Height];
            texture.GetData(colorData);

            Dictionary<Color, int> table = Tile.genTable();
            
            var arrayID = new int[texture.Width,texture.Height];
            for (int row = 0; row < texture.Height; row++)
            {
                for (int col = 0; col < texture.Width; col++)
                {
                    arrayID[col, row] = colorToID(table, colorData[row * texture.Width + col]);
                }
            }

            mapData = arrayID;
        }

        private void genTiles() {
            Vector2 topLeft = indices * chunkSize;
            
            for (int i = 0; i < chunkSize; i++) {
                for (int j = 0; j < chunkSize; j++) {
                    tiles[i, j] = genTile((int)(topLeft.X + i), (int)(topLeft.Y + j));
                }
            }
        }

        public void load() {
            for (int i = 0; i < chunkSize; i++) {
                for (int j = 0; j < chunkSize; j++) {
                    tiles[i, j].findTexture(); // currently causing overflow b/c it tries to load chunk after chunk
                }
            }

            loaded = true;
        }

        private Tile genTile(int x, int y) {

            int ID = (int) Tile.type.air;
            
            if (x >= 0 && x < mapData.GetLength(0) && y >= 0 && y < mapData.GetLength(1)) {
                ID = mapData[x, y];
            }

            return new Tile((Tile.type) ID, new Vector2(x, y));
        }

        private static int colorToID(Dictionary<Color, int> table, Color color) {
            
            return table.GetValueOrDefault(color, (int) Tile.type.air);
        }

        public void render(Camera camera, SpriteBatch spriteBatch) {
            for (int i = 0; i < chunkSize; i++) {
                for (int j = 0; j < chunkSize; j++) {
                    tiles[i,j].render(camera, spriteBatch);
                }
            }
        }
    }
}