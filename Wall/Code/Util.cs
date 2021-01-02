using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Wall {
    public static class Util {

        private static Random rand = new Random();
        
        public static Rectangle center(Vector2 pos, Vector2 dimen) {
            return new Rectangle((int) (pos.X - dimen.X / 2), (int) (pos.Y - dimen.Y / 2), (int)dimen.X, (int)dimen.Y);
        }
        
        public static float modulus(float a, float b) { // floats??
            return (float) (a - b * Math.Floor(a / b));
        }

        public static int intMod(float a, float b) {
            return (int) (a - b * Math.Floor(a / b));
        }

        public static Vector2 polar(float mag, float angle) {
            return new Vector2((float) Math.Cos(angle) * mag, (float) Math.Sin(angle) * mag);
        }

        public static float angle(Vector2 vector) {
            return (float) Math.Atan2(vector.Y, vector.X);
        }
        
        public static float mag(Vector2 vector) {
            return Vector2.Distance(Vector2.Zero, vector);
        }
        
        public static void render(Texture2D texture, Vector2 pos, Vector2 dimen, float rotation, Camera camera, SpriteBatch spriteBatch) { // TODO: perhaps use more efficient drawing unless needed, also add rotation
            
            Vector2 textureSize = new Vector2(texture.Width, texture.Height);
            Vector2 scale = dimen * camera.scale / textureSize;
            spriteBatch.Draw(texture, camera.toScreen(pos), null, Color.White, rotation, textureSize / 2F, scale,  SpriteEffects.None, 0);
        }
        
        public static void render(Texture2D texture, Vector2 pos, Vector2 dimen, float rotation, Camera camera, SpriteBatch spriteBatch, bool flipped) { // TODO: perhaps use more efficient drawing unless needed, also add rotation
            
            SpriteEffects effects = flipped ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            Vector2 textureSize = new Vector2(texture.Width, texture.Height);
            Vector2 scale = dimen * camera.scale / textureSize;
            spriteBatch.Draw(texture, camera.toScreen(pos), null, Color.White, rotation, textureSize / 2F, scale,  effects, 0);
        }

        public static int randInt(int startInc, int endExc) {
            return rand.Next(startInc, endExc);
        }
        
        public static int randInt(int endExc) {
            return rand.Next(0, endExc);
        }

        public static float random(float min, float max) {
            return (float) rand.NextDouble() * (max - min) + min;
        }

        public static float randomAngle() {
            return random(Maths.twoPI);
        }

        public static float random(float max) {
            return (float) rand.NextDouble() * max;
        }

        public static float heightToJumpPower(float jumpHeight, float gravity) {
            return (float) Math.Sqrt(jumpHeight * 2 * gravity);
        }

        public static Color[] colorArray(Texture2D texture) {
            var colorData = new Color[texture.Width * texture.Height];
            texture.GetData(colorData);

            return colorData;
        }

        public static bool chance(float chance) {
            return (rand.NextDouble() < chance);
        }

        public static Color randomColor(Texture2D texture) { // TODO:  WARNING:does not guarantee a non invisible color
            var arr = colorArray(texture);
            return arr[(int) (rand.NextDouble() * arr.Length)];
        }
        
        public static Color randomColor(Texture2D texture, Rectangle rect) { // TODO:  WARNING:does not guarantee a non invisible color
            var arr = colorArray(texture);
            int x = randInt(rect.Width + rect.X);
            int y = randInt(rect.Height + rect.Y);
            return arr[x + y * texture.Width];
        }

        public static bool isClassOrSub(Object obj, Type superClass) {
            return obj.GetType().IsSubclassOf(superClass) || obj.GetType() == superClass;
        }

        public static void debugDot(Vector2 pos, Camera camera, SpriteBatch spriteBatch) {
            render(Textures.get("bush"), pos, Vector2.One * 0.5F,  0, camera, spriteBatch);
        }
        
        // FROM: https://stackoverflow.com/questions/972307/how-to-loop-through-all-enum-values-in-c
        public static IEnumerable<T> GetValues<T>() {
            return Enum.GetValues(typeof(T)).Cast<T>();
        }
    }
}