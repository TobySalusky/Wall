using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Wall {
    public class PixelFlame : Projectile {

        public Color tint;
        public float alpha;
        public const float startAlpha = 0.75F;
        
        public PixelFlame(Vector2 pos, Vector2 vel, bool playerOwned) : base(pos, vel, playerOwned) { 

            damage = 0.5F;
            knockback = 0.1F;
            dimen *= 3;

            bonkMult = -0.15F;

            hasGravity = true;
            gravity /= 4;

            timeLeft = Util.random(0.9F, 1.3F);
            alpha = startAlpha;
        }


        public override void update(float deltaTime) {
            base.update(deltaTime);
            
            const float fadeStart = 0.3F;
            if (timeLeft < fadeStart) {
                alpha = (timeLeft / fadeStart) * startAlpha;
            }
        }

        public override bool collidesAt(Vector2 pos, Vector2 dimen) {
            return Wall.map.pointCollide(pos);
        }

        public Color findTint() {
            return new Color(tint, alpha);
        }

        public override void render(Camera camera, SpriteBatch spriteBatch) {
            
            spriteBatch.Draw(texture, camera.toScreen(pos, dimen), findTint());
        }
    }
}