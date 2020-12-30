using Microsoft.Xna.Framework;

namespace Wall {
    public class SnowSlime : Entity {
        
        public SnowSlime(Vector2 pos) : base(pos) {
            dimen = Vector2.One * 2;
        }

        public override void update(float deltaTime) {
            base.update(deltaTime);
        }
    }
}