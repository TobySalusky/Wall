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
        public const float pixelSize = 1 / 8F;

        public bool background;
        public bool specialCollide;
        public int specialCollideType;

        public static Color[] shades;
        public const int shadeRange = 7;

        public bool solid = true;

        static Tile() {
            genAtlas();

            shades = new Color[11];
            for (int i = 0; i < 11; i++) {
                float val = 1 - (i / 10F) * 0.9F;
                shades[i] = new Color(val, val, val);
            }
        }

        public enum type {
            
            // blocks
            air,
            snow,
            ice,
            frostStone,
            
            // decor
            DECOR_START,
            snowGrass,
            DECOR_END,
            
            // backgrounds
            snowBack,
            iceBack,
            frostStoneBack
        }

        public static Dictionary<Color, int> genTileTable() {
            var table = new Dictionary<Color, int>();

            tableAdd(table, Color.White, type.snow);
            tableAdd(table, Color.Red, type.ice);
            tableAdd(table, Color.Blue, type.frostStone);
            tableAdd(table, new Color(0F, 1F, 0F, 1F), type.snowGrass);

            return table;
        }

        public static Dictionary<Color, int> genBackTable() {
            var table = new Dictionary<Color, int>();

            tableAdd(table, Color.Red, type.snowBack);
            tableAdd(table, Color.Blue, type.frostStoneBack);

            return table;
        }
        
        public static Dictionary<Color, int> genShadeTable() {
            var table = new Dictionary<Color, int>();

            for (int i = 0; i < 11; i++) {
                table[shades[i]] = i;
            }

            return table;
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

                string identifier = tile.ToString();

                Texture2D atlas = (!Textures.has(identifier) && identifier.Contains("Back"))
                    ? genBackAtlas(identifier)
                    : Textures.get(identifier);
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

        private static Texture2D genBackAtlas(string identifier) {
            string blockIdentifier = identifier.Substring(0, identifier.IndexOf("Back"));

            Texture2D texture = Textures.get(blockIdentifier);
            var arr = Util.colorArray(texture);
            var newArr = new Color[arr.Length];

            for (int i = 0; i < arr.Length; i++) {
                Color col = arr[i];
                newArr[i] = Color.Lerp(arr[i], new Color(Color.Black, col.A / 255F), 0.3F);
            }

            var back = new Texture2D(Wall.getGraphicsDevice(), texture.Width, texture.Height);
            back.SetData(newArr);

            return back;
        }

        private static void tableAdd(Dictionary<Color, int> dict, Color color, type tileType) {
            dict[color] = (int) tileType;
        }

        public Tile(type tileType, Vector2 pos) {

            this.tileType = tileType;
            this.pos = pos;

            background = tileType.ToString().IndexOf("Back") != -1;

            solid = !(tileType == type.air || isDecor());
        }

        public bool isSolid() {
            return solid;
        }

        public bool specialCollideWithRect(Vector2 center, Vector2 dimen) {
            if (specialCollideType == 1) {
                Vector2 corner = center + dimen / 2;
                Vector2 diff = corner - pos;
                diff.X = Math.Clamp(diff.X, 0, 1F);
                diff.Y = Math.Clamp(diff.Y, 0, 1F);

                return diff.X + diff.Y > 1F;
            }

            if (specialCollideType == -1) {
                Vector2 corner = center + dimen / 2 * new Vector2(-1, 1);
                Vector2 diff = corner - pos;
                diff.X = Math.Clamp(diff.X, 0, 1F);
                diff.Y = Math.Clamp(diff.Y, 0, 1F);

                return diff.Y > diff.X; // kinda sus ngl
            }

            return false;
        }

        public bool isDecor() {
            return tileType > type.DECOR_START && tileType < type.DECOR_END;
        }

        public void findTexture() {

            //texture = (tileType == type.air) ? null : Textures.get(tileType.ToString());
            texture = fullAtlas;

            if (!isSolid()) {
                if (blockMasks(
                    -1, -1, -1,
                    -1, -1, 1,
                    0, 1, 0)) {
                    specialCollide = true;
                    specialCollideType = 1;
                }

                if (blockMasks(
                    -1, -1, -1,
                    1, -1, -1,
                    0, 1, 0)) {
                    specialCollide = true;
                    specialCollideType = -1;
                }
            }

            if (texture != null)
                findAtlasRect(findAtlasIndex());
        }

        public Color findShade() {

            float minMag = shadeRange + 1;
            for (int x = -shadeRange; x <= shadeRange; x++) {
                for (int y = -shadeRange; y <= shadeRange; y++) {
                    Vector2 diff = new Vector2(x, y);
                    if (!solidAt(pos + diff)) {
                        float mag = Util.mag(diff);
                        if (mag < minMag) {
                            minMag = mag;
                        }
                    }
                }
            }

            minMag--;

            float darkness = minMag/shadeRange;

            darkness = Math.Clamp(darkness, 0, 1);

            return shades[(int) (darkness * 10)];
        }

        public bool blockMasks(int a, int b, int c, int d, int e, int f, int g, int h, int i) {

            if (!blockMask(a, pos - Vector2.One)) return false;
            if (!blockMask(b, pos - Vector2.UnitY)) return false;
            if (!blockMask(c, pos + new Vector2(1, -1))) return false;
            
            if (!blockMask(d, pos - Vector2.UnitX)) return false;
            if (!blockMask(e, pos)) return false;
            if (!blockMask(f, pos + Vector2.UnitX)) return false;
            
            if (!blockMask(g, pos + new Vector2(-1, 1))) return false;
            if (!blockMask(h, pos + Vector2.UnitY)) return false;
            if (!blockMask(i, pos + Vector2.One)) return false;

            return true;
        }

        public bool blockMask(int i, Vector2 pos) {
            if (i == 0) return true;

            //bool air = !isAirAt(pos);
            //return (i == 1) == air;
            
            bool collision = solidAt(pos);
            return (i == 1) == collision;
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

            if (isDecor()) {
                switch (tileType) {
                    case type.snowGrass:
                        if (sameBelow()) return 0;
                        if (sameAbove()) return 1;

                        return Util.randInt(2, 6);
                }
            }

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

        private bool isAirAt(Vector2 pos, bool background = false) { // TODO: unscuff

            if (background) {
                return Wall.map.getRawBack(pos).tileType == type.air;
            }

            return
                Wall.map.getRawTile(pos).tileType == type.air; //|| (tileType == type.ice && Wall.map.getRawTile(pos).tileType == type.snow);
        }
        
        private bool solidAt(Vector2 pos, bool background = false) { // TODO: unscuff

            if (background) {
                return Wall.map.getRawBack(pos).isSolid();
            }

            return
                Wall.map.getRawTile(pos).isSolid();
        }

        private bool sameAt(Vector2 pos, bool background = false) { // TODO: unscuff

            if (background) {
                return Wall.map.getRawBack(pos).tileType == tileType;
            }

            return
                Wall.map.getRawTile(pos).tileType == tileType;
        }
        private bool sameAbove() {
            return sameAt(pos - Vector2.UnitY, background);
        }
        
        private bool sameBelow() {
            return sameAt(pos + Vector2.UnitY, background);
        }
        
        private bool sameLeft() {
            return sameAt(pos - Vector2.UnitX, background);
        }
        
        private bool sameRight() {
            return sameAt(pos + Vector2.UnitX, background);
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