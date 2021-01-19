using Microsoft.Xna.Framework;

namespace Wall {
    public class Shuriken : Item {
        
        public Shuriken(int count) : base(count) {
            useDelay = 0.45F;
            makeStackable();
            consumable = true;
            
            offset = 1.5F;
            renderWhenReady = true;
            handOffset = Vector2.UnitX * -0.4F;
        }

        public override void use(float angle, float distance) {
            base.use(angle, distance);
            
            Wall.projectiles.Add(new ShurikenProjectile(player.pos + Util.polar(offset, angle), Util.polar(40, angle), true));
        }
    }
}