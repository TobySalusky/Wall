using Microsoft.Xna.Framework;

namespace Wall {
    public class Arrow : Projectile {
        
        public Arrow(Vector2 pos, Vector2 vel, bool playerOwned) : base(pos, vel, playerOwned) {
            hasGravity = true;

            damage = 5;
            knockback = 13;
        }

        public override float findRotation() {
            return base.findRotation() + Maths.halfPI;
        }
    }
}