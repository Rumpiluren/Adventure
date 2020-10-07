using System;

namespace Adventure
{
    public abstract class GameObject
    {
        protected Random random = new Random();
        public GameManager gameManager;

        public Coordinates coordinates;
        public Stats stats;

        public void DrawObject()
        {
            //Draw the object on screen. Can we tidy this up? Seems redundant to do the same thing twice.
            Console.SetCursorPosition(coordinates.x, coordinates.y);
            Console.Write(stats.Symbol);

            Console.SetCursorPosition(coordinates.x, coordinates.y);
            Console.Write("");
        }

        public void UndrawObject()
        {
            Console.SetCursorPosition(coordinates.x, coordinates.y);
            Console.Write(" ");
        }

        public void RandomizePosition()
        {
            //This randomizes our position.
            coordinates.x = random.Next(1, gameManager.gameBoard.boardWalls.GetLength(0) - 1);
            coordinates.y = random.Next(1, gameManager.gameBoard.boardWalls.GetLength(1) - 1);
        }

        public virtual void Interact(Player player) { }
    }

}
