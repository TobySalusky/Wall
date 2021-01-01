using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Wall {
    public class Grapple : Entity {

        public Player user;
        public bool hit;
        
        public Grapple(Player user, Vector2 pos, Vector2 vel) : base(pos) {
            this.vel = vel;
            hasStep = false;
            hasGravity = false;
            this.user = user;
            user.grapple = this;
            dimen = new Vector2(9 / 8F, 1);

            rotation = Util.angle(vel) + (float) Math.PI / 2;

            texture = Textures.get("grapple");
        }

        public override void update(float deltaTime) {

            if (!hit && collidesAt(pos, dimen * 1.3F)) {
                hit = true;
                vel = Vector2.Zero;
                user.grappleHit = true;
                user.hasGravity = false;
            }
            
            collisionMove(vel * deltaTime);
        }

        public override void render(Camera camera, SpriteBatch spriteBatch) {

            Vector2 diff = pos - user.pos;
            float chainAngle = Util.angle(diff);
            Vector2 chainSize = new Vector2(5, 9) / 8F;
            Texture2D chain = Textures.get("grapple_chain");

            Vector2 step = Util.polar(chainSize.Y, chainAngle);

            for (int i = 0; i < Util.mag(diff) / chainSize.Y; i++) {
                Util.render(chain, pos - step * i, chainSize, chainAngle + (float) Math.PI / 2, camera, spriteBatch);
            }
            
            base.render(camera, spriteBatch);
        }
    }
}