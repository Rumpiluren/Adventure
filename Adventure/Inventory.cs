using System;
using System.Collections.Generic;

namespace Adventure
{
    public class Inventory
    {
        public List<Item> bag = new List<Item>();
        public Item[] equippedItems;
        GameManager gameManager;
        public Menu menu;

        public Inventory(GameManager newManager)
        {
            gameManager = newManager;
            equippedItems = new Item[4];
        }

        public void addItem(Item newItem)
        {
            bag.Add(newItem);
        }

        public void UseInventory(Player player)
        {
            UpdateInventory();

            ConsoleKeyInfo input;
            do
            {
                int result;
                input = Console.ReadKey(true);
                if (int.TryParse(input.KeyChar.ToString(), out result))
                {
                    for (int i = 0; i < bag.Count; i++)
                    {
                        if (result == i)
                        {
                            bag[i].UseItem();
                            UpdateInventory();
                        }
                    }
                }
                else { break; }
            } while (input.Key != ConsoleKey.Escape);

            menu.Clear();
            player.gameManager.infoBox.Redraw();
        }

        void UpdateInventory()
        {
            menu = new Menu(gameManager.infoBox.sizeX, gameManager.infoBox.sizeY, gameManager.infoBox.offsetX, gameManager.infoBox.offsetY, "INVENTORY");

            foreach (var item in equippedItems)
            {
                if (item != null)
                {
                    menu.Write(item.stats.Name);
                }
            }
            menu.Write("\n EQUIPMENT:");

            if (bag.Count > 0)
            {
                for (int i = 0; i < bag.Count; i++)
                {
                    menu.Write($"{i}: {bag[i].stats.Name}");
                }
                menu.Write("ITEMS:");
            }
            else
            {
                menu.Write("Your bag is empty!");
            }
        }
    }
}
