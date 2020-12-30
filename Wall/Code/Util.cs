using System;
using Microsoft.Xna.Framework;

namespace Wall {
    public static class Util {

        public static Rectangle center(Vector2 pos, Vector2 dimen) {
            return new Rectangle((int) (pos.X - dimen.X / 2), (int) (pos.Y - dimen.Y / 2), (int)dimen.X, (int)dimen.Y);
        }
        
        public static float modulus(float a, float b) { // floats??
            return (float) (a - b * Math.Floor(a / b));
        }

        public static int intMod(float a, float b) {
            return (int) (a - b * Math.Floor(a / b));
        }

    }
}