using Microsoft.Xna.Framework;

namespace Wall {
    public class SnowWorm : WormHead {
        
        public SnowWorm(Vector2 pos) : base(pos) {}

        public override void setup() {
            segmentCount = 15;
            segmentOffset = 1.5F;
        }
    }
}