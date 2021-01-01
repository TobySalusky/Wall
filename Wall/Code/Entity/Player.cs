using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Wall {
    public class Player : Entity {
        
        private float jumpSpeed = 25;
        private float jumpTime;
        private const float jumpTimeStart = 0.6F;

        public bool grappleOut, grappleHit;
        public Grapple grapple;
        
        public Player(Vector2 pos) : base(pos) {}

        public override void update(float deltaTime) {

            if (grappleHit) { // grapple movement
                Vector2 accel = Vector2.Normalize(grapple.pos - pos) * 90;
                //accel += Vector2.UnitY * gravity;
                vel += accel * deltaTime;
            }
            
            base.update(deltaTime);
        }

        public void mouseInput(MouseState state, float deltaTime) {

            if (state.LeftButton == ButtonState.Pressed && !grappleOut) {
                grappleOut = true;
                Vector2 diff = new Vector2(state.X, state.Y) - new Vector2(1920, 1080) / 2; // TODO: use camera to translate player location into center
                Wall.entities.Add(new Grapple(this, pos, Util.polar(150F, Util.angle(diff))));
            }

        }

        public void keyInput(KeyboardState state, float deltaTime) {
            int inputX = 0;

            if (state.IsKeyDown(Keys.A))
                inputX--;
            if (state.IsKeyDown(Keys.D))
                inputX++;

            if (inputX > 0) {
                facingLeft = false;
            } else if (inputX < 0) {
                facingLeft = true;
            }

            jumpTime -= deltaTime;
            
            if (!grappleHit) {
                float accelSpeed = (inputX == 0 && grounded) ? 5 : 2.5F;
                vel.X += ((inputX * speed) - vel.X) * deltaTime * accelSpeed;

                if (grounded && jumpPressed(state) && jumpTime < jumpTimeStart - 0.5F) {
                    vel.Y -= jumpSpeed;
                    jumpTime = jumpTimeStart;
                }

                if (!grounded && jumpPressed(state) && jumpTime > 0) {
                    float fade = jumpTime / jumpTimeStart;
                    vel.Y -= jumpSpeed * 2F * deltaTime * fade;
                }
            } else {
                if (state.IsKeyDown(Keys.Space)) {
                    grapple.deleteFlag = true;
                    grapple = null;
                    grappleOut = false;
                    grappleHit = false;
                    hasGravity = true;
                }
            }
        }

        public bool jumpPressed(KeyboardState state) {
            return state.IsKeyDown(Keys.W) || state.IsKeyDown(Keys.Space);
        }
    }
}