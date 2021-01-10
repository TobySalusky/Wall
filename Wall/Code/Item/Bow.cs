using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Wall {
    public class Bow : Item {
        public Texture2D baseTexture;
        public Texture2D[] pullTextures;
        public float[] pullArrowOffsets;
        public float velocity;
        public int arrowPosIndex;
        
        public Bow(int count) : base(count) {
            useDelay = 0.5F;
            velocity = 70;
            offset = 1.5F;

            allwaysRender = true;
            baseTexture = texture;

            string name = GetType().Name;
            pullTextures = new Texture2D[3];
            for (int i = 0; i < 3; i++) {
                pullTextures[i] = Textures.get(name + "Pull" + (i + 1));
            }

            pullArrowOffsets = new float[] {1, 0.7F, 0.4F};

            maxSpecialChargeTime = 1F;
        }

        public override void holdSpecial(float deltaTime, MouseInfo mouse) {
            base.holdSpecial(deltaTime, mouse);
            
            int index = Math.Clamp((int) (((specialChargeAmount())) / (1 / 3F)) - 1, 0, 2);
            arrowPosIndex = index;
            texture = pullTextures[index];
        }

        public override void useSpecial(float angle, float mag) {
            if (canUse()) {
                specialUse = true;
                use(angle, mag);
                player.vel -= Util.polar(specialChargeAmount() * 20, angle);
            }
        }

        public override bool canUse() {
            return base.canUse() && topArrow() != null;
        }

        public Arrow topArrow() {
            return (Arrow) player.firstItem(item => Util.isClassOrSub(item, typeof(Arrow)));
        }

        public override void use(float angle, float distance) {
            base.use(angle, distance);

            Arrow arrow = topArrow();

            float mult = (specialUse) ? 1 + specialChargeAmount() : 1;
            Projectile proj = arrow.createArrow(player.pos + Util.polar(offset, angle), Util.polar(velocity * mult, angle));
            proj.damage *= mult;
            Wall.projectiles.Add(proj);
            arrow.count --;
        }

        public override void update(float deltaTime, MouseInfo mouse) {
            base.update(deltaTime, mouse);

            if (!holdingSpecial() || specialUse) {
                texture = baseTexture;
            }

            angle = Util.angle(mouse.pos - Wall.camera.toScreen(player.pos));
        }

        public override void renderInHand(Camera camera, SpriteBatch spriteBatch) {

            Vector2 pos = player.pos + Util.polar((offset), angle);

            float renderAngle = (player.facingLeft) ? angle + Maths.PI / 4F : angle + Maths.PI * 3 / 4F;
            Util.render(texture, pos, dimen, renderAngle, camera, spriteBatch, !player.facingLeft);

            if (holdingSpecial() && !specialUse) {
                Arrow arrow = topArrow();
                var (arrowTexture, arrowDimen) = (arrow.texture, arrow.dimen);
                
                Vector2 origin = new Vector2(0.5F, 1) * Util.textureVec(arrowTexture);
                
                Util.render(arrowTexture, player.pos + Util.polar(pullArrowOffsets[arrowPosIndex], angle), arrowDimen, angle + Maths.halfPI, camera, spriteBatch, false, origin);
            }
        }
    }
}