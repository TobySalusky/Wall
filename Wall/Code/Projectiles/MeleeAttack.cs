using Microsoft.Xna.Framework;

namespace Wall {
    public class MeleeAttack : Projectile {
        
        public MeleeAttack(Vector2 pos, bool playerOwned) : base(pos, Vector2.Zero, playerOwned) {
            hitsLeft = -1;
            texture = null;
        }
    }
}