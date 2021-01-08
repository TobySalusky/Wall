using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Wall {
    public class Spear : MeleeWeapon {

        public float tipSize, tipOffset, maxExtent, swingTime, angleSpeed = Maths.PI;
        
        public Spear(int count) : base(count) {
        }

        public override void update(float deltaTime, MouseInfo mouse) {
            base.update(deltaTime, mouse);

            if (isUsing()) {
                angle = Util.nearestAngle(angle, 0);
                angle += Math.Sign(Util.nearestAngle(mousePlayerAngle(mouse), angle) - angle) * angleSpeed * deltaTime;
                offset = maxExtent * (float) Math.Sin(Maths.PI * swingAmount());
            }
        }

        public float swingAmount() { // turns time though delay into float of 0-1
            return (swingTime - useTimer) / swingTime;
        }

        public override bool canUse() {
            return base.canUse() && useTimer <= -(useDelay - swingTime);
        }

        public override void use(float angle, float distance) {
            base.use(angle, distance);

            chunks = new [] {new MeleeAttack(player.pos, true) {damage = damage, knockback = knockback, dimen = Vector2.One * tipSize}};
            Wall.projectiles.Add(chunks[0]);

            useTimer = swingTime;

            this.angle = angle;
        }

        public override void positionChunks(float deltaTime) {
            chunks[0].pos = player.pos + Util.polar(offset + tipOffset, angle);
            chunks[0].knockbackAngle = angle;
        }
        
        public override void renderInHand(Camera camera, SpriteBatch spriteBatch) {

            Vector2 pos = player.pos + Util.polar((offset), angle);

            bool facingLeft = Util.angleDir(angle);
            float renderAngle = facingLeft ? angle + Maths.PI * 3/4: angle + Maths.PI / 4;

            Util.render(texture, pos, dimen, renderAngle, camera, spriteBatch, !Util.angleDir(angle));
        }
    }

    public class StoneSpear : Spear {
        public StoneSpear(int count) : base(count) {
            tipSize = 1.5F;
            tipOffset = 2;
            maxExtent = 5;
            
            damage = 3.5F;
            knockback = 20;
            
            offset = 1;
            useDelay = 0.8F;
            swingTime = useDelay * 0.75F;
        }
    }
}