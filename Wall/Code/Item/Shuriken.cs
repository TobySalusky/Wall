namespace Wall {
    public class Shuriken : Item {
        
        public float offset = 1.5F;

        public Shuriken(int count) : base(type.Shuriken, count) {
            useDelay = 0.45F;
            makeStackable();
            consumable = true;
        }

        public override void use(float angle, float distance) {
            base.use(angle, distance);
            
            Wall.projectiles.Add(new ShurikenProjectile(player.pos + Util.polar(offset, angle), Util.polar(40, angle), true));
        }
    }
}