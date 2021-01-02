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

        public override void die() {
            base.die();
            
            dropItem(new SnowBall(Util.randInt(1, 4)));

            Color[] colorArray = Util.colorArray(texture);
            
            Vector2 tMid = new Vector2(texture.Width, texture.Height) / 2;
            float angle = Util.angle(vel);
            float mag = Math.Clamp(Util.mag(vel) / 10, 3, 7);
            
            for (int i = 0; i < texture.Width; i++) {
                for (int j = 0; j < texture.Height; j++) {
                    int index = i + j * texture.Width;

                    Color color = colorArray[index];

                    if (color.A != 0) {
                        Vector2 add = (new Vector2(i, j) - tMid) * Tile.pixelSize;
                        if (!facingLeft) {
                            add.X *= -1;
                        }

                        Vector2 pPos = pos + add;

                        Particle particle = new SnowPixel(pPos,
                            Util.polar(Util.random(0.9F, 5F) * mag, angle + Util.random(-0.1F, 0.1F)), color);
                        
                        Wall.particles.Add(particle);
                    }
                }
            }
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

        public override void attackTick(float deltaTime) {
            meleeAttack(player, 7, 40);
        }
    }
}