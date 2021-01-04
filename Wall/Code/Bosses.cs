using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Wall {
    public static class Bosses {

        public static Texture2D back, front;
        
        static Bosses() {
            back = Textures.get("ItemSlot");
            front = Textures.get("HealthBar");
        }

        public static void renderHealthBar(Entity boss, float y, SpriteBatch spriteBatch) {
            
            Rectangle healthRect = Util.center(new Vector2(1920 / 2, y), new Vector2(1820, 30));
            spriteBatch.Draw(back, healthRect, new Color(Color.White, 0.4F));
            spriteBatch.Draw(front, new Rectangle(healthRect.X, healthRect.Y, (int) (healthRect.Width * Math.Max(boss.health / boss.maxHealth, 0)), healthRect.Height), 
                new Color(Color.White, 0.4F));

            Vector2 textPos = Util.toVec(healthRect.Location);
            spriteBatch.DrawString(Fonts.arial, ("" + boss.GetType().Name + ": " + boss.health + " / " + boss.maxHealth), textPos, Color.White);
        }

    }
}