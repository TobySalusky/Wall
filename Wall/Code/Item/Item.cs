using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Wall {
    public class Item {

        public type itemType;
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

        public enum type {
            FrostSword, Bow, Shuriken, SnowBall, Arrow, RubberArrow
        }

        public Item(int count) {
            itemType = Enum.Parse<type>(GetType().Name);
            this.count = count;

            texture = Textures.get(GetType().Name);
            dimen = new Vector2(texture.Width, texture.Height) * Tile.pixelSize;
        }

        public virtual void switchOff() {
            useCancelled = true;
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

        public bool isUsing() {
            return useTimer > 0 && !useCancelled;
        }

        public virtual void update(float deltaTime, MouseInfo mouse) {
            useTimer -= deltaTime;

            if (useTimer <= 0) {
                useCancelled = false;
            }

            if (((autoSwing && mouse.leftDown) || mouse.leftPressed) && canUse()) {
                Vector2 diff = mouse.pos - Wall.camera.toScreen(player.pos);
                use(Util.angle(diff), Util.mag(diff));
            }
        }

        public void makeStackable(int stackSize = baseStackSize) {
            maxStack = stackSize;
        }

        public void render(Camera camera, SpriteBatch spriteBatch) {
            if (allwaysRender || isUsing()) {
                renderInHand(camera, spriteBatch);
            }
        }
        
        public virtual void renderInHand(Camera camera, SpriteBatch spriteBatch) {
        }
    }
}