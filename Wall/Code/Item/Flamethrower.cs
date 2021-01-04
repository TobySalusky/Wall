using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Wall {
    public class Flamethrower : Item {
        
        public Flamethrower(int count) : base(count) {
            useDelay = 0.01F;
            offset = 2F;
        }

        public override void update(float deltaTime, MouseInfo mouse) {
            base.update(deltaTime, mouse);
            angle = Util.angle(mouse.pos - Wall.camera.toScreen(player.pos));
        }

        public override void use(float angle, float distance) {
            base.use(angle, distance);
            
            Wall.projectiles.Add(new PixelFlame(player.pos + Util.polar(offset + dimen.X / 2, angle), 
                Util.polar(Util.random(25, 35), angle + Util.randomPN() * Maths.PI * 0.05F), true) {tint = new Color(1F, Util.random(), 0F)});
        }
        
        public override void renderInHand(Camera camera, SpriteBatch spriteBatch) {

            Vector2 pos = player.pos + Util.polar((offset), angle);

            bool facingLeft = Util.angleDir(angle);
            float renderAngle = facingLeft ? angle + Maths.PI : angle;

            Util.render(texture, pos, dimen, renderAngle, camera, spriteBatch, !Util.angleDir(angle));
        }
    }
}