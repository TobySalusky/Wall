using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Wall
{
    public class Entity
    {
        public Vector2 pos, vel;
        
        public Texture2D texture;
        public Vector2 dimen;

        public float speed = 25F;

        private const float collisionStep = 0.1F;
        
        public const float gravity = 70F;
        public bool grounded;
        public bool facingLeft = true;

        public float rotation;

        public bool hasStep = true, hasGravity = true;
        public float maxStepHeight = 1.1F;

        public bool deleteFlag;

        public float health = 10, maxHealth = 10;
        public float timeSinceDamaged = 100;

        public Entity(Vector2 pos) {
            this.pos = pos;

            vel = new Vector2();
            texture = Textures.get("bush");

            dimen = new Vector2(2, 2);
        }

        public void initHealth(float health) {
            maxHealth = health;
            this.health = maxHealth;
        }
        
        public Tile getTileOn() {
            return getTileOn(pos);
        }

        public Tile getTileOn(Vector2 pos) {
            return Wall.map.getTile(pos + Vector2.UnitY * (dimen.Y / 2 + 0.1F));
        }
        
        public virtual void jump(float jumpHeight) {
            vel.Y -= Util.heightToJumpPower(jumpHeight, gravity);

            Tile tileOn = getTileOn();
            if (tileOn.tileType == Tile.type.snow) {
                snowImpactPuff(10, 0.5F, pos, tileOn);
            }
        }
        
        public void snowImpactPuff(int count, float intensity, Vector2 pos, Tile tileOn) {
            for (int i = 0; i < count; i++) {
                Particle puff = new SnowPixel(pos + new Vector2(Util.random(-1, 1) * dimen.X * 0.3F, dimen.Y / 2), new Vector2(Util.random(-2, 2) * (1 + intensity), -Util.random(0.3F, 1.3F)), Util.randomColor(tileOn.texture, tileOn.textureAtlasRect()));
                Wall.particles.Add(puff);
            }
        }

        public virtual void die() {
            deleteFlag = true;
        }

        public virtual void damaged(float damage) {
            health -= damage;
            timeSinceDamaged = 0;

            if (health <= 0 && !deleteFlag) {
                die();
            }
        }

        public void knockedBack(Vector2 knockVel) {
            vel += knockVel;
        }

        public void knockedBack(float mag, Vector2 otherPos) {
            knockedBack(Util.polar(mag, Util.angle(pos - otherPos)));
        }
        
        public void knockedBack(float mag, float angle) {
            knockedBack(Util.polar(mag, angle));
        }

        public virtual float findRotation() {
            return 0;
        }

        public virtual void update(float deltaTime) {
            timeSinceDamaged += deltaTime;
            rotation = findRotation();
            
            grounded = collidesAt(pos + Vector2.UnitY * 0.1F);

            if (hasGravity) {
                vel.Y += gravity * deltaTime;

                /*const float groundGrav = 2F;
                if (grounded && vel.Y > groundGrav)
                    vel.Y = groundGrav;*/
            }

            snowRunningPuffs(deltaTime);

            collisionMove(vel * deltaTime);
        }

        public void snowRunningPuffs(float deltaTime) {
            Tile tileOn = getTileOn();
            if (tileOn.tileType != Tile.type.air) {
                float signX = Math.Sign(vel.X), mag = vel.X * signX;

                if (mag > 1) {
                    if (Util.chance(deltaTime * 10)) {
                        Particle puff = new SnowPixel(pos + new Vector2(Util.random(-1, 1) * dimen.X / 4, dimen.Y / 2), new Vector2(-signX * Math.Clamp(mag / 10, 1, 5), -Util.random(0.3F, 1.3F)), Util.randomColor(tileOn.texture, tileOn.textureAtlasRect()));
                        Wall.particles.Add(puff);
                    }
                }
            }
        }

        protected bool collidesWith(Entity entity, Vector2 pos, Vector2 dimen) {
            return Util.center(pos, dimen).Intersects(Util.center(entity.pos, entity.dimen));
        }

        protected bool collidesWith(Entity entity) {
            return collidesWith(entity, pos, dimen);
        }

        public virtual void bonkX(Vector2 newPos) {
            vel.X = 0;
        }
        
        public virtual void bonkY(Vector2 newPos) {
            
            Tile tileOn = getTileOn(newPos);
            if (tileOn.tileType == Tile.type.snow) { //TODO: for some reason causes particles when program is unfocused?!!
                
                float intensity = Math.Min(vel.Y * 0.4F / 30, 1);

                int count = (int) (intensity * 30);
                
                snowImpactPuff(count, intensity, newPos, tileOn);
            }
            
            vel.Y = 0;
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

                            if (hasStep && grounded) { // stepping up single blocks
                                for (float step = 0; step <= maxStepHeight; step += collisionStep) {
                                    if (!collidesAt(pos + new Vector2(diffX, -step))) {
                                        pos += new Vector2(diffX, -step);
                                        collisionMove(fullDiff - Vector2.UnitX * diffX);
                                        return;
                                    }
                                }
                            }

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

        public bool collidesAt(Vector2 pos, Vector2 dimen) {
            return Wall.map.rectangleCollide(pos, dimen);
        }

        public bool collidesAt(Vector2 pos) {
            return collidesAt(pos, dimen);
        }

        public virtual Color getTint() {
            const float redTime = 0.5F, start = 0.35F;

            if (timeSinceDamaged > redTime) {
                return Color.White;
            }

            float amount = (1 - start) * timeSinceDamaged / redTime + start;

            return new Color(1F, amount, amount);
        }

        public virtual void render(Camera camera, SpriteBatch spriteBatch) { // TODO: perhaps use more efficient drawing unless needed, also add rotation
            
            //spriteBatch.Draw(texture, camera.toScreen(pos, dimen), Color.White);
            SpriteEffects effects = facingLeft ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            Vector2 textureSize = new Vector2(texture.Width, texture.Height);
            Vector2 scale = dimen * camera.scale / textureSize;
            spriteBatch.Draw(texture, camera.toScreen(pos), null, getTint(), rotation, textureSize / 2F, scale,  effects, 0);
        }
    }
}