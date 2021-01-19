using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Wall {
    public 
        class ChunkMap {
        public readonly Dictionary<Point, Chunk> chunks;

        public ChunkMap() {
            chunks = new Dictionary<Point, Chunk>();
        }

        public Chunk getChunk(Point indices) {

            Chunk chunk = getRawChunk(indices);

            if (!chunk.loaded)
                chunk.load();
            
            return chunk;
        }

        public Chunk getRawChunk(Point indices) { // returns chunk without fully loading it (generates texture-less tiles)
            chunks.TryGetValue(indices, out var chunk);

            if (chunk == null) {
                return addChunk(indices);
            }

            return chunk;
        }
        
        public Chunk getRawChunk(Vector2 position) { // finds chunk containing these coordinates

            return getRawChunk(chunkIndices(position));
        }

        public Chunk getChunk(Vector2 position) { // finds chunk containing these coordinates

            return getChunk(chunkIndices(position));
        }

        public static Point chunkIndices(Vector2 position) { // finds indices of chunk containing these coordinates
            var (x, y) = position / Chunk.chunkSize;
            return new Point((int) Math.Floor(x), (int) Math.Floor(y));
        }

        public static Point blockIndices(Vector2 position) {
            var (x, y) = position;
            return new Point((int) Math.Floor(x), (int) Math.Floor(y));
        }

        public Chunk addChunk(Point indices) {
            
            var (x, y) = indices;
            Chunk chunk = new Chunk(new Vector2(x, y));

            chunks[indices] = chunk;
            
            return chunk;
        }

        public void render(Camera camera, SpriteBatch spriteBatch) {

            Vector2 diff = camera.screenCenter / camera.scale;
            Point from = chunkIndices(camera.pos - diff);
            Point to = chunkIndices(camera.pos + diff);

            for (int i = from.X; i <= to.X; i++) {
                for (int j = from.Y; j <= to.Y; j++) {
                    getChunk(new Point(i, j)).render(camera, spriteBatch);
                }
            }
        }
        
        public void renderShading(Camera camera, SpriteBatch spriteBatch) {
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, SamplerState.PointClamp, null, null, Wall.testShader, null);

            Vector2 diff = camera.screenCenter / camera.scale;
            Point from = chunkIndices(camera.pos - diff);
            Point to = chunkIndices(camera.pos + diff);

            Rectangle rect = getChunk(from).screenRect(camera);
            Point startRect = new Point(rect.X, rect.Y);
            int startX = rect.X;
            int startY = rect.Y;
            for (int i = from.X; i <= to.X; i++) {
                for (int j = from.Y; j <= to.Y; j++) {
                    int diffX = i - from.X;
                    int diffY = j - from.Y;
                    Rectangle thisRect = new Rectangle(startX + rect.Width * diffX, startY + rect.Height * diffY, rect.Width,
                        rect.Height);
                    Chunk chunk = getChunk(new Point(i, j));
                    spriteBatch.Draw(chunk.darkness, thisRect, Color.White);
                }
            }
            
            spriteBatch.End();
        }

        public Tile getTile(Vector2 pos) {
            return getTile(blockIndices(pos));
        }

        public Tile getTile(Point indices) {
            var (x, y) = indices;
            Chunk chunk = getChunk(chunkIndices(new Vector2(x, y)));
            return chunk.tiles[Util.intMod(x, Chunk.chunkSize), Util.intMod(y, Chunk.chunkSize)];
        }
        
        public Tile getRawTile(Vector2 pos) {
            return getRawTile(blockIndices(pos));
        }
        
        public Tile getRawTile(Point indices) {
            var (x, y) = indices;
            Chunk chunk = getRawChunk(chunkIndices(new Vector2(x, y)));
            return chunk.tiles[Util.intMod(x, Chunk.chunkSize), Util.intMod(y, Chunk.chunkSize)];
        }
        
        public Tile getRawBack(Vector2 pos) {
            return getRawBack(blockIndices(pos));
        }
        
        public Tile getRawBack(Point indices) {
            var (x, y) = indices;
            Chunk chunk = getRawChunk(chunkIndices(new Vector2(x, y)));
            return chunk.backgrounds[Util.intMod(x, Chunk.chunkSize), Util.intMod(y, Chunk.chunkSize)];
        }

        public bool pointCollide(Vector2 pos) {
            return getTile(pos).isSolid();
        }

        public bool rectangleCollide(Vector2 center, Vector2 dimen) { // TODO: optimise please (go chunk by chunk)
            Vector2 diff = dimen / 2;
            Point from = blockIndices(center - diff);
            Point to = blockIndices(center + diff);
            
            for (int i = from.X; i <= to.X; i++) {
                for (int j = from.Y; j <= to.Y; j++) {
                    Tile tile = getTile(new Point(i, j));

                    if ((tile.isSolid() || tile.specialCollide) && (!tile.specialCollide || tile.specialCollideWithRect(center, dimen)))
                        return true;
                }
            }

            return false;
        }
    }
}