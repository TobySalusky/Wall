using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Wall {
    public class Flamethrower : Item {
        
        public Flamethrower(int count) : base(count) {
            useDelay = 0.01F;
            offset = 2F;

            allwaysRender = true;
            maxSpecialChargeTime = 1F;
            handOffset = new Vector2(-0.3F, 0.3F);
        }

        public override void useSpecial(float angle, float mag) {
            base.useSpecial(angle, mag);
            if (canUse()) {
                useTimer = useDelay;
                specialUse = true;
                if (specialChargeAmount() > 0.5F) {
                    
                    player.vel -= Util.polar(specialChargeAmount() * 30, angle);
                    
                    for (int i = 0; i < 200 * specialChargeAmount(); i++) {

                        float velAngle = angle + Util.randomPN() * Maths.PI * 0.15F;
                        float dot = Vector2.Dot(Util.polar(1, angle), Util.polar(1, velAngle));
                        Vector2 vel = Util.polar(Util.random(15, 20 + 50 * (float) Math.Pow(dot, 60)) * specialChargeAmount(),
                            velAngle);
                        
                        Wall.projectiles.Add(new PixelFlame(player.pos + Util.polar(offset + dimen.X / 2, angle),
                                vel, true)
                            {tint = new Color(1F, Util.random(), 0F)});
                    }
                }
            }
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