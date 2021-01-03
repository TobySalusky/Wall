using Microsoft.Xna.Framework;

namespace Wall {
    public class Arrow : Item {
        
        public Arrow(int count) : base(count) {
            makeStackable();
        }

        public virtual ArrowProjectile createArrow(Vector2 pos, Vector2 vel) {
            return new ArrowProjectile(pos, vel, true);
        }
    }

    public class RubberArrow : Arrow {
        public RubberArrow(int count) : base(count) {
        }

        public override ArrowProjectile createArrow(Vector2 pos, Vector2 vel) {
            return new RubberArrowProjectile(pos, vel, true);
        }
    }
}