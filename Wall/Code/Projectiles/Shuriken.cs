using System;
using Microsoft.Xna.Framework;

namespace Wall {
    public class Shuriken : Projectile {
        
        public Shuriken(Vector2 pos, Vector2 vel, bool playerOwned) : base(pos, vel, playerOwned) {
            hasGravity = true;
            damage = 3;
            hitsLeft = 3;
            knockback = 10;
        }

        public override void changeRotation(float deltaTime) {
            rotation += Math.Sign(vel.X) * deltaTime * (float) Math.PI * 5;
        }
    }
}