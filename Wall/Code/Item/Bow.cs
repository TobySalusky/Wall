using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Wall {
    public class Bow : Item {

        public float angle, velocity, offset = 1.5F;
        
        public Bow(int count) : base(type.Bow, count) {
            useDelay = 0.5F;
            velocity = 70;

            allwaysRender = true;
        }

        public override void use(float angle, float distance) {
            base.use(angle, distance);
            
            Wall.projectiles.Add(new Arrow(player.pos + Util.polar(offset, angle), Util.polar(velocity, angle), true));
        }

        public override void update(float deltaTime, MouseInfo mouse) {
            base.update(deltaTime, mouse);
            angle = Util.angle(mouse.pos - Wall.camera.toScreen(player.pos));
        }

        public override void renderInHand(Camera camera, SpriteBatch spriteBatch) {

            Vector2 pos = player.pos + Util.polar((offset), angle);

            float renderAngle = (player.facingLeft) ? angle + Maths.PI / 4F : angle + Maths.PI * 3 / 4F;
            Util.render(texture, pos, dimen, renderAngle, camera, spriteBatch, !player.facingLeft);
        }
    }
}