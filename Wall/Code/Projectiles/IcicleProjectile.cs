using Microsoft.Xna.Framework;

namespace Wall {
    public class IcicleProjectile : StickableProjectile {
        
        public IcicleProjectile(Vector2 pos, Vector2 vel, bool playerOwned) : base(pos, vel, playerOwned) {
            hasGravity = true;
            damage = 7;
            knockback = 10;
        }

        public override float findRotation() {
            return base.findRotation() + Maths.PI / 4;
        }

        public override void bonk(Vector2 newPos) {
            base.bonk(newPos);
            getStuck();
        }
    }
}