using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Wall {
    public class YotsugiHat : Armor {

        public static Texture2D body, leftEar, rightEar;
        public float leftAngle, rightAngle;
        public static Vector2 bodyDimen, earDimen;

        static YotsugiHat() { 
            body = Textures.get("YotsugiHatBody");
            leftEar = Textures.get("YotsugiHatEarLeft");
            rightEar = Textures.get("YotsugiHatEarRight");
            
            bodyDimen = Util.dimen(body);
            earDimen = Util.dimen(leftEar);
        }

        public YotsugiHat(int count) : base(count) {
            wearOffset = -Vector2.UnitY * player.dimen.Y / 2;
        }

        public override void renderWearing(Camera camera, SpriteBatch spriteBatch) {
            Vector2 flip = new Vector2((player.facingLeft) ? 1 : -1, 1);
            
            Vector2 leftOff = new Vector2(-0.2F, -0.3F) * flip;
            Vector2 rightOff = new Vector2(0.2F, -0.3F) * flip;
            
            Vector2 leftPos = player.pos + Util.rotate(wearOffset + leftOff, player.rotation);
            Vector2 rightPos = player.pos + Util.rotate(wearOffset + rightOff, player.rotation);
            
            // TODO: move to update
            const float maxRot = (float) Math.PI * 0.4F;
            int dir = Math.Sign(player.vel.X);
            leftAngle = -dir * Math.Min(1, Math.Abs(player.vel.X) / 100F) * maxRot;
            rightAngle = leftAngle;

            if (dir == -1) {
                rightAngle *= 2F;
            } else if (dir == 1) {
                leftAngle *= 2F;
            }

            Vector2 origin = new Vector2(0.5F, 1) * Util.textureVec(leftEar);
            
            Util.render(leftEar, leftPos, earDimen, (player.facingLeft) ? leftAngle : rightAngle, camera, spriteBatch, player.facingLeft, origin);
            Util.render(rightEar, rightPos, earDimen, (player.facingLeft) ? rightAngle : leftAngle, camera, spriteBatch, player.facingLeft, origin);

            Util.render(wearingTexture(), player.pos + Util.rotate(wearOffset, player.rotation), bodyDimen, player.rotation, camera, spriteBatch, player.facingLeft);
        }

        public override Texture2D wearingTexture() {
            return body;
        }
    }
}