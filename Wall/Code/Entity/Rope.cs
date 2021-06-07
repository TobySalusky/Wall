using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Wall {
    public class Rope : Entity {
        
        public Player user;
        
        public bool hit;
        public static readonly Texture2D rope;
        
        static Rope() {
            rope = Textures.get("grapple_chain");
        }
        
        public Rope(Player user, Vector2 pos, Vector2 vel) : base(pos) {
            this.vel = vel;
            hasStep = false;
            this.user = user;
            user.Rope = this;
            useSpawnSlot = false;

            rotation = Util.angle(vel) + (float) Math.PI / 2;

            texture = Textures.get("grapple");
            dimen = new Vector2(texture.Width, texture.Height) * Tile.pixelSize;

            canDespawn = false;
            invincible = true;
        }

        /**
         * No clue what this is for
         */
        public override void die() {
            Logger.log("Rope's die() was called");
        }

        /**
         * why is this called bonk
         * ifCollision
         */
        public override void bonk(Vector2 newPos) {
            if (!hit) {
                hit = true;
                user.grappleHit = true;
            }
            else {
                
                //TODO put in complicated rope logic
                
            }
        }

        /**
         * updates rope as long as it hasnt hit
         */
        public override void update(float deltaTime) {

            if (!hit)
                collisionMove(vel * deltaTime);
        }

        /**
         * Renders the rope
         */
        public override void render(Camera camera, SpriteBatch spriteBatch) {

            Vector2 diff = pos - user.pos;
            float chainAngle = Util.angle(diff);
            Vector2 chainSize = new Vector2(5, 9) / 8F;

            Vector2 step = Util.polar(chainSize.Y, chainAngle);

            for (int i = 0; i < Util.mag(diff) / chainSize.Y; i++) {
                Util.render(rope, pos - step * i, chainSize, chainAngle + (float) Math.PI / 2, camera, spriteBatch);
            }
            
            base.render(camera, spriteBatch);
        }
    }
    
}