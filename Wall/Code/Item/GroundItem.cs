using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Wall {
    public class GroundItem {

        public Item item;

        public const float despawnTime = 100;
        
        public Vector2 pos, vel;
        
        public bool deleteFlag;

        public float timeLeft;

        public float gravity = Entity.gravity / 2;
        
        private const float collisionStep = 0.1F;

        
        public GroundItem(Item item, Vector2 pos, Vector2 vel) {
            this.pos = pos - item.dimen.Y / 2 * Vector2.UnitY;
            this.vel = vel;

            this.item = item;

            timeLeft = despawnTime;
        }
        
        protected bool collidesWith(Entity entity) {
            return Util.center(pos, item.dimen).Intersects(Util.center(entity.pos, entity.dimen));
        }
        
        public virtual void update(float deltaTime) {

            // gravitates towards player
            const float gravStart = 6;
            Player player = Wall.player;
            Vector2 diff = (player.pos - player.dimen.Y / 2 * Vector2.UnitY) - pos;
            if (Util.mag(diff) < gravStart) {
                vel += Util.polar(50 * (1 - (Util.mag(diff) / gravStart)), Util.angle(diff)) * deltaTime;

                if (collidesWith(player)) {
                    player.tryPickUp(item);
                }
            }

            vel += Vector2.UnitY * deltaTime * gravity;
            
            float friction = (collidesAt(pos + Vector2.UnitY * 0.1F)) ? 2 : 1F;
            vel.X -= vel.X * deltaTime * friction;

            timeLeft -= deltaTime;
            if (timeLeft <= 0) {
                deleteFlag = true;
            }

            collisionMove(vel * deltaTime);
        }
        
        public bool collidesAt(Vector2 pos) {
            return collidesAt(pos, item.dimen);
        }

        public virtual bool collidesAt(Vector2 pos, Vector2 dimen) {
            return Wall.map.pointCollide(pos + dimen.Y / 2 * Vector2.UnitY);
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
                            vel.X = 0;
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
                            vel.Y = 0;
                            break;
                        }
                    }

                    pos += Vector2.UnitY * diffY;
                }
            }
        }
        
        public void render(Camera camera, SpriteBatch spriteBatch) { // TODO: make more efficient
            
            spriteBatch.Draw(item.texture, camera.toScreen(pos, item.dimen), Color.White);
        }
    }
}