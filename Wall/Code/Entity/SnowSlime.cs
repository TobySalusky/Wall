using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Wall {
    public class SnowSlime : Enemy {

        private static Texture2D idle = Textures.get("SnowSlimeIdle"), look = Textures.get("SnowSlimeLook");
        private static Texture2D[] frames;

        public float chargeTimer = -1;
        public int chargeDir;
        public bool jumped;

        static SnowSlime() {
            frames = new Texture2D[] {
                Textures.get("SnowSlimeMove1"),
                Textures.get("SnowSlimeMove2"),
                Textures.get("SnowSlimeMove3"),
                Textures.get("SnowSlimeMove4"),
                Textures.get("SnowSlimeMove3"),
                Textures.get("SnowSlimeMove2"),
                Textures.get("SnowSlimeMove1")
            };
        }

        public SnowSlime(Vector2 pos) : base(pos) {
            dimen = Vector2.One * 2;

            texture = idle;

            speed = 60;
        }

        public override void update(float deltaTime) {

            Vector2 diff = player.pos - pos;

            chargeTimer -= deltaTime;

            if (chargeTimer <= -1F) {
                chargeTimer = Util.random(0.5F, 2);
                chargeDir = 0;
                jumped = false;
            }

            if (Util.mag(diff) < 40) {
                facingLeft = diff.X < 0;
                texture = look;

                if (chargeTimer <= 0) {
                    if (chargeDir == 0) {
                        chargeDir = Math.Sign(diff.X);
                    }

                    if (chargeTimer > -0.4F) {
                        vel.X += chargeDir * speed * deltaTime;

                        int index = (int) (chargeTimer / (-0.4F / 7));
                        texture = frames[index];
                    }
                    else {
                        vel.X += chargeDir * speed * 0.1F * deltaTime;
                        if (!jumped) {
                            jumped = true;

                            float jumpHeight = Math.Clamp(-diff.Y + 1, 1, 7);
                            
                            vel.Y -= Util.heightToJumpPower(jumpHeight, gravity);
                        }
                    }
                }

            } else {
                texture = idle;
            }

            float friction = (grounded) ? 2 : 1F;
            vel.X -= vel.X * deltaTime * friction;

            base.update(deltaTime);
        }
    }
}