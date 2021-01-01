using System;
using Microsoft.Xna.Framework;

namespace Wall {
    public class SnowPixel : PixelParticle {
        
        public SnowPixel(Vector2 pos, Vector2 vel, Color tint) : base(pos, vel, tint) {
            hasCollision = true;
            bonkMult = -0.5F;

            timeLeft = Util.random(0.6F, 1F);
        }

        public override void update(float deltaTime) {
            base.update(deltaTime);
            vel += deltaTime * Util.polar(0.4F, Util.random(-100, 100));
            
            const float shrinkStart = 0.3F;
            if (timeLeft < shrinkStart) {
                dimen = Vector2.One * Tile.pixelSize * timeLeft / shrinkStart;
            }
        }
    }
}