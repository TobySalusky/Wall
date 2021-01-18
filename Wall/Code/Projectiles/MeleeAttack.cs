using Microsoft.Xna.Framework;

namespace Wall {
    public class MeleeAttack : Projectile {

        public float knockbackAngle;
        
        public MeleeAttack(Vector2 pos, bool playerOwned) : base(pos, Vector2.Zero, playerOwned) {
            hitsLeft = -1;
            texture = null;
        }

        public override float knockbackDir(Entity entity) {
            return knockbackAngle;
        }
    }
}