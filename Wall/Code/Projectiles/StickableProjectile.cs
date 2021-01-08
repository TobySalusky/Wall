using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Wall {
    public class StickableProjectile : Projectile {
        
        public bool stuck;

        public StickableProjectile(Vector2 pos, Vector2 vel, bool playerOwned) : base(pos, vel, playerOwned) {
            
        }
        
        public override void changeRotation(float deltaTime) {
            if (!stuck) {
                base.changeRotation(deltaTime);
            }
        }
        
        public override bool canHit(Entity entity) {
            return !stuck && base.canHit(entity);
        }
        
        public Color findTint() {
            const float fadeStart = 0.5F;
            if (stuck && timeLeft < fadeStart) {
                return new Color(Color.White, timeLeft / fadeStart);
            }

            return Color.White;
        }

        public void getStuck() {
            vel = Vector2.Zero;
            hasGravity = false;
            stuck = true;
            timeLeft = 5F;
        }

        public override void render(Camera camera, SpriteBatch spriteBatch) { // TODO: perhaps use more efficient drawing unless needed, also add rotation

            if (texture == null) {
                return;
            }

            Vector2 textureSize = new Vector2(texture.Width, texture.Height);
            Vector2 scale = dimen * camera.scale / textureSize;
            spriteBatch.Draw(texture, camera.toScreen(pos), null, findTint(), rotation, textureSize / 2F, scale,  SpriteEffects.None, 0);
        }
    }
}