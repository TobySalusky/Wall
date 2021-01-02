using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Wall {
    public class Sword : Item {

        public Projectile[] chunks;
        public int chunkCount;
        public float chunkSize;
        public float damage;
        public float knockback;

        public float angle, offset;

        public float swingTime;
        public int swingDir;

        public Sword(type itemType, int count) : base(itemType, count) {

            swingTime = useDelay;
        }

        public override bool canUse() {
            return base.canUse() && chunks == null;
        }

        public override void update(float deltaTime, MouseInfo mouse) {
            base.update(deltaTime, mouse);

            if (isUsing()) {
                positionChunks(deltaTime);
            } else {
                if (chunks != null) {
                    foreach (var chunk in chunks) {
                        chunk.deleteFlag = true;
                    }
                    chunks = null;
                }
            }
        }

        public override void use(float angle, float distance) {
            base.use(angle, distance);

            swingDir = player.facingLeft ? -1 : 1;
            
            this.angle = -Maths.halfPI - Maths.PI * 0.2F * swingDir;

            var hasHit = new List<Entity>();

            chunks = new Projectile[chunkCount];
            for (int i = 0; i < chunkCount; i++) {
                chunks[i] = new MeleeAttack(player.pos, true) {hasHit = hasHit, damage = damage, knockback = knockback, dimen = Vector2.One * chunkSize};
                Wall.projectiles.Add(chunks[i]);
            }
        }

        public float findSwingSpeed() {
            return Maths.PI * 0.85F / swingTime;
        }

        public void positionChunks(float deltaTime) {
            angle += deltaTime * findSwingSpeed() * swingDir;

            Vector2 start = player.pos + Util.polar(offset, angle);
            
            for (int i = 0; i < chunks.Length; i++) {
                chunks[i].pos = start + Util.polar(chunkSize, angle) * i;
            }
        }

        public override void renderInHand(Camera camera, SpriteBatch spriteBatch) {
            
            Vector2 pos = player.pos + Util.polar((offset + chunkCount * chunkSize) / 2, angle);

            float renderAngle = (swingDir == -1) ? angle + Maths.PI / 4 : angle + 3/4F * Maths.PI;
            Util.render(texture, pos, dimen, renderAngle, camera, spriteBatch, swingDir == -1);
        }
    }
}