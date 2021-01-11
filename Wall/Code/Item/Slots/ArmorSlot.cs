namespace Wall {
    public class ArmorSlot : ItemSlot {

        public ArmorSlot(Armor.armorType armorType) {
            this.armorType = armorType;
        }

        public Armor armor => (Armor) item;
        public Armor.armorType armorType;

        public override bool canHold(Item item) {
            return item == null || item.GetType().IsSubclassOf(typeof(Armor)) && ((Armor)item).armorSlot == armorType;
        }
    }
}