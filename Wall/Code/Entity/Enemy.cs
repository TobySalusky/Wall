using System;
using Microsoft.Xna.Framework;

namespace Wall {
    public class Enemy : Entity {

        public Player player;

        protected float meleeDelay = 1F, meleeTimer;
        
        
        public Enemy(Vector2 pos) : base(pos) {
            player = Wall.player;
        }

        public override void update(float deltaTime) {
            
            attackTick(deltaTime);
            meleeTimer -= deltaTime;
            
            base.update(deltaTime);
        }

        public virtual void attackTick(float deltaTime) {
            
        }

        public float angleToPlayer() {
            return Util.angle(player.pos - pos);
        }

        public void meleeAttack(Entity target, float damage, float knockBack) {
            if (collidesWith(target) && meleeTimer <= 0) {
                meleeTimer = meleeDelay;
                target.damaged(damage);
                target.knockedBack(knockBack, pos);
            }
        }
        
        // FOR SNOW SLIMES
        public void snowPuffDeath() {
            Color[] colorArray = Util.colorArray(texture);
            
            Vector2 tMid = new Vector2(texture.Width, texture.Height) / 2;
            float angle = Util.angle(vel);
            float mag = Math.Clamp(Util.mag(vel) / 10, 3, 7);
            
            for (int i = 0; i < texture.Width; i++) {
                for (int j = 0; j < texture.Height; j++) {
                    int index = i + j * texture.Width;

                    Color color = colorArray[index];

                    if (color.A != 0) {
                        Vector2 add = (new Vector2(i, j) - tMid) * Tile.pixelSize;
                        if (!facingLeft) {
                            add.X *= -1;
                        }

                        Vector2 pPos = pos + add;

                        Particle particle = new SnowPixel(pPos,
                            Util.polar(Util.random(0.9F, 5F) * mag, angle + Util.random(-0.1F, 0.1F)), color);
                        
                        Wall.particles.Add(particle);
                    }
                }
            }
        }
    }
}