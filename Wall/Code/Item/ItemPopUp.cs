using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Wall {
    public class ItemPopUp {

        public static Texture2D back;

        public Item item;
        
        public Vector2 pos;
        public float time;

        public const float startEnd = 0.4F, endStart = 1.2F, endEnd = 1.6F;
        
        public static Vector2 dimen = new Vector2(200, 150);

        public ItemPopUp(Item item) {
            this.item = item;
        }

        static ItemPopUp() {
            back = Textures.get("ItemSlot");
        }

        public void update(float deltaTime) {
            time += deltaTime;

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
            player.renderSlot(item, Util.tl(pos + diff, dimen - diff * 2), camera, spriteBatch, false, false);
        }
    }
}