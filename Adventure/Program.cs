using System;
using System.Collections.Generic;
using System.Text;

namespace Adventure
{
    class Program
    {
        static void Main(string[] args)
        {
            //We should try to move as much as possible from the Main function if we can.

            //First, we create a new board to play on.
            //This will end up defining the edges based on the given size, and then draw it out.
            Board board = new Board(80, 30);
            Player player = new Player();

            board.SceneObjects = new List<GameObject>();
            board.SceneObjects.Add(player);
            board.SceneObjects.Add(new Item());
            board.SceneObjects.Add(new Skeleton());

            //Next, we spawn the player and draw them on the screen.

            //Next, we spawn enemies. Currently, we only spawn one on a fixed location.
            foreach (var GameObject in board.SceneObjects)
            {
                GameObject.board = board;
                GameObject.RandomizePosition();
                GameObject.DrawObject();
            }

            //This here loops indefinitely. We read player input, then redraw all characters in the scene.
            while (true)
            {
                player.ReadInput();
                foreach (var GameObject in board.SceneObjects)
                {
                    GameObject.DrawObject();
                }
                player.DrawObject();
            }
        }
    }

    public class GameObject
    {
        Random random = new Random();
        public string name;
        public string symbol = "!";
        public int xPosition;
        public int yPosition;
        public Board board;

        public void DrawObject()
        {
            Console.SetCursorPosition(xPosition, yPosition);
            Console.Write(symbol);

            Console.SetCursorPosition(xPosition, yPosition);
            Console.Write("");
        }

        public void RandomizePosition()
        {
            //This randomizes our position.
            xPosition = random.Next(1, board.boardWalls.GetLength(0) - 1);
            yPosition = random.Next(1, board.boardWalls.GetLength(1) - 1);
        }

        public virtual void Interact(Player player) { }
    }

    public class Character : GameObject
    {
        //Base class for each creature in the game, including the player.
        Random random = new Random();

        public int strength = 0;
        public int health = 0;
        public int accuracy = 0;
        //public new string symbol = "%";

        public Character()
        {
            symbol = "%";
        }

        protected int[,] Move(int moveY, int moveX)
        {
            //When we move, we need to ensure we are not moving into a wall. We check with the board.
            if (board.isWall(xPosition + moveX, yPosition + moveY) != true)
            {
                xPosition += moveX;
                yPosition += moveY;
            }
            return new int[yPosition, xPosition];
        }

        public void Attack(Character opponent)
        {
            if (random.Next(1, 100) <= accuracy)
            {
                opponent.health -= strength;
            }
            if (opponent.health <= 0)
            {
                opponent.Death();
            }
        }

        public void Death()
        {
            if (board.SceneObjects.Contains(this))
            {
                board.SceneObjects.Remove(this);
            }
        }

    }

    public class Player : Character
    {
        public Inventory bag = new Inventory();

        public Player()
        {
            symbol = "@";
            health = 10;
            strength = 1;
            accuracy = 75;
        }

        public void ReadInput()
        {
            //This here is where we end up after each button press.
            //We read the user input,
            //and if the input equals any of the arrow keys, we call the 'Move' function from the parent class.
            var input = Console.ReadKey(false).Key;

            if (IsInteracting(xPosition, yPosition))
            {

            }
            else
            {
                switch (input)
                {
                    case ConsoleKey.UpArrow:
                        Move(-1, 0);
                        break;
                    case ConsoleKey.DownArrow:
                        Move(1, 0);
                        break;
                    case ConsoleKey.LeftArrow:
                        Move(0, -1);
                        break;
                    case ConsoleKey.RightArrow:
                        Move(0, 1);
                        break;
                    case ConsoleKey.Escape:
                        OpenInventory();
                        break;
                }
            }
        }

        bool IsInteracting(int xPos, int yPos)
        {
            for (int i = 0; i < board.SceneObjects.Count; i++)
            {
                if (xPos == board.SceneObjects[i].xPosition && yPos == board.SceneObjects[i].yPosition && board.SceneObjects[i] != this)
                {
                    board.SceneObjects[i].Interact(this);
                    return true;
                }
            }
            return false;
        }

        void OpenInventory()
        {
            Console.Clear();

            Console.WriteLine($"Health: {health}");

            foreach (var Item in bag.Bag)
            {
                Console.WriteLine(Item.name);
            }

            do
            {
            } while (Console.ReadKey(false).Key != ConsoleKey.Escape);

            Console.Clear();
            board.DrawScene();
        }

    }

    class Skeleton : Character
    {
        public Skeleton()
        {
            symbol = "#";
            health = 10;
            strength = 1;
            accuracy = 25;
        }

        public override void Interact(Player player)
        {
            player.Attack(this);
            Attack(player);
        }

    }

    public class Board
    {
        //This class is in charge of knowing the layout of the level and the location of all the walls.
        //It owns an instance of the Border class, which in turn is in charge of drawing the walls visually.
        public bool[,] boardWalls;
        Border boardEdges;
        Random rnd = new Random();
        List<GameObject> sceneObjects;

        public List<GameObject> SceneObjects {
            get { return sceneObjects; }
            set { sceneObjects = value; }
        }

        public Board(int x, int y)
        {
            //Constructor script.
            //The 'boardWalls' array below is an array of bools with the same dimensions as the area size.
            //The bool value indicates whether or not the location is a wall.
            boardWalls = new bool[x, y];

            for (int i = 0; i < boardWalls.GetLength(1); i++)
            {
                for (int j = 0; j < boardWalls.GetLength(0); j++)
                {
                    if (i == 0 || j == 0 || i == boardWalls.GetLength(1) - 1 || j == boardWalls.GetLength(0) - 1)
                    {
                        //This loop ends up here if we are at the edges of the map.
                        //Every position out here is a wall, so we set the value at this position in the array to true.
                        boardWalls[j, i] = true;
                    }
                    else
                    {
                        //If we end up here, this is not a wall.
                        boardWalls[j, i] = false;
                    }
                }
            }

            //Now that we have finished setting up the boardWalls position, we instantiate a new Border class with these values.
            //Doing this will draw out the walls on the screen.
            boardEdges = new Border(boardWalls);
        }

        public bool isWall(int xPos, int yPos)
        {
            //This is used by any moving object to see if they can move.
            //It is called by the Character class.
            return boardWalls[xPos, yPos];
        }

        void RandomizeLayout()
        {
            //Unfinished
            bool[,] room = boardWalls;

            for (int i = 0; i < boardWalls.GetLength(0); i++)
            {
                for (int j = 0; j < boardWalls.GetLength(1); j++)
                {
                    boardWalls[i, j] = rnd.Next(2) == 0;
                }
            }
        }

        public void DrawScene()
        {
            Console.SetCursorPosition(0, 0);
            boardEdges.DrawBorder();

            for (int i = 0; i < sceneObjects.Count; i++)
            {
                sceneObjects[i].DrawObject();
            }
        }

    }

    public class Border
    {
        //This is a catch-all class that can be used as long as we need a border.
        //I imagine this can be used for the board, but also for any menu system we want to use (like inventory)

        bool[,] dimensions;

        public Border(bool[,] walls)
        {
            dimensions = walls;
            DrawBorder();
        }

        string CheckNeighbours(int positionX, int positionY)
        {
            //This function check tiles surrounding the wall, and dictates what kind of wall this is.
            //This is only done so we can figure out what graphic to draw to represent the wall.
            //The function takes in two integers for our X and Y position. This is compared to other positions in the dimensions array.

            int surroundingWalls = 0;

            //We set these values independently equal to the bool on that position in the array.
            //We also check if we are on the edge of the array to avoid OutofBounds-errors.
            bool above = (positionY <= 0) ? false : dimensions[positionX, positionY - 1];    //Up
            bool below = (positionY >= dimensions.GetLength(1) - 1) ? false : dimensions[positionX, positionY + 1];    //Down
            bool left = (positionX <= 0) ? false : dimensions[positionX - 1, positionY];    //Left
            bool right = (positionX >= dimensions.GetLength(0) - 1) ? false : dimensions[positionX + 1, positionY];    //Right

            //This here might not be the most elegant solution.
            //I let each bool represent a digit in a base 2 binary number (1s and 0s only, true or false respectively)
            //Each bool adds their value to the surroundingWalls integer declared above, and we get unique results for each situation.
            if (above)
                surroundingWalls += 8;
            if (below)
                surroundingWalls += 4;
            if (left)
                surroundingWalls += 2;
            if (right)
                surroundingWalls += 1;

            //By adding the numbers above together,
            //we can figure out what type of wall this is and convert it to a symbol.
            switch (surroundingWalls)
            {
                case 0:
                    return "█";
                case 1:
                case 2:
                case 3:
                    //return "horizontal";
                    return "═";
                case 4:
                case 8:
                case 12:
                    //return "vertical";
                    return "║";
                case 5:
                    //return "upper left";
                    return "╔";
                case 6:
                    //return "upper right";
                    return "╗";
                case 9:
                    //return "lower left";
                    return "╚";
                case 10:
                    //return "lower right";
                    return "╝";
                case 13:
                    //return "middle left";
                    return "╠";
                case 14:
                    //return "middle right";
                    return "╣";
                case 7:
                    //return "middle top";
                    return "╦";
                case 11:
                    //return "middle bottom";
                    return "╩";
                case 15:
                    //return "middle";
                    return "╬";
                default:
                    return "???"; //This should never happen.
            }
        }

        public void DrawBorder()
        {
            for (int i = 0; i < dimensions.GetLength(1); i++)
            {
                for (int j = 0; j < dimensions.GetLength(0); j++)
                {
                    //Nestled for loop in here to define the borders.
                    if (dimensions[j, i])
                    {
                        //The line below does all the magic.
                        //CheckNeighbours returns a symbol, which is sent directly into Console.Write.
                        Console.Write(CheckNeighbours(j, i));
                    }
                    else
                    {
                        //There is no wall here, so we just draw an empty space.
                        Console.Write(" ");
                    }
                }
                //New line
                Console.WriteLine();
            }
        }
    }
    public class Item : GameObject
    {
        public Item()
        {
            symbol = "¤";
            name = "Item";
        }

        public override void Interact(Player player)
        {
            if (board.SceneObjects.Contains(this))
            {
                board.SceneObjects.Remove(this);
                player.bag.addItem(this);
            }
        }
    }
    public class Equipment : Item { }
    public class Consumable : Item { }

    public class Inventory
    {
        public List<Item> Bag = new List<Item>();
        public void addItem(Item newItem)
        {
            Bag.Add(newItem);
            Console.WriteLine($"Added: {newItem.name}");
        }
    }
}
