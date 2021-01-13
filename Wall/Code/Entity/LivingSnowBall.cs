using System;
using Microsoft.Xna.Framework;

namespace Wall {
    public class LivingSnowBall : Enemy {

        public int circle;
        public float circleTimer;
        
        public LivingSnowBall(Vector2 pos) : base(pos) {
            initTexture("SnowBall");

            hasGravity = false;
            runParticles = false;
            canSpawnInAir = true;
            initHealth(3);
            bonkMult = -0.3F;
            circleTimer = Util.random(-0.5F, 0);
        }

        public override void update(float deltaTime) {

            circleTimer -= deltaTime;
                        
            if (circleTimer <= 0) {
                circle = 0;
            }

            if (circleTimer < -0.5F) {
                circleTimer = Util.random(0.1F, 0.7F);
                circle = Util.randomIntPN();
            }

            Vector2 toVel = Util.polar((circle == 0) ? 40 : 30, angleToPlayer() + Maths.halfPI * circle);

            vel += (toVel - vel) * Math.Min(1, deltaTime * 3);
            
            base.update(deltaTime);
        }

        public override void die() {
            base.die();
            
            snowPuffDeath();
        }
        
        public override void bonk(Vector2 newPos) {
            if (Util.mag(vel) > 10F) {
                die();
            }
            base.bonk(newPos);
        }

        public override float findRotation(float deltaTime) {
            return rotation + Math.Sign(vel.X) * Math.Clamp((vel.X / 30), -1, 1) * deltaTime * (float) Math.PI * 5;
        }

        public override void attackTick(float deltaTime) {
            meleeAttack(player, 5, 5);
        }

        public override float knockBackDir(Entity target) {
            return Util.angle(vel);
        }

        public override bool meleeAttack(Entity target, float damage, float knockBack) {
            bool hit = base.meleeAttack(target, damage, knockBack);
            if (hit)
                die();
            return hit;
        }
    }
}