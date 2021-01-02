using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Wall {
    public class Player : Entity {
        
        private float jumpHeight = 4;
        private float jumpTime;
        private const float jumpTimeStart = 0.6F;

        public bool grappleOut, grappleHit;
        public Grapple grapple;

        public Item[,] inventory;
        public Array2DView<Item> hotbar;
        
        public int selectedItemIndex;
        
        public Item currentItem;

        public Player(Vector2 pos) : base(pos) {
            
            speed = 25F;
            initHealth(30);
            Item.player = this;
            
            inventory = new Item[9, 4];
            hotbar = new Array2DView<Item>(inventory, 0);

            hotbar[0] = new FrostSword(1);
            hotbar[1] = new Bow(1);
            hotbar[2] = new Shuriken(99);
            hotbar[3] = new SnowBall(99);
        }

        public void tryPickUp(GroundItem ground) {

            Item item = ground.item;
            Item.type type = item.itemType;
            
            // first tries to stack the item with others
            for (int y = 0; y < inventory.GetLength(1); y++) {
                for (int x = 0; x < inventory.GetLength(0); x++) {
                    Item thisItem = inventory[x, y];
                    if (thisItem != null && thisItem.itemType == type) {
                        int canTake = thisItem.maxStack - thisItem.count;
                        int take = Math.Min(item.count, canTake);

                        item.count -= take;
                        thisItem.count += take;
                    }
                }
            }

            if (item.count == 0) {
                ground.deleteFlag = true;
            } else if (item.count < 0) {
                Console.WriteLine("Something clearly went wrong here, you see " + item + " wound up with a count less than 0");
            } else {
                // checks for empty slots
                for (int y = 0; y < inventory.GetLength(1); y++) {
                    for (int x = 0; x < inventory.GetLength(0); x++) {
                        Item thisItem = inventory[x, y];
                        if (thisItem == null) {
                            inventory[x, y] = item;
                            ground.deleteFlag = true;
                            return;
                        }
                    }
                }
            }


        }

        public void setSelectedItemIndex(int index) {

            if (index != selectedItemIndex) {
                hotbar[selectedItemIndex]?.switchOff();
            }

            selectedItemIndex = index;
        }

        public override void jump(float jumpHeight) {
            base.jump(jumpHeight);
            jumpTime = jumpTimeStart;
        }

        public override void update(float deltaTime) {

            currentItem = hotbar[selectedItemIndex];
            
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

        public void mouseInput(MouseState state, bool leftChange, bool middleChange, bool rightChange, int scroll, float deltaTime) {

            MouseInfo mouse = new MouseInfo(state, leftChange, middleChange, rightChange);

            int newIndex = selectedItemIndex + scroll;
            if (selectedItemIndex < 0) {
                newIndex = hotbar.Length + newIndex;
                newIndex = Math.Max(newIndex, 0);
            }
            setSelectedItemIndex(newIndex % hotbar.Length);
            
            Vector2 diff = mouse.pos - Wall.camera.toScreen(pos);
            
            currentItem?.update(deltaTime, mouse);

            if (state.RightButton == ButtonState.Pressed && rightChange) {
                Wall.entities.Add(new SnowSlime(Wall.camera.toWorld(mouse.pos)));
            }

        }

        public void keyInput(MouseState mouseState, KeyboardState state, float deltaTime) {
            
            Vector2 diff = new Vector2(mouseState.X, mouseState.Y) - Wall.camera.toScreen(pos);

            int inputX = 0;

            if (state.IsKeyDown(Keys.E) && !grappleOut) {
                grappleOut = true;
                Wall.entities.Add(new Grapple(this, pos, Util.polar(150F, Util.angle(diff))));
            }

            if (state.IsKeyDown(Keys.D1))
                setSelectedItemIndex(0);
            if (state.IsKeyDown(Keys.D2))
                setSelectedItemIndex(1);
            if (state.IsKeyDown(Keys.D3))
                setSelectedItemIndex(2);
            if (state.IsKeyDown(Keys.D4))
                setSelectedItemIndex(3);
            if (state.IsKeyDown(Keys.D5))
                setSelectedItemIndex(4);
            if (state.IsKeyDown(Keys.D6))
                setSelectedItemIndex(5);
            if (state.IsKeyDown(Keys.D7))
                setSelectedItemIndex(6);
            if (state.IsKeyDown(Keys.D8))
                setSelectedItemIndex(7);
            if (state.IsKeyDown(Keys.D9))
                setSelectedItemIndex(8);
            
            

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

                if (grounded && jumpPressed(state) && jumpTime < jumpTimeStart - 0.1F) {

                    jump(jumpHeight);
                }

                if (!grounded && jumpPressed(state) && jumpTime > 0) {
                    float fade = jumpTime / jumpTimeStart;
                    vel.Y -= 50F * deltaTime * fade;
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

        public override void render(Camera camera, SpriteBatch spriteBatch) {
            base.render(camera, spriteBatch);
            currentItem?.render(camera, spriteBatch);

            renderUI(camera, spriteBatch);
        }

        public void renderUI(Camera camera, SpriteBatch spriteBatch) {
            
            
            // hotbar
            int x = 20, y = 20;
            Texture2D itemSlot = Textures.get("ItemSlot");
            for (int i = 0; i < hotbar.Length; i++) {
                Rectangle rect = new Rectangle(x, y, 64, 64);

                if (i == selectedItemIndex) {
                    spriteBatch.Draw(itemSlot, rect, new Color(Color.White, 0.6F));
                }
                else {
                    spriteBatch.Draw(itemSlot, rect, new Color(Color.White, 0.3F));
                }

                Item item = hotbar[i];
                if (item != null) {
                    spriteBatch.Draw(hotbar[i].texture, rect, Color.White);

                    if (item.maxStack != 1) {
                        spriteBatch.DrawString(Fonts.arial, "" + item.count, new Vector2(x + 40, y + 40), Color.White);
                    }
                }

                x += 70;
            }

        }
    }
}