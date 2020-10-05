using System;
using System.Collections.Generic;
using System.Text;

namespace Adventure
{
    class Program
    {
        static void Main(string[] args)
        {
            GameManager gameManager = new GameManager();
        }
    }

 

    public class GameObject
    {
        Random random = new Random();
        public string name;
        public string symbol = "!";
        public int xPosition;
        public int yPosition;
        public GameManager gameManager;

        public void DrawObject()
        {
            //Draw the object on screen. Can we tidy this up? Seems redundant to do the same thing twice.
            Console.SetCursorPosition(xPosition, yPosition);
            Console.Write(symbol);

            Console.SetCursorPosition(xPosition, yPosition);
            Console.Write("");
        }

        public void UndrawObject()
        {
            Console.SetCursorPosition(xPosition, yPosition);
            Console.Write(" ");
            //symbol = " ";
            //DrawObject();
        }

        public void RandomizePosition()
        {
            //This randomizes our position.
            xPosition = random.Next(1, gameManager.gameBoard.boardWalls.GetLength(0) - 1);
            yPosition = random.Next(1, gameManager.gameBoard.boardWalls.GetLength(1) - 1);
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

        protected int[,] Move(int moveX, int moveY)
        {
            //When we move, we need to ensure we are not moving into a wall. We check with the board.
            if (gameManager.gameBoard.isWall(xPosition + moveX, yPosition + moveY) != true)
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
            if (gameManager.SceneObjects.Contains(this))
            {
                gameManager.SceneObjects.Remove(this);
                UndrawObject();
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
                    bag.UseInventory(this);
                    break;
            }

            if (!IsInteracting(xPosition + inputDirection[0], yPosition + inputDirection[1]))
            {
                Move(inputDirection[0], inputDirection[1]);
            }
        }

        bool IsInteracting(int xPos, int yPos)
        {
            for (int i = 0; i < gameManager.SceneObjects.Count; i++)
            {
                if (xPos == gameManager.SceneObjects[i].xPosition && yPos == gameManager.SceneObjects[i].yPosition && gameManager.SceneObjects[i] != this)
                {
                    gameManager.SceneObjects[i].Interact(this);
                    return true;
                }
            }
            return false;
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
            Console.WriteLine($"You hit skeleton for {player.strength}! Skeleton has {health} hp left!");

            Attack(player);
            Console.WriteLine($"Skeleton hit you for {strength}! You have have {player.health} hp left!");

            gameManager.Combat();
    }

    }

    public class GameManager
    {
        //This class handles all logic in the game.

        public Board gameBoard { get; private set; }
        List<GameObject> sceneObjects;
        Player player;

        public List<GameObject> SceneObjects
        {
            get { return sceneObjects; }
            set { sceneObjects = value; }
        }

        public GameManager()
        {
            //First, we create a new board to play on.
            //This will end up defining the edges based on the given size, and then draw it out.
            gameBoard = new Board(80, 30);
            player = new Player();

            SceneObjects = new List<GameObject>
            {
                new Consumable(),
                new Skeleton(),
                player
            };



            //Next, we spawn enemies. Currently, we only spawn one on a fixed location.
            foreach (var GameObject in SceneObjects)
            {
                GameObject.gameManager = this;
                GameObject.RandomizePosition();
                GameObject.DrawObject();
            }

            //This here loops indefinitely. We read player input, then redraw all characters in the scene.
            while (true)
            {
                player.ReadInput();
                foreach (var GameObject in SceneObjects)
                {
                    GameObject.DrawObject();
                }
            }
        }

        public void Combat()
        {
            bool [,] boardWalls = new bool[30, 30];

            for (int i = 0; i < boardWalls.GetLength(1); i++)
            {
                for (int j = 0; j < boardWalls.GetLength(0); j++)
                {
                    if (i == 0 || j == 0 || i == boardWalls.GetLength(1) - 1 || j == boardWalls.GetLength(0) - 1)
                    {
                     
                        boardWalls[j, i] = true;
                    }
                    else
                    {
                        boardWalls[j, i] = false;
                    }
                }
            }

            Border CombatMenu = new Border(boardWalls, 80, 0);

        }

        public void DrawCharacters()
        {
            for (int i = 0; i < sceneObjects.Count; i++)
            {
                sceneObjects[i].DrawObject();
            }
        }
    }

    public class Menu
    {
        //Draw content within bounds.
        //Draw border around bounds

        Border menuBorder;
        int sizeX;
        int sizeY;
        int offsetX;
        int offsetY;

        public Menu(int borderSizeX, int borderSizeY, int borderOffsetX, int borderOffsetY)
        {
            sizeX = borderSizeX;
            sizeY = borderSizeY;
            offsetX = borderSizeX;
            offsetY = borderSizeY;


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

        public List<GameObject> SceneObjects
        {
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
            Console.Clear();
            boardEdges.DrawBorder();
        }

        public void DrawCharacters()
        {
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


        int offsetX = 0;
        int offsetY = 0;
        bool[,] dimensions;

        public Border(bool[,] walls)
        {
            dimensions = walls;
            DrawBorder();
        }

        public Border(bool[,] walls, int newOffsetX, int newOffsetY)
        {
            dimensions = walls;
            offsetX = newOffsetX;
            offsetY = newOffsetY;
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
            Console.SetCursorPosition(offsetX, offsetY);

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
                Console.SetCursorPosition(offsetX, offsetY + i);
            }
        }
    }
    public class Item : GameObject
    {
        protected Player owner;
        public Item()
        {
            symbol = "¤";
            name = "Item";
        }

        public override void Interact(Player player)
        {
            if (gameManager.SceneObjects.Contains(this))
            {
                gameManager.SceneObjects.Remove(this);
                player.bag.addItem(this);
                UndrawObject();
                owner = player;
            }
        }

        public virtual void UseItem() { }
    }
    public class Equipment : Item { }
    public class Consumable : Item
    {
        public Consumable()
        {
            symbol = "!";
        }

        public override void UseItem()
        {
            owner.health += 5;
            owner.bag.Bag.Remove(this);
        }
    }

    public class Inventory
    {
        public List<Item> Bag = new List<Item>();
        public void addItem(Item newItem)
        {
            Bag.Add(newItem);
            //Console.WriteLine($"Added: {newItem.name}");
        }

        public void UseInventory(Player player)
        {
            Console.Clear();

            Console.WriteLine($"Health: {player.health}");
            Console.WriteLine();

            for (int i = 0; i < Bag.Count; i++)
            {
                Console.WriteLine($"{i}: {Bag[i].name}");
            }

            ConsoleKeyInfo input;
            do
            {
                int result;
                input = Console.ReadKey(true);
                if (int.TryParse(input.KeyChar.ToString(), out result))
                {
                    Console.WriteLine(result);
                    for (int i = 0; i < Bag.Count; i++)
                    {
                        //PROBLEMATIQUE
                        //Takes ANY input to mean success.
                        //Fix.
                        if (result == i)
                        {
                            Console.WriteLine($"Using {Bag[i].name}!");
                            Bag[i].UseItem();
                            break;
                        }
                    }
                } else { Console.WriteLine(input); }
            } while (input.Key != ConsoleKey.Escape);

            Console.Clear();
            player.gameManager.gameBoard.DrawScene();
        }

    }
}
