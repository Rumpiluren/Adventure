namespace Adventure
{
    public abstract class Consumable : Item
    {
        public override void UseItem()
        {
            owner.gameManager.infoBox.Write($"{owner.stats.Name} used {stats.Name}!");
            owner.stats.Add(stats);
            owner.playerInventory.bag.Remove(this);
            base.UseItem();
        }
    }

/// <summary>
/// All types of consumables below!
/// </summary>

    public class HealthPotion : Consumable
    {
        public HealthPotion()
        {
            stats = new Stats("Health Potion", "%", 10, 0, 0);
        }
    }
    public class SmallHealthPotion : Consumable
    {
        public SmallHealthPotion()
        {
            stats = new Stats("Minor Health Potion", "%", 5, 0, 0);
        }
    }
    public class LargeHealthPotion : Consumable
    {
        public LargeHealthPotion()
        {
            stats = new Stats("Major Health Potion", "%", 15, 0, 0);
        }
    }
    public class StrengthPotion : Consumable
    {
        public StrengthPotion()
        {
            stats = new Stats("Bottle of Beer", "%", 0, 5, 0);
        }
    }
    public class AccuracyPotion : Consumable
    {
        public AccuracyPotion()
        {
            stats = new Stats("Potion of Speed", "%", 0, 0, 5);
        }
    }
}
