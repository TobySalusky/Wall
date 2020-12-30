using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Wall {
    public class Player : Entity {
        
        private float jumpSpeed = 25;
        private float jumpTime;
        private const float jumpTimeStart = 0.6F;
        
        public Player(Vector2 pos) : base(pos) {}

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

            float accelSpeed = (inputX == 0) ? 5 : 2.5F; 
            vel.X += ((inputX * speed) - vel.X) * deltaTime * accelSpeed;

            jumpTime -= deltaTime;
            if (grounded && jumpPressed(state) && jumpTime < jumpTimeStart - 0.5F) {
                vel.Y -= jumpSpeed;
                jumpTime = jumpTimeStart;
            }

            if (!grounded && jumpPressed(state) && jumpTime > 0) {
                float fade = jumpTime / jumpTimeStart;
                vel.Y -= jumpSpeed * 2F * deltaTime * fade;
            }
        }

        public bool jumpPressed(KeyboardState state) {
            return state.IsKeyDown(Keys.W) || state.IsKeyDown(Keys.Space);
        }
    }
}