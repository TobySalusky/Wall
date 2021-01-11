using Microsoft.Xna.Framework.Graphics;

namespace Wall {
    public class ItemSlot {
        public Item item;

        public void trySwap(ItemSlot slot) {
            if (canHold(slot.item) && slot.canHold(item)) {
                Item temp = item;
                item = slot.item;
                slot.item = temp;
            }
        }

        public virtual bool canHold(Item item) {
            return true;
        }
    }
}