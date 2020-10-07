using System;

namespace Adventure
{
    public class Player : Character
    {
        public Inventory playerInventory;

        public Player(GameManager newManager)
        {
            stats = new Stats("Player", "@", 100, 10, 75);
            gameManager = newManager;
            playerInventory = new Inventory(gameManager);
        }

        public void ReadInput()
        {
            //This here is where we end up after each button press.
            //We read the user input,
            //and if the input equals any of the arrow keys, we store the movement.
            var input = Console.ReadKey(false).Key;
            int[] inputDirection = new int[2];
            switch (input)
            {
                case ConsoleKey.UpArrow:
                    inputDirection[1] = -1;
                    break;
                case ConsoleKey.DownArrow:
                    inputDirection[1] = 1;
                    break;
                case ConsoleKey.LeftArrow:
                    inputDirection[0] = -1;
                    break;
                case ConsoleKey.RightArrow:
                    inputDirection[0] = 1;
                    break;
                case ConsoleKey.Escape:
                    playerInventory.UseInventory(this);
                    break;
            }

            //Check if we would land on another Game Object. If there is nothing there, we use the saved value to move.
            if (!IsInteracting(coordinates.x + inputDirection[0], coordinates.y + inputDirection[1]))
            {
                Move(inputDirection[0], inputDirection[1]);
            }

            //Wipe the status screen. Why do we do this?
            gameManager.statusBox.Clear();
            gameManager.statusBox.Wipe();
            gameManager.statusBox.Write($"HP: {stats.Health}\n Strength: {stats.Strength}\n Accuracy: {stats.Accuracy}");
        }

        bool IsInteracting(int xPos, int yPos)
        {
            //Recieves a position in XY, checks to see if there is a GameObject there. If so, call the Interact function. If not, return false.
            for (int i = 0; i < gameManager.SceneObjects.Count; i++)
            {
                if (xPos == gameManager.SceneObjects[i].coordinates.x && yPos == gameManager.SceneObjects[i].coordinates.y && gameManager.SceneObjects[i] != this)
                {
                    gameManager.SceneObjects[i].Interact(this);
                    return true;
                }
            }
            return false;
        }

        public override void Death()
        {
            base.Death();
            Environment.Exit(0);
        }
    }

}
