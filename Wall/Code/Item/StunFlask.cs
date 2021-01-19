using Microsoft.Xna.Framework;

namespace Wall {
    public class StunFlask : Item {
        
        public StunFlask(int count) : base(count) {
            useDelay = 3F;
            makeStackable();
            consumable = true;
            offset = 1.5F;

            renderWhenReady = true;
            
            handOffset = Vector2.UnitX * -0.2F;
        }

        public override void use(float angle, float distance) {
            base.use(angle, distance);
            
            Wall.projectiles.Add(new StunFlaskProjectile(player.pos + Util.polar(offset, angle), Util.polar(40, angle), true));
        }
    }
}