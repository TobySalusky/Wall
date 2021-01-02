using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Wall {
    public class Projectile {

        public Vector2 pos, vel, dimen;
        public List<Entity> targets;
        public int hitsLeft = 1;
        public List<Entity> hasHit;

        public float damage = 5, knockback = 20;

        public float rotation = 0;
        
        protected Texture2D texture;

        public bool deleteFlag;

        public float timeLeft = 5;

        public bool hasGravity;
        public float gravity = Entity.gravity;

        public Projectile(Vector2 pos, Vector2 vel, bool playerOwned) {
            this.pos = pos;
            this.vel = vel;


            texture = Textures.get(GetType().Name);
            dimen = new Vector2(texture.Width, texture.Height) * Tile.pixelSize;

            targets = playerOwned ? Wall.entities : Wall.playerList;
        }

        public bool canHit(Entity entity) {
            return hitsLeft != 0 && collidesWith(entity) && (hasHit == null || !hasHit.Contains(entity));
        }

        public bool collidesWith(Entity entity) {
            return collidesWith(entity, pos, dimen);
        }

        public bool collidesWith(Entity entity, Vector2 pos, Vector2 dimen) {
            return Util.center(pos, dimen).Intersects(Util.center(entity.pos, entity.dimen));
        }

        public virtual void hit(Entity entity) {
            hitsLeft--;
            entity.knockedBack(knockback, Util.angle(vel));
            entity.damaged(damage);

            if (hitsLeft != 0) {
                if (hasHit == null) {
                    hasHit = new List<Entity>();
                }
                hasHit.Add(entity);
            }
        }

        public virtual void changeRotation(float deltaTime) {
            rotation = findRotation();
        }

        public virtual float findRotation() {
            return Util.angle(vel);
        }

        public virtual void update(float deltaTime) {

            if (hasGravity) {
                vel += Vector2.UnitY * deltaTime * gravity;
            }

            timeLeft -= deltaTime;
            if (timeLeft <= 0) {
                deleteFlag = true;
            }

            pos += vel * deltaTime;
            
            foreach (Entity entity in targets) {
                if (canHit(entity)) {
                    hit(entity);
                }

                if (hitsLeft == 0) {
                    deleteFlag = true;
                    break;
                }
            }

            changeRotation(deltaTime);
        }

        public virtual void render(Camera camera, SpriteBatch spriteBatch) { // TODO: perhaps use more efficient drawing unless needed, also add rotation

            if (texture == null) {
                return;
            }

            Vector2 textureSize = new Vector2(texture.Width, texture.Height);
            Vector2 scale = dimen * camera.scale / textureSize;
            spriteBatch.Draw(texture, camera.toScreen(pos), null, Color.White, rotation, textureSize / 2F, scale,  SpriteEffects.None, 0);
        }
    }
}