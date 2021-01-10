using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Wall {
    public class Sword : MeleeWeapon {

        public int chunkCount;
        public float chunkSize;
        
        public float swingTime;
        public int swingDir;

        public Sword(int count) : base(count) {

            swingTime = useDelay;
            maxSpecialChargeTime = 0.5F;
        }

        public override void useSpecial(float angle, float mag) {

            if (canUse()) {
                specialUse = true;
                use(angle, mag);
                player.vel += Util.polar(specialChargeAmount() * 20, angle);
            }
        }

        public override void holdSpecial(float deltaTime, MouseInfo mouse) {
            base.holdSpecial(deltaTime, mouse);
            
            swingDir = player.facingLeft ? -1 : 1;
            float diff = - Maths.PI * 0.2F * swingDir;
            angle = -Maths.halfPI + diff * specialChargeAmount();
        }

        public override bool canUse() {
            return base.canUse() && chunks == null;
        }

        public override void use(float angle, float distance) {
            base.use(angle, distance);

            swingDir = player.facingLeft ? -1 : 1;
            
            this.angle = -Maths.halfPI - Maths.PI * 0.2F * swingDir;

            var hasHit = new List<Entity>();

            float mult = (specialUse) ? 1 + specialChargeAmount() : 1;
            
            chunks = new MeleeAttack[chunkCount];
            for (int i = 0; i < chunkCount; i++) {
                chunks[i] = new MeleeAttack(player.pos, true) {hasHit = hasHit, damage = damage * mult, knockback = knockback, dimen = Vector2.One * chunkSize};
                Wall.projectiles.Add(chunks[i]);
            }
        }

        public float findSwingSpeed() {
            return Maths.PI * 0.85F / swingTime;
        }

        public override void positionChunks(float deltaTime) {
            angle += deltaTime * findSwingSpeed() * swingDir;

            Vector2 start = player.pos + Util.polar(offset, angle);
            
            for (int i = 0; i < chunks.Length; i++) {
                chunks[i].pos = start + Util.polar(chunkSize, angle) * i;
                chunks[i].knockbackAngle = angle;
            }
        }

        public override void renderInHand(Camera camera, SpriteBatch spriteBatch) {
            
            Vector2 pos = player.pos + Util.polar((offset + chunkCount * chunkSize) / 2, angle);

            float renderAngle = (swingDir == -1) ? angle + Maths.PI / 4 : angle + 3/4F * Maths.PI;
            Util.render(texture, pos, dimen, renderAngle, camera, spriteBatch, swingDir == -1);
        }
    }
}