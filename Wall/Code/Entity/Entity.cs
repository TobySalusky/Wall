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

        public float speed = 30F;

        private const float collisionStep = 0.1F;
        
        private const float gravity = 70F;
        public bool grounded;
        public bool facingLeft = true;
        
        public Entity(Vector2 pos) {
            this.pos = pos;

            vel = new Vector2();
            texture = Textures.get("bush");

            dimen = new Vector2(2, 2);
        }
        
        public virtual void update(float deltaTime) {
            
            grounded = collidesAt(pos + Vector2.UnitY * 0.1F);
            vel.Y += gravity * deltaTime;

            if (grounded && vel.Y > 0.5F)
                vel.Y = 0.5F;

            collisionMove(vel * deltaTime);
        }

        private void collisionMove(Vector2 fullDiff) {
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

        public void render(Camera camera, SpriteBatch spriteBatch) { // TODO: perhaps use more efficient drawing unless needed, also add rotation
            
            //spriteBatch.Draw(texture, camera.toScreen(pos, dimen), Color.White);
            SpriteEffects effects = facingLeft ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            spriteBatch.Draw(texture, camera.toScreen(pos, dimen), null, Color.White, 0, Vector2.Zero, effects, 0);
            
        }
    }
}