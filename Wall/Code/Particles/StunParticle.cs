using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Wall {
    public class StunParticle : Particle {
        
        private static readonly Texture2D starTexture = Textures.get("StunStar");

        public float rotSpeed;
        
        public StunParticle(Vector2 pos, Vector2 vel) : base(pos, vel, starTexture) {
            hasCollision = true;
            bonkMult = -0.5F;

            timeLeft = Util.random(0.4F, 0.6F);
            rotSpeed = Util.random(-1, 1);
        }

        public override void update(float deltaTime) {
            base.update(deltaTime);
            vel += deltaTime * Util.polar(0.4F, Util.random(-100, 100));

            rotation += rotSpeed * deltaTime * Maths.twoPI;
            
            const float shrinkStart = 0.3F;
            if (timeLeft < shrinkStart) {
                dimen = Vector2.One * Tile.pixelSize * timeLeft / shrinkStart;
            }
        }
    }
}