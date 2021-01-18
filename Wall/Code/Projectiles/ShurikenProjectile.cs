using System;
using Microsoft.Xna.Framework;

namespace Wall {
    public class ShurikenProjectile : StickableProjectile {
        
        public ShurikenProjectile(Vector2 pos, Vector2 vel, bool playerOwned) : base(pos, vel, playerOwned) {
            hasGravity = true;
            damage = 3;
            hitsLeft = 3;
            knockback = 10;

            rotation = Util.randomAngle();
        }

        public override bool collidesAt(Vector2 pos, Vector2 dimen) {
            return base.collidesAt(pos, dimen * 0.5F);
        }

        public override void changeRotation(float deltaTime) {
            rotation += Math.Sign(vel.X) * deltaTime * (float) Math.PI * 5;
        }

        public override void bonk(Vector2 newPos) {
            base.bonk(newPos);

            getStuck();
        }
    }
}