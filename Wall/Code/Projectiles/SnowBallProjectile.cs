using System;
using Microsoft.Xna.Framework;

namespace Wall {
    public class SnowBallProjectile : Projectile {
        
        public SnowBallProjectile(Vector2 pos, Vector2 vel, bool playerOwned) : base(pos, vel, playerOwned) {
            hasGravity = true;
            damage = 2F;
            knockback = 5;
        }

        public override bool collidesAt(Vector2 pos, Vector2 dimen) {
            return base.collidesAt(pos, dimen * 0.5F);
        }

        public override void changeRotation(float deltaTime) {
            rotation += Math.Sign(vel.X) * deltaTime * (float) Math.PI * 2;
        }

        public override void hit(Entity entity) {
            base.hit(entity);
            puff(30);
        }

        public void puff(int count) {
            for (int i = 0; i < count; i++) {
                
                Particle particle = new SnowPixel(pos + Util.polar(dimen.X / 2, Util.randomAngle()),
                    Util.polar(Util.random(8, 15), Util.angle(vel)) + Util.polar(Util.random(3), Util.randomAngle()), Util.randomColor(texture));
                
                Wall.particles.Add(particle);
            }
        }

        public override void bonk(Vector2 newPos) {
            base.bonk(newPos);

            puff(100);
            deleteFlag = true;
        }
    }
}