using Microsoft.Xna.Framework;

namespace Wall {
    public class SnowBall : Item {
        
        public SnowBall(int count) : base(count) {
            useDelay = 0.3F;
            makeStackable();
            consumable = true;

            offset = 2;
            renderWhenReady = true;
            handOffset = Vector2.UnitX * -0.3F;
        }

        public override void use(float angle, float distance) {
            base.use(angle, distance);
            
            Wall.projectiles.Add(new SnowBallProjectile(player.pos + Util.polar(offset, angle), Util.polar(50, angle), true));
        }
    }
}