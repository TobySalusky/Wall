using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Wall {
    public class Chunk {

        public static int[,] mapData, backMapData, shadeData; // TODO: do this in chunks or something (perhaps load on the fly) because this can be a huge memory-use
        
        public const int chunkSize = 8;
        public readonly Tile[,] tiles = new Tile[chunkSize, chunkSize];
        public readonly Tile[,] backgrounds = new Tile[chunkSize, chunkSize];

        public Texture2D darkness;

        private readonly Vector2 indices; // should always be ints

        public bool loaded;
        
        public Chunk(Vector2 indices) {
            this.indices = indices;

            genTiles();
        }

        public static void loadMapData() { // TODO: change colors to ints here

            mapData = loadColorMap(Textures.get("mapData"), Tile.genTileTable());
            backMapData = loadColorMap(Textures.get("backMapData"), Tile.genBackTable());
            
            Texture2D shadeImage = Textures.get("shadeMapData");
            if (shadeImage != Textures.nullTexture && shadeImage.Width == mapData.GetLength(0) && shadeImage.Height == mapData.GetLength(1)) {
                shadeData = loadColorMap(shadeImage, Tile.genShadeTable());
            }
            else {
                shadeData = new int[mapData.GetLength(0), mapData.GetLength(1)];
            }
        }

        public static void shadeMap() {
            int width = mapData.GetLength(0), height = mapData.GetLength(1);

            var col = new Color[width * height];
            
            Logger.log("Starting Map Shading Process:");

            // load all chunks
            for (int x = 0; x < width; x += chunkSize) {
                for (int y = 0; y < height; y += chunkSize) {
                    Wall.map.getRawChunk(new Vector2(x, y));
                }
            }

            Logger.log("All Map-Chunks Loaded.");

            int progReport = width * height / 10;
            for (int x = 0; x < width; x++) {
                for (int y = 0; y < height; y++) {
                    int index = x + y * width;
                    col[index] = Wall.map.getRawTile(new Point(x, y)).findShade();
                    if (index % progReport == 0 && index != 0) {
                        Logger.log("Progress: " + (int) ((float) index / (width * height) * 100F) + "%" + index);
                    }
                }
            }

            Logger.log("Finished Shading.");
            
            Texture2D shadeImage = new Texture2D(Wall.getGraphicsDevice(), width, height);
            shadeImage.SetData(col);
            Textures.exportTexture(shadeImage, Paths.texturePath, "shadeMapData");
            Logger.log("Shade Data Exported.");
            Textures.debugTexturesGrab()["shadeMapData"] = shadeImage;
            shadeData = loadColorMap(shadeImage, Tile.genShadeTable());
            
            Wall.map.chunks.Clear();
        }

        public static int[,] loadColorMap(Texture2D texture, Dictionary<Color, int> table) { // TODO: change colors to ints here
            
            var colorData = new Color[texture.Width * texture.Height];
            texture.GetData(colorData);
            
            var arrayID = new int[texture.Width,texture.Height];
            for (int row = 0; row < texture.Height; row++)
            {
                for (int col = 0; col < texture.Width; col++)
                {
                    arrayID[col, row] = colorToID(table, colorData[row * texture.Width + col]);
                }
            }

            return arrayID;
        }

        private void genTiles() {
            Vector2 topLeft = indices * chunkSize;
            
            for (int i = 0; i < chunkSize; i++) {
                for (int j = 0; j < chunkSize; j++) {
                    backgrounds[i, j] = genBack((int)(topLeft.X + i), (int)(topLeft.Y + j));
                    tiles[i, j] = genTile((int)(topLeft.X + i), (int)(topLeft.Y + j));
                }
            }
        }

        public Rectangle screenRect(Camera camera) {
            return Util.tl(camera.toScreen(indices * chunkSize), Vector2.One * chunkSize * camera.scale);
        }

        public void load() {
            
            for (int i = 0; i < chunkSize; i++) {
                for (int j = 0; j < chunkSize; j++) {
                    tiles[i, j].findTexture();
                    backgrounds[i, j].findTexture();
                }
            }

            int size = chunkSize + 2;
            darkness = new Texture2D(Wall.getGraphicsDevice(), size, size);
            var col = new Color[size * size];
            for (int i = 0; i < size; i++) {
                for (int j = 0; j < size; j++) {
                    Vector2 topLeft = indices * chunkSize;
                    int x = (int) (topLeft.X + i) - 1;
                    int y = (int) (topLeft.Y + j) - 1;
                    if (x >= 0 && x < mapData.GetLength(0) && y >= 0 && y < mapData.GetLength(1)) {
                        float dark = shadeData[x, y] / 10F;
                        col[i + j * size] = new Color(0F, 0F, 0F, dark);
                    }
                }
            }
            darkness.SetData(col);

            loaded = true;
        }

        private Tile genTile(int x, int y) {
            const int airID = (int) Tile.type.air;
            int ID = airID;

            if (x >= 0 && x < mapData.GetLength(0) && y >= 0 && y < mapData.GetLength(1)) {
                ID = mapData[x, y];
            }

            return new Tile((Tile.type) ID, new Vector2(x, y));
        }
        
        private Tile genBack(int x, int y) {

            int ID = (int) Tile.type.air;
            
            if (x >= 0 && x < backMapData.GetLength(0) && y >= 0 && y < backMapData.GetLength(1)) {
                ID = backMapData[x, y];
            }

            return new Tile((Tile.type) ID, new Vector2(x, y));
        }

        private static int colorToID(Dictionary<Color, int> table, Color color) {
            
            return table.GetValueOrDefault(color, (int) Tile.type.air);
        }

        public void render(Camera camera, SpriteBatch spriteBatch) {
            for (int i = 0; i < chunkSize; i++) {
                for (int j = 0; j < chunkSize; j++) {
                    backgrounds[i,j].render(camera, spriteBatch); // TODO: only render when necessary
                    tiles[i,j].render(camera, spriteBatch);
                }
            }
        }
    }
}