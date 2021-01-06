using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Wall {
    public class Armor : Item {

        public float defense;
        public armorType armorSlot;
        
        public Vector2 wearOffset;

        public enum armorType {
            helmet, chest, boots
        }

        public Armor() : base(1) { }

        public virtual Texture2D wearingTexture() {
            return texture;
        }

        public virtual void renderWearing(Camera camera, SpriteBatch spriteBatch) {
            Vector2 pos = player.pos + Util.rotate(wearOffset, player.rotation);
            
            Util.render(wearingTexture(), pos, dimen, player.rotation, camera, spriteBatch, player.facingLeft);
        }
    }
}