namespace Adventure
{
    public abstract class Item : GameObject
    {
        protected Player owner;

        public override void Interact(Player player)
        {
            //Remove item from scene, add to player inventory, update the InfoBox
            gameManager.SceneObjects.Remove(this);
            player.playerInventory.addItem(this);
            UndrawObject();
            owner = player;

            gameManager.infoBox.Write($"Picked up {stats.Name}!");
        }

        public virtual void UseItem()
        {
            //Update Status box.
            gameManager.statusBox.Clear();
            gameManager.statusBox.Wipe();
            gameManager.statusBox.Write($"HP: {owner.stats.Health}\n Strength: {owner.stats.Strength}\n Accuracy: {owner.stats.Accuracy}");
        }

        public virtual void DiscardItem()
        {
            //Update Status box and info box 
            gameManager.statusBox.Clear();
            gameManager.statusBox.Wipe();
            owner.gameManager.infoBox.Write($"{owner.stats.Name} discarded {stats.Name}!");
            gameManager.statusBox.Write($"HP: {owner.stats.Health}\n Strength: {owner.stats.Strength}\n Accuracy: {owner.stats.Accuracy}");
        }
    }
}
