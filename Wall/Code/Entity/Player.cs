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

        public Player(Vector2 pos) : base(pos) {
            
            speed = 25F;
            initHealth(30);
            
        }

        public void snowImpactPuff(int count, float intensity, Vector2 pos, Tile tileOn) {
            for (int i = 0; i < count; i++) {
                Particle puff = new SnowPixel(pos + new Vector2(Util.random(-1, 1) * dimen.X * 0.3F, dimen.Y / 2), new Vector2(Util.random(-2, 2) * (1 + intensity), -Util.random(0.3F, 1.3F)), Util.randomColor(tileOn.texture));
                Wall.particles.Add(puff);
            }
        }

        public override void bonkY(Vector2 newPos) { 

            Tile tileOn = getTileOn(newPos);
            if (tileOn.tileType == Tile.type.snow) { //TODO: for some reason causes particles when program is unfocused?!!
                
                float intensity = Math.Min(vel.Y * 0.4F / 30, 1);

                int count = (int) (intensity * 30);
                
                snowImpactPuff(count, intensity, newPos, tileOn);
            }

            base.bonkY(newPos);
        }

        public Tile getTileOn() {
            return getTileOn(pos);
        }

        public Tile getTileOn(Vector2 pos) {
            return Wall.map.getTile(pos + Vector2.UnitY * (dimen.Y / 2 + 0.1F));
        }

        public override void update(float deltaTime) {

            Tile tileOn = getTileOn();

            if (tileOn.tileType == Tile.type.snow) {
                float signX = Math.Sign(vel.X), mag = vel.X * signX;

                if (mag > 1) {
                    if (Util.chance(deltaTime * 10)) {
                        Particle puff = new SnowPixel(pos + new Vector2(Util.random(-1, 1) * dimen.X / 4, dimen.Y / 2), new Vector2(-signX * Math.Clamp(mag / 10, 1, 5), -Util.random(0.3F, 1.3F)), Util.randomColor(tileOn.texture));
                        Wall.particles.Add(puff);
                    }
                }
            }

            if (grappleHit) { // grapple movement
                Vector2 accel = Vector2.Normalize(grapple.pos - pos) * 90;
                //accel += Vector2.UnitY * gravity;
                vel += accel * deltaTime;
            }
            
            base.update(deltaTime);
        }

        public override void die() {
            health = maxHealth;
        }

        public override float findRotation() {
            const float maxRot = (float) Math.PI * 0.4F;
            return Math.Sign(vel.X) * Math.Min(1, Math.Abs(vel.X) / 100F) * maxRot;
        }

        public void mouseInput(MouseState state, bool leftChange, bool middleChange, bool rightChange, float deltaTime) {

            Vector2 mousePos = new Vector2(state.X, state.Y);
            Vector2 diff = mousePos - Wall.camera.toScreen(pos);
            
            if (state.LeftButton == ButtonState.Pressed && leftChange) {
                Wall.projectiles.Add(new Shuriken(pos, Vector2.Normalize(diff) * 40, true));
            }

            if (state.RightButton == ButtonState.Pressed && rightChange) {
                Wall.entities.Add(new SnowSlime(Wall.camera.toWorld(mousePos)));
            }

        }

        public void jump() {
            vel.Y -= jumpSpeed;
            jumpTime = jumpTimeStart;

            Tile tileOn = getTileOn();
            if (tileOn.tileType == Tile.type.snow) {
                snowImpactPuff(10, 0.5F, pos, tileOn);
            }
        }

        public void keyInput(MouseState mouseState, KeyboardState state, float deltaTime) {
            
            Vector2 diff = new Vector2(mouseState.X, mouseState.Y) - Wall.camera.toScreen(pos);

            int inputX = 0;

            if (state.IsKeyDown(Keys.E) && !grappleOut) {
                grappleOut = true;
                Wall.entities.Add(new Grapple(this, pos, Util.polar(150F, Util.angle(diff))));
            }

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

                    jump();
                }

                if (!grounded && jumpPressed(state) && jumpTime > 0) {
                    float fade = jumpTime / jumpTimeStart;
                    vel.Y -= jumpSpeed * 2F * deltaTime * fade;
                }
            }
            
            if (grappleOut && state.IsKeyDown(Keys.Space)) {
                grapple.deleteFlag = true;
                grapple = null;
                grappleOut = false;
                grappleHit = false;
                hasGravity = true;
            }
        }

        public bool jumpPressed(KeyboardState state) {
            return state.IsKeyDown(Keys.W) || state.IsKeyDown(Keys.Space);
        }
    }
}