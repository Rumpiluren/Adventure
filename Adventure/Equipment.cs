namespace Adventure
{
    public abstract class Equipment : Item
    {
        public EquipmentSlots equipmentSlot;

        public override void UseItem()
        {
            owner.playerInventory.bag.Remove(this);

            //Check if player is already wearing equipment in this slot.
            if (owner.playerInventory.equippedItems[(int)equipmentSlot] != null)
            {
                owner.playerInventory.equippedItems[(int)equipmentSlot].DiscardItem();
            }

            //Add this item to the list of equipped equipment
            owner.gameManager.infoBox.Write($"{owner.stats.Name} equipped {stats.Name}!");
            owner.stats.Add(stats);
            owner.playerInventory.equippedItems[(int)equipmentSlot] = this;
            base.UseItem();
        }

        public override void DiscardItem()
        {
            //Remove item from player
            owner.stats.Subtract(stats);
            owner.playerInventory.equippedItems[(int)equipmentSlot] = null;
            base.DiscardItem();
        }
    }
    /// <summary>
    /// All types of equipment below!
    /// </summary>
    public class Helmet : Equipment
    {
        public Helmet()
        {
            equipmentSlot = EquipmentSlots.Head;
            stats = new Stats("Iron Helmet", "Q", 5, 0, 0);
        }
    }
    public class Chestplate : Equipment
    {
        public Chestplate()
        {
            equipmentSlot = EquipmentSlots.Torso;
            stats = new Stats("Iron Chestplate", "Q", 5, 0, 0);
        }
    }
    public class Leggings : Equipment
    {
        public Leggings()
        {
            equipmentSlot = EquipmentSlots.Lower;
            stats = new Stats("Iron Legplates", "Q", 5, 0, 0);
        }
    }
    public class Sword_Iron : Equipment
    {
        public Sword_Iron()
        {
            equipmentSlot = EquipmentSlots.Weapon;
            stats = new Stats("Iron Sword", "W", 0, 10, 0);
        }
    }
    public class Sword_Steel : Equipment
    {
        public Sword_Steel()
        {
            equipmentSlot = EquipmentSlots.Weapon;
            stats = new Stats("Steel Sword", "W", 0, 20, 0);
        }
    }
}
