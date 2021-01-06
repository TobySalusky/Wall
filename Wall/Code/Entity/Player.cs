using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Wall {
    public class Player : Entity {

        private bool terrariaMode = true;
        
        private float jumpHeight = 4;
        private float jumpTime;
        private const float jumpTimeStart = 0.6F;

        public bool grappleOut, grappleHit;
        public Grapple grapple;

        public bool canGrapple = true;
        public bool canNotBounce = false;

        public Item[,] inventory;
        public Array2DView<Item> hotbar;
        public bool inventoryOpen = true;

        public Armor[] armor;
        
        public int selectedItemIndex;
        
        public Item currentItem;

        public Player(Vector2 pos) : base(pos) {
            
            speed = 25F;
            initHealth(50);
            Item.player = this;
            
            inventory = new Item[9, 4];
            armor = new Armor[3];
            hotbar = new Array2DView<Item>(inventory, 0);

            hotbar[0] = new FrostSword(1);
            hotbar[1] = new Bow(1);
            hotbar[2] = new Shuriken(99);
            hotbar[3] = new SnowBall(4);
            hotbar[4] = new RubberArrow(5);
            hotbar[5] = new Arrow(99);
            hotbar[6] = new Flamethrower(99);
            
            armor[0] = new YotsugiHat();
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

        public Item firstItem(Predicate<Item> pred) {
            for (int y = 0; y < inventory.GetLength(1); y++) {
                for (int x = 0; x < inventory.GetLength(0); x++) {
                    Item item = inventory[x, y];
                    if (item != null && pred(item)) {
                        return item;
                    }
                }
            }

            return null;
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
            
            for (int y = 0; y < inventory.GetLength(1); y++) {
                for (int x = 0; x < inventory.GetLength(0); x++) {
                    Item item = inventory[x, y];
                    if (item != null && item.count <= 0) {
                        inventory[x, y] = null;
                    }
                }
            }

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
            Wall.deaths++;
        }

        public override float findRotation() {
            const float maxRot = (float) Math.PI * 0.4F;
            return Math.Sign(vel.X) * Math.Min(1, Math.Abs(vel.X) / 100F) * maxRot;
        }

        public void mouseInput(MouseInfo mouse, float deltaTime) {


            int newIndex = selectedItemIndex + mouse.scroll;
            if (newIndex < 0) {
                newIndex = hotbar.Length + newIndex;

                newIndex = Math.Max(newIndex, 0);
            }
            setSelectedItemIndex(newIndex % hotbar.Length);
            
            currentItem?.update(deltaTime, mouse);

            if (mouse.middlePressed) {
                Wall.entities.Add(new IceSnake(Wall.camera.toWorld(mouse.pos)));
            }

            if (mouse.rightPressed) {
                Wall.entities.Add(new SnowSlime(Wall.camera.toWorld(mouse.pos)));
            }

        }

        public void keyInput(MouseState mouseState, KeyInfo keys, float deltaTime) {
            
            Vector2 diff = new Vector2(mouseState.X, mouseState.Y) - Wall.camera.toScreen(pos);

            int inputX = 0;

            if (keys.pressed(Keys.T)) {
                terrariaMode = !terrariaMode;
            }
            
            if (keys.pressed(Keys.Tab)) {
                inventoryOpen = !inventoryOpen;
            }
            
            if (keys.pressed(Keys.E) && !grappleOut) {
                grappleOut = true;
                Wall.entities.Add(new Grapple(this, pos, Util.polar(150F, Util.angle(diff))));
            }

            if (keys.pressed(Keys.D1))
                setSelectedItemIndex(0);
            if (keys.pressed(Keys.D2))
                setSelectedItemIndex(1);
            if (keys.pressed(Keys.D3))
                setSelectedItemIndex(2);
            if (keys.pressed(Keys.D4))
                setSelectedItemIndex(3);
            if (keys.pressed(Keys.D5))
                setSelectedItemIndex(4);
            if (keys.pressed(Keys.D6))
                setSelectedItemIndex(5);
            if (keys.pressed(Keys.D7))
                setSelectedItemIndex(6);
            if (keys.pressed(Keys.D8))
                setSelectedItemIndex(7);
            if (keys.pressed(Keys.D9))
                setSelectedItemIndex(8);



            if (keys.down(Keys.A))
                inputX--;
            if (keys.down(Keys.D))
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

                if (grounded && jumpPressed(keys) && jumpTime < jumpTimeStart - 0.1F) {

                    jump(jumpHeight);
                }

                if (!grounded && jumpPressed(keys) && jumpTime > 0) {
                    float fade = jumpTime / jumpTimeStart;
                    vel.Y -= 50F * deltaTime * fade;
                }
            }
            
            if (grappleOut && ((keys.up(Keys.E)&&!terrariaMode)||(keys.pressed(Keys.Space)&&terrariaMode))) {

                grapple.deleteFlag = true;
                grapple = null;
                grappleOut = false;
                grappleHit = false;
                hasGravity = true;
                canGrapple = true;
            }

            if (keys.up(Keys.E) && !terrariaMode) {
                canGrapple = true;
            }

            if (keys.down(Keys.Space)) {
                tryWallBounce();
            }
        }

        public void tryWallBounce() {

            bool bounced = false;
            
            for (int dir = -1; dir <= 1; dir += 2) {
                if (collidesAt(pos + (Vector2.UnitX * 0.2F) * dir, dimen)) {
                    if (!collidesAt(pos + Vector2.UnitY * 2) && !canNotBounce) {
                        bounced = true;
                        
                        vel.X = -30 * dir;
                        if (vel.Y > -25)
                        {
                            vel.Y = -25;
                        }

                        if (grappleOut) {
                            grapple.deleteFlag = true;
                            grapple = null;
                            grappleOut = false;
                            grappleHit = false;
                            hasGravity = true;
                            if (!terrariaMode)
                            {
                                canGrapple = false;
                            }
                        }

                        break;
                    }
                }
            }

            if (!bounced) {
                canNotBounce = false;
            }
        }

        public bool jumpPressed(KeyInfo keys) {
            return keys.down(Keys.W) || keys.down(Keys.Space);
        }

        public override void render(Camera camera, SpriteBatch spriteBatch) {
            base.render(camera, spriteBatch);
            
            foreach (var piece in armor) {
                piece?.renderWearing(camera, spriteBatch);
            }
            
            currentItem?.render(camera, spriteBatch);

            renderUI(camera, spriteBatch);
        }

        public void renderSlot(Item item, Rectangle rect, Camera camera, SpriteBatch spriteBatch, bool isSelected = false) {
            Texture2D itemSlot = Textures.get("ItemSlot");
            if (isSelected) {
                spriteBatch.Draw(itemSlot, rect, new Color(Color.White, 0.6F));
            }
            else {
                spriteBatch.Draw(itemSlot, rect, new Color(Color.White, 0.3F));
            }

            if (item != null) {
                spriteBatch.Draw(item.texture, Util.useRatio(item.dimen, rect), Color.White);

                if (item.maxStack != 1) {
                    spriteBatch.DrawString(Fonts.arial, "" + item.count, new Vector2(rect.Left + 40, rect.Top + 40), Color.White);
                }
            }
        }

        public void renderUI(Camera camera, SpriteBatch spriteBatch) {
            
            Texture2D itemSlot = Textures.get("ItemSlot");


            int x = 20, y = 20;
            if (inventoryOpen) { 
                // inventory
                for (int j = 0; j < inventory.GetLength(1); j++) {
                    for (int i = 0; i < inventory.GetLength(0); i++) {
                        Rectangle rect = new Rectangle(x, y, 64, 64);

                        renderSlot(inventory[i, j], rect, camera, spriteBatch, j == 0 && i == selectedItemIndex);

                        x += 70;
                    }

                    x = 20;
                    y += 70;
                }
            } else {
                // hotbar
                for (int i = 0; i < hotbar.Length; i++) {
                    Rectangle rect = new Rectangle(x, y, 64, 64);

                    renderSlot(hotbar[i], rect, camera, spriteBatch, i == selectedItemIndex);

                    x += 70;
                }
            }

            // health bar
            Rectangle healthRect = new Rectangle(1500, 10, 300, 30);
            spriteBatch.Draw(itemSlot, healthRect, new Color(Color.White, 0.4F));
            spriteBatch.Draw(Textures.get("HealthBar"), new Rectangle(healthRect.X, healthRect.Y, (int) (healthRect.Width * (health / maxHealth)), healthRect.Height), 
                new Color(Color.White, 0.4F));
            

        }
    }
}