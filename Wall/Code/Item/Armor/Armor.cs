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

        public Armor(int count) : base(count) { }
        
        public new static Armor create(ItemType itemType, int count) {
            return (Armor) Item.create(itemType, count);
        }
        
        public new static Armor create(ItemType itemType) {
            return (Armor) Item.create(itemType);
        }

        public virtual Texture2D wearingTexture() {
            return texture;
        }

        public virtual void renderWearing(Camera camera, SpriteBatch spriteBatch) {
            Vector2 pos = player.pos + Util.rotate(wearOffset, player.rotation);
            
            Util.render(wearingTexture(), pos, dimen, player.rotation, camera, spriteBatch, player.facingLeft);
        }
    }
}