using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Wall {
    public class Grapple : Entity {

        public Player user;
        public bool hit;
        public Texture2D chain;
        
        public Grapple(Player user, Vector2 pos, Vector2 vel) : base(pos) {
            this.vel = vel;
            hasStep = false;
            hasGravity = false;
            this.user = user;
            user.grapple = this;

            rotation = Util.angle(vel) + (float) Math.PI / 2;

            texture = Textures.get("grapple");
            dimen = new Vector2(texture.Width, texture.Height) * Tile.pixelSize;
            
            chain = Textures.get("grapple_chain");
        }

        public override void die() {
        }

        public override void bonk(Vector2 newPos) {
            if (!hit) {
                hit = true;
                user.grappleHit = true;
                user.hasGravity = false;
            }
        }

        public override void update(float deltaTime) {

            if (!hit)
                collisionMove(vel * deltaTime);
        }

        public override void render(Camera camera, SpriteBatch spriteBatch) {

            Vector2 diff = pos - user.pos;
            float chainAngle = Util.angle(diff);
            Vector2 chainSize = new Vector2(5, 9) / 8F;

            Vector2 step = Util.polar(chainSize.Y, chainAngle);

            for (int i = 0; i < Util.mag(diff) / chainSize.Y; i++) {
                Util.render(chain, pos - step * i, chainSize, chainAngle + (float) Math.PI / 2, camera, spriteBatch);
            }
            
            base.render(camera, spriteBatch);
        }
    }
}