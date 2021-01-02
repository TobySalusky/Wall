namespace Wall {
    public class SnowBall : Item {
        
        public float offset = 1.5F;

        public SnowBall(int count) : base(type.SnowBall, count) {
            useDelay = 0.3F;
            makeStackable();
            consumable = true;
        }

        public override void use(float angle, float distance) {
            base.use(angle, distance);
            
            Wall.projectiles.Add(new SnowBallProjectile(player.pos + Util.polar(offset, angle), Util.polar(50, angle), true));
        }
    }
}