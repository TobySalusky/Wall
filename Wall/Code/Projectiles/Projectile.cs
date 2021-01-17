using System;
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

        public float rotation;

        public float bonkMult = 0;
        
        protected Texture2D texture;

        public bool deleteFlag;

        public float timeLeft = 5;

        public bool hasGravity;
        public float gravity = Entity.gravity;
        
        private const float collisionStep = 0.1F;
        public bool hasCollision = true;

        public Projectile(Vector2 pos, Vector2 vel, bool playerOwned) {
            this.pos = pos;
            this.vel = vel;


            string textureName = GetType().Name;

            int projIndex = textureName.IndexOf("Projectile");
            if (projIndex != -1) {
                textureName = textureName.Substring(0, projIndex);
            }

            texture = Textures.get(textureName);
            dimen = new Vector2(texture.Width, texture.Height) * Tile.pixelSize;

            targets = playerOwned ? Wall.entities : Wall.playerList;
        }

        
        
        public virtual bool canHit(Entity entity) {
            return hitsLeft != 0 && collidesWith(entity) && (hasHit == null || !hasHit.Contains(entity)) && !entity.invincible;
        }

        public bool collidesWith(Entity entity) {
            return collidesWith(entity, pos, dimen);
        }

        public bool collidesWith(Entity entity, Vector2 pos, Vector2 dimen) {
            return (pos.X + dimen.X / 2 > entity.pos.X - entity.dimen.X / 2 &&
                     pos.X - dimen.X / 2 < entity.pos.X + entity.dimen.X / 2 &&
                    pos.Y + dimen.Y / 2 > entity.pos.Y - entity.dimen.Y / 2 &&
                     pos.Y - dimen.Y / 2 < entity.pos.Y + entity.dimen.Y / 2);
        }

        public virtual void bonk(Vector2 newPos) {
            
        }

        public virtual void bonkX(Vector2 newPos) {
            bonk(newPos);
            vel.X *= bonkMult;
        }

        public virtual void bonkY(Vector2 newPos) {
            bonk(newPos);
            vel.Y *= bonkMult;
        }

        public bool collides() {
            return collidesAt(pos, dimen);
        }

        public bool collidesAt(Vector2 pos) {
            return collidesAt(pos, dimen);
        }

        public virtual bool collidesAt(Vector2 pos, Vector2 dimen) {
            return Wall.map.rectangleCollide(pos, dimen);
        }

        public virtual float knockbackDir() {
            return Util.angle(vel);
        }

        public virtual void hit(Entity entity) {
            hitsLeft--;
            entity.knockedBack(knockback, knockbackDir());
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
            return velAngle();
        }

        public float velAngle() {
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

            if (hasCollision) {
                collisionMove(vel * deltaTime);
            }
            else {
                pos += vel * deltaTime;
            }

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

        protected void collisionMove(Vector2 fullDiff) {
            if (!collidesAt(pos + fullDiff)) { // TODO: improve (can result in clipping [due to initial skip])
                pos += fullDiff;
            } else {

                float diffX = 0, diffY = 0;
                float stepX = collisionStep * Math.Sign(fullDiff.X), stepY = collisionStep * Math.Sign(fullDiff.Y);
                
                // x-component
                if (!collidesAt(pos + Vector2.UnitX * fullDiff.X)) {
                    pos += Vector2.UnitX * fullDiff.X;
                } else {
                    for (int i = 0; i < Math.Abs(fullDiff.X) / collisionStep; i++) {
                        diffX += stepX;
                        if (collidesAt(pos + Vector2.UnitX * diffX)) {

                            diffX -= stepX;
                            bonkX(pos + Vector2.UnitX * diffX); // bonking
                            break;
                        }
                    }

                    pos += Vector2.UnitX * diffX;
                }

                // y-component
                if (!collidesAt(pos + Vector2.UnitY * fullDiff.Y)) {
                    pos += Vector2.UnitY * fullDiff.Y;
                } else {
                    for (int i = 0; i < Math.Abs(fullDiff.Y) / collisionStep; i++) {
                        diffY += stepY;
                        if (collidesAt(pos + Vector2.UnitY * diffY)) {
                            diffY -= stepY;
                            bonkY(pos + Vector2.UnitY * diffY); // bonking
                            break;
                        }
                    }

                    pos += Vector2.UnitY * diffY;
                }
            }
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