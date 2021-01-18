using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Wall {
    public class ItemPopUp {

        public static Texture2D back;

        public Item item;
        
        public Vector2 pos;
        public float time;

        public float addTime;

        public const float startEnd = 0.4F, endStart = 1.2F, endEnd = 1.6F;
        
        public static Vector2 dimen = new Vector2(200, 150);

        public ItemPopUp(Item item) {
            this.item = item;
        }

        static ItemPopUp() {
            back = Textures.get("ItemSlot");
        }

        public bool shouldAdd(Item item) {
            return time < endStart && item.itemType == this.item.itemType;
        }

        public void addedPick(Item item) {
            if (time > 0) {
                time = Math.Min(time, startEnd);
                addTime = 0.5F;
            }

            this.item.count += item.count;
        }

        public void update(float deltaTime) {
            time += deltaTime;
            addTime -= deltaTime;

            pos = new Vector2(1620, 770);
            float diff = 1920 - pos.X;

            if (time < startEnd) {
                pos += (1 - Util.sinSmooth(time, startEnd)) * Vector2.UnitX * diff;
            }

            if (time > endStart) {
                pos += Util.sinSmooth(time - endStart, endEnd - endStart) * Vector2.UnitX * diff;
            }
        }

        public void render(Player player, Camera camera, SpriteBatch spriteBatch) {
            spriteBatch.Draw(back, Util.tl(pos, dimen), new Color(Color.White, 0.5F));
            Vector2 diff = Vector2.One * 20;
            player.renderSlot(item, Util.tl(pos + diff, dimen - diff * 2), camera, spriteBatch, false, false, false);

            float colVal = Math.Clamp(addTime / 0.5F, 0, 1);
            
            spriteBatch.DrawString(Fonts.arial, "x" + item.count, pos, new Color(1F, 1F - colVal / 2F, 1F - colVal));
        }
    }
}