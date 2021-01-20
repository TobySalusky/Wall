using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Wall {
    public class Item {

        public ItemType itemType;
        public int count;

        public int maxStack = 1;
        public const int baseStackSize = 999;
        public bool consumable = false;

        public float useDelay = 1F;
        public float useTimer;
        public bool useCancelled;

        public bool autoSwing = true, allwaysRender = false;

        public Texture2D texture;

        public static Player player;
        public Vector2 dimen;

        public float offset, angle;

        public float specialTimeCharging, maxSpecialChargeTime = 5;
        public bool specialUse;

        public string name;

        public bool angleFollow = true, renderWhenReady = false;

        public Vector2 handOffset;
        public bool flipHandOffsetY = true;

        private static Dictionary<ItemType, Type> typeDict;

        public Item(int count) {
            itemType = Enum.Parse<ItemType>(GetType().Name);
            this.count = count;
            
            texture = Textures.get(GetType().Name);
            dimen = new Vector2(texture.Width, texture.Height) * Tile.pixelSize;
            
            name = Util.spacedName(GetType().Name);
        }

        public static Item create(ItemType itemType, int count) {
            var construct = typeDict[itemType].GetConstructor(new [] {typeof(int)});

            return (Item) construct.Invoke(new object[] {count});
        }
        
        public static Item create(ItemType itemType) {
            var construct = typeDict[itemType].GetConstructor(new [] {typeof(int)});

            return (Item) construct.Invoke(new object[] {1});
        }

        public string genHoverInfo() {
            return name;
        }

        public void renderHoverInfo(Vector2 mousePos, SpriteBatch spriteBatch) {
            
            mousePos += Vector2.One * 20;
            
            Texture2D itemSlot = Textures.get("ItemSlot");
            Vector2 infoDimen = new Vector2(200, 200);
            Vector2 center = mousePos + infoDimen / 2;
            
            spriteBatch.Draw(itemSlot, Util.center(center, infoDimen), new Color(Color.White, 0.4F));
            spriteBatch.DrawString(Fonts.arial, genHoverInfo(), mousePos, Color.White);
        }

        public static Vector2 mouseDiff(MouseInfo mouse) {
            return mouse.pos - Wall.camera.toScreen(player.pos);
        }

        public static float mousePlayerAngle(MouseInfo mouse) {
            return Util.angle(mouseDiff(mouse));
        }

        public static void loadItems() {
            
            typeDict = new Dictionary<ItemType, Type>();
            
            var types = Util.subClassesOf(typeof(Item));

            foreach (Type itemClass in types) {
                try {
                    ItemType itemType = Enum.Parse<ItemType>(itemClass.Name);

                    typeDict[itemType] = itemClass;
                }
                catch {
                    // ignore items without enum-counterparts
                }
            }
        }

        public virtual void switchOff() {
            useCancelled = true;
            specialTimeCharging = 0;
        }

        public virtual bool canUse() {
            return useTimer <= 0;
        }

        public virtual void use(float angle, float distance) {
            useTimer = useDelay;

            if (consumable) {
                count--;
            }
        }

        public virtual bool isUsing() {
            return useTimer > 0 && !useCancelled;
        }

        public float usedAmount() { // turns time though delay into float of 0-1
            return (useDelay - useTimer) / useDelay;
        }

        public bool holdingSpecial() {
            return specialTimeCharging > 0;
        }

        public virtual void holdSpecial(float deltaTime, MouseInfo mouse) {
            specialTimeCharging += deltaTime;
        }

        public float specialChargeAmount() {
            return Math.Min(specialTimeCharging / maxSpecialChargeTime, 1F);
        }

        public virtual void useSpecial(float angle, float mag) {
            
        }

        public virtual void update(float deltaTime, MouseInfo mouse) {
            
            Vector2 diff = mouse.pos - Wall.camera.toScreen(player.pos);
            if (angleFollow)
                angle = Util.angle(diff);
            
            useTimer -= deltaTime;

            if (mouse.rightUnpressed) {
                useSpecial(Util.angle(diff), Util.mag(diff));
            }

            if (useTimer <= 0) {
                useCancelled = false;
                if (!mouse.rightDown || specialUse) {
                    specialTimeCharging = 0;
                    specialUse = false;
                }
            }

            if (mouse.rightDown && !mouse.leftDown && canUse() && !specialUse) {
                holdSpecial(deltaTime, mouse);
            }

            if (((autoSwing && mouse.leftDown) || mouse.leftPressed) && canUse()) {
                use(Util.angle(diff), Util.mag(diff));
            }
        }

        public void makeStackable(int stackSize = baseStackSize) {
            maxStack = stackSize;
        }

        public virtual bool renderInHand() {
            return allwaysRender || (!renderWhenReady && (isUsing() || holdingSpecial())) || (renderWhenReady && canUse());
        }

        public void render(Camera camera, SpriteBatch spriteBatch) {
            if (renderInHand()) {
                renderInHand(camera, spriteBatch);
            }
        }
        
        public virtual void renderInHand(Camera camera, SpriteBatch spriteBatch) {
            Vector2 pos = player.pos + Util.polar((offset), angle);

            bool facingLeft = Util.angleDir(angle);
            float renderAngle = facingLeft ? angle + Maths.PI : angle;

            Util.render(texture, pos, dimen, renderAngle, camera, spriteBatch, !Util.angleDir(angle));
        }

        public virtual void animatePlayer(float deltaTime) {
            if (renderInHand()) {
                Vector2 hand = handOffset;
                if (flipHandOffsetY && Util.angleDir(angle)) {
                    hand.Y *= -1;
                }

                player.topHand = player.pos + Util.polar(offset, angle) + Util.rotate(hand, angle);
            }
        }
    }
}