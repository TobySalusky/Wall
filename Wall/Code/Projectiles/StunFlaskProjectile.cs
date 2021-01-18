using System;
using System.Runtime.CompilerServices;
using Microsoft.Xna.Framework;

namespace Wall {
    public class StunFlaskProjectile : Projectile {
        
        public StunFlaskProjectile(Vector2 pos, Vector2 vel, bool playerOwned) : base(pos, vel, playerOwned) {
            hasGravity = true;
            damage = 0;
            knockback = 0;

            rotation = velAngle();
        }

        public override void changeRotation(float deltaTime) {
            rotation += Math.Sign(vel.X) * deltaTime * (float) Math.PI * 5;
        }

        public override void hit(Entity entity) {
            stunExplode();
        }

        public void stunExplode() {

            deleteFlag = true;

            for (int i = 0; i < 15; i++) {
                Wall.particles.Add(new StunParticle(pos, Util.polar(Util.random(5, 15), Util.randomAngle())));
            }
            Wall.projectiles.Add(new StunExplosion(pos, true));
        }

        public override void bonk(Vector2 newPos) {
            base.bonk(newPos);

            stunExplode();
        }
    }
}