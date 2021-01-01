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
        
        public Entity(Vector2 pos) {
            this.pos = pos;

            vel = new Vector2();
            texture = Textures.get("bush");

            dimen = new Vector2(2, 2);
        }

        public virtual float findRotation() {
            return 0;
        }

        public virtual void update(float deltaTime) {
            rotation = findRotation();
            
            grounded = collidesAt(pos + Vector2.UnitY * 0.1F);

            if (hasGravity) {
                vel.Y += gravity * deltaTime;

                const float groundGrav = 2F;
                if (grounded && vel.Y > groundGrav)
                    vel.Y = groundGrav;
            }

            collisionMove(vel * deltaTime);
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
                            vel.X = 0; // bonking
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
                            vel.Y = 0; // bonking
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

        public virtual void render(Camera camera, SpriteBatch spriteBatch) { // TODO: perhaps use more efficient drawing unless needed, also add rotation
            
            //spriteBatch.Draw(texture, camera.toScreen(pos, dimen), Color.White);
            SpriteEffects effects = facingLeft ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            Vector2 textureSize = new Vector2(texture.Width, texture.Height);
            Vector2 scale = dimen * camera.scale / textureSize;
            spriteBatch.Draw(texture, camera.toScreen(pos), null, Color.White, rotation, textureSize / 2F, scale,  effects, 0);

        }
    }
}