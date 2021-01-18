using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Wall {
    public class Player : Entity {

        public bool terrariaMode = true;
        
        private float jumpHeight = 4;
        private float jumpTime;
        private const float jumpTimeStart = 0.6F;

        public bool grappleOut, grappleHit;
        public Grapple grapple;

        public bool canGrapple = true;
        public bool canNotBounce = false;
        public bool canBounce = true;

        public ItemSlot[,] inventory;
        public Array2DView<ItemSlot> hotbar;
        public bool inventoryOpen;

        public ArmorSlot[] armor;
        public ItemSlot handSlot;
        
        public int selectedItemIndex;
        
        public Item currentItem;
        public List<ItemPopUp> itemPopUps;
        public static bool highlightPickups = true;

        public Player(Vector2 pos) : base(pos) {
            
            speed = 25F;
            initHealth(50);
            Item.player = this;

            handSlot = new ItemSlot();
            inventory = new ItemSlot[9, 4];
            for (int y = 0; y < inventory.GetLength(1); y++) {
                for (int x = 0; x < inventory.GetLength(0); x++) {
                    inventory[x, y] = new ItemSlot();
                }
            }
            armor = new []{new ArmorSlot(Armor.armorType.helmet), new ArmorSlot(Armor.armorType.chest), new ArmorSlot(Armor.armorType.boots)};
            

            hotbar = new Array2DView<ItemSlot>(inventory, 0);

            hotbar[0].item = Item.create(ItemType.FrostSword);
            hotbar[1].item = Item.create(ItemType.Bow);
            inventory[1,1].item = Item.create(ItemType.HighTechBow);
            hotbar[2].item = Item.create(ItemType.StunFlask, 99);
            inventory[2, 1].item = Item.create(ItemType.Shuriken, 99);
            hotbar[3].item = Item.create(ItemType.SnowBall, 30);
            hotbar[4].item = Item.create(ItemType.RubberArrow, 20);
            hotbar[5].item = Item.create(ItemType.Arrow, 99);
            hotbar[6].item = Item.create(ItemType.Flamethrower);
            hotbar[7].item = Item.create(ItemType.FryingPan);
            hotbar[8].item = Item.create(ItemType.IcicleSpear);
            
            armor[0].item = Armor.create(ItemType.YotsugiHat);

            itemPopUps = new List<ItemPopUp>();
        }

        public ItemSlot mouseToSlot(Vector2 mousePos) {
            
            Vector2 invStart = Vector2.One * 20;
            Vector2 ind = (mousePos - invStart) / 70;
            var (x, y) = ((int) Math.Floor(ind.X), (int) Math.Floor(ind.Y));
            if (x >= 0 && x < hotbar.Length) {
                if (y >= 0 && y < inventory.GetLength(1)) {
                    return inventory[x, y];
                }
            }
            
            invStart = new Vector2(20, 400);
            ind = (mousePos - invStart) / 70;
            (x, y) = ((int) Math.Floor(ind.X), (int) Math.Floor(ind.Y));

            if (x == 0 && y >= 0 && y < armor.Length) {
                return armor[y];
            }

            return null;
        }

        public void tryPickUp(GroundItem ground) {

            Item item = ground.item;
            ItemType type = item.itemType;

            int pickCount = 0;
            
            // first tries to stack the item with others
            for (int y = 0; y < inventory.GetLength(1); y++) {
                for (int x = 0; x < inventory.GetLength(0); x++) {
                    Item thisItem = inventory[x, y].item;
                    if (thisItem != null && thisItem.itemType == type) {
                        int canTake = thisItem.maxStack - thisItem.count;
                        int take = Math.Min(item.count, canTake);

                        item.count -= take;
                        thisItem.count += take;
                        if (take > 0) {
                            pickCount += take;
                        }
                    }
                }
            }

            if (item.count == 0) {
                ground.deleteFlag = true;
            } else if (item.count < 0) {
                Logger.log("Something clearly went wrong here, you see " + item + " wound up with a count less than 0");
            } else {
                // checks for empty slots
                for (int y = 0; y < inventory.GetLength(1); y++) {
                    for (int x = 0; x < inventory.GetLength(0); x++) {
                        Item thisItem = inventory[x, y].item;
                        if (thisItem == null) {
                            inventory[x, y].item = item;
                            pickCount += item.count;
                            ground.deleteFlag = true;
                            return;
                        }
                    }
                }
            }

            if (pickCount > 0 && highlightPickups) {
                Item picked = Item.create(item.itemType, pickCount);
                foreach (var popup in itemPopUps) {
                    if (popup.shouldAdd(picked)) {
                        popup.addedPick(picked);
                        return;
                    }
                }

                itemPopUps.Add(new ItemPopUp(picked));
            }
        }

        public Item firstItem(Predicate<Item> pred) {
            for (int y = 0; y < inventory.GetLength(1); y++) {
                for (int x = 0; x < inventory.GetLength(0); x++) {
                    Item item = inventory[x, y].item;
                    if (item != null && pred(item)) {
                        return item;
                    }
                }
            }

            return null;
        }

        public void setSelectedItemIndex(int index) {

            if (index != selectedItemIndex) {
                hotbar[selectedItemIndex].item?.switchOff();
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
                    Item item = inventory[x, y].item;
                    if (item != null && item.count <= 0) {
                        inventory[x, y].item = null;
                    }
                }
            }

            currentItem = hotbar[selectedItemIndex].item;

            if (grappleHit) { // grapple movement
                Vector2 accel = Vector2.Normalize(grapple.pos - pos) * 90;
                //accel += Vector2.UnitY * gravity;
                vel += accel * deltaTime;
            }
            
            base.update(deltaTime);

            if (itemPopUps.Count > 0) {
                itemPopUps[0].update(deltaTime);
                if (itemPopUps[0].time > ItemPopUp.endEnd) {
                    itemPopUps.RemoveAt(0);
                }
            }
        }

        public override void die() {
            health = maxHealth;
            Wall.deaths++;
        }

        public override float findRotation(float deltaTime) {
            const float maxRot = (float) Math.PI * 0.4F;
            return Math.Sign(vel.X) * Math.Min(1, Math.Abs(vel.X) / 100F) * maxRot;
        }

        public void inventoryMouseInput(MouseInfo mouse, float deltaTime) {
            ItemSlot slotOver = mouseToSlot(mouse.pos);

            if (mouse.leftPressed) {
                if (slotOver != null) {
                    handSlot.trySwap(slotOver);
                }
            }
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
                //Wall.items.Add(new GroundItem(Item.create(ItemType.Arrow, 99), Wall.camera.toWorld(mouse.pos), Vector2.Zero));
                Wall.entities.Add(create(EntityType.SnowWorm, Wall.camera.toWorld(mouse.pos)));
            }

        }

        public void keyInput(MouseState mouseState, KeyInfo keys, float deltaTime) {
            
            Vector2 diff = new Vector2(mouseState.X, mouseState.Y) - Wall.camera.toScreen(pos);

            int inputX = 0;

            if (keys.pressed(Keys.T)) {
                terrariaMode = !terrariaMode;
            }

            if (keys.pressed(Keys.E) && !grappleOut) {
                grappleOut = true;
                Wall.entities.Add(new Grapple(this, pos, Util.polar(150F, Util.angle(diff))));
            }

            if (keys.up(Keys.Space)) {
                canBounce = true;
            }

            if (grounded) {
                canBounce = false;
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
                    if (!collidesAt(pos + Vector2.UnitY * 2) && canBounce) {
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
        }

        public bool jumpPressed(KeyInfo keys) {
            return keys.down(Keys.W) || keys.down(Keys.Space);
        }

        public override void render(Camera camera, SpriteBatch spriteBatch) {
            base.render(camera, spriteBatch);
            
            foreach (var piece in armor) {
                piece.armor?.renderWearing(camera, spriteBatch);
            }
            
            currentItem?.render(camera, spriteBatch);
        }

        public void renderSlot(Item item, Rectangle rect, Camera camera, SpriteBatch spriteBatch, bool isSelected = false, bool background = true, bool renderNum = true) {
            if (background) {
                Texture2D itemSlot = Textures.get("ItemSlot");
                if (isSelected) {
                    spriteBatch.Draw(itemSlot, rect, new Color(Color.White, 0.6F));
                }
                else {
                    spriteBatch.Draw(itemSlot, rect, new Color(Color.White, 0.3F));
                }
            }

            if (item != null) {
                spriteBatch.Draw(item.texture, Util.useRatio(item.dimen, rect), Color.White);

                if (renderNum && item.maxStack != 1) {
                    spriteBatch.DrawString(Fonts.arial, "" + item.count, new Vector2(rect.Left + 40, rect.Top + 40), Color.White);
                }
            }
        }

        public void renderUI(Camera camera, SpriteBatch spriteBatch) {
            
            Texture2D itemSlot = Textures.get("ItemSlot");


            int x = 20, y = 20;
            
            // inventory
            for (int j = 0; j < inventory.GetLength(1); j++) {
                for (int i = 0; i < inventory.GetLength(0); i++) {
                    Rectangle rect = new Rectangle(x, y, 64, 64);

                    renderSlot(inventory[i, j].item, rect, camera, spriteBatch, j == 0 && i == selectedItemIndex);

                    x += 70;
                }

                if (!inventoryOpen) break;

                x = 20;
                y += 70;
            }

            if (inventoryOpen) {
                x = 20;
                y = 400;
                for (int i = 0; i < armor.Length; i++) {
                    Rectangle rect = new Rectangle(x, y, 64, 64);
                    renderSlot(armor[i].item, rect, camera, spriteBatch);
                    y += 70;
                }
            }


            // health bar
            Rectangle healthRect = new Rectangle(1500, 10, 300, 30);
            spriteBatch.Draw(itemSlot, healthRect, new Color(Color.White, 0.4F));
            spriteBatch.Draw(Textures.get("HealthBar"), new Rectangle(healthRect.X, healthRect.Y, (int) (healthRect.Width * (health / maxHealth)), healthRect.Height), 
                new Color(Color.White, 0.4F));
            

            // special charge bar
            if (currentItem != null) {
                healthRect = new Rectangle(1600, 950, 200, 30);
                spriteBatch.Draw(itemSlot, healthRect, new Color(Color.White, 0.4F));
                spriteBatch.Draw(Textures.get("SpecialBar"),
                    new Rectangle(healthRect.X, healthRect.Y, (int) (healthRect.Width * (currentItem.specialChargeAmount())),
                        healthRect.Height),
                    new Color(Color.White, 0.4F));
            }

            if (itemPopUps.Count > 0)
                itemPopUps[0].render(this, camera, spriteBatch);
            
            // item hover text
            if (inventoryOpen) {
                Vector2 mousePos = Wall.lastMouseInfo.pos;
                ItemSlot slot = mouseToSlot(mousePos);

                slot?.item?.renderHoverInfo(mousePos, spriteBatch);
                renderSlot(handSlot.item, Util.center(mousePos, Vector2.One * 64), camera, spriteBatch, false, false);
            }
        }
    }
}