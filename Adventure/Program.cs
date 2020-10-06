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



    public abstract class GameObject
    {
        Random random = new Random();
        public string name;
        public string symbol = "!";
        public int xPosition;
        public int yPosition;
        public GameManager gameManager;

        public Stats stats;

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

    public abstract class Character : GameObject
    {
        //Base class for each creature in the game, including the player.
        Random random = new Random();
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
            if (random.Next(1, 100) <= stats.Accuracy)
            {
                opponent.stats.Add(new Stats(-stats.Strength, 0, 0));

                gameManager.infoBox.Write($"{name} hit {opponent.name} for {stats.Strength} damage!");
            }
            if (opponent.stats.Health <= 0)
            {
                opponent.Death();
            }
        }

        public virtual void Death()
        {
            if (gameManager.SceneObjects.Contains(this))
            {
                gameManager.SceneObjects.Remove(this);
                UndrawObject();
                gameManager.infoBox.Write($"{name} died!");
            }
        }

    }

    public class Player : Character
    {
        public Inventory playerInventory;

        public Player(GameManager newManager)
        {
            name = "Player";
            symbol = "@";
            stats = new Stats(100, 10, 75);
            gameManager = newManager;
            playerInventory = new Inventory(gameManager);
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
                    playerInventory.UseInventory(this);
                    break;
            }

            if (!IsInteracting(xPosition + inputDirection[0], yPosition + inputDirection[1]))
            {
                Move(inputDirection[0], inputDirection[1]);
            }

            gameManager.statusBox.Clear();
            gameManager.statusBox.Wipe();
            gameManager.statusBox.Write($"HP: {stats.Health}\n Strength: {stats.Strength}\n Accuracy: {stats.Accuracy}");
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

        public override void Death()
        {
            base.Death();
            Environment.Exit(0);
        }
    }

    class Enemy : Character
    {
        public Enemy()
        {
            name = "Skeleton";
            symbol = "#";
            stats = new Stats(100, 15, 100);
        }

        public override void Interact(Player player)
        {
            player.Attack(this);

            if (stats.Health > 0) { Attack(player); }

        }

    }

    public class GameManager
    {
        //This class handles all logic in the game.

        public Board gameBoard { get; private set; }
        public Menu infoBox;
        public Menu statusBox;
        List<GameObject> sceneObjects;
        Player player;

        public List<GameObject> SceneObjects {
            get { return sceneObjects; }
            set { sceneObjects = value; }
        }

        public GameManager()
        {
            //First, we create a new board to play on.
            //This will end up defining the edges based on the given size, and then draw it out.
            Console.CursorVisible = false;
            gameBoard = new Board(80, 30);
            infoBox = new Menu(30, 20, 80, 0, "HISTORY", true);
            statusBox = new Menu(30, 10, 80, infoBox.sizeY, "STATUS");
            player = new Player(this);

            SceneObjects = new List<GameObject>
            {
                new Consumable(),
                new Consumable(),
                new Consumable(),
                new Enemy(),
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

        List<char> oldMessages;
        string label;
        bool spaceLines;

        Border menuBorder;
        public int sizeX { get; internal set; }
        public int sizeY { get; internal set; }
        public int offsetX { get; internal set; }
        public int offsetY { get; internal set; }

        public Menu(int borderSizeX, int borderSizeY, int borderOffsetX, int borderOffsetY, string windowLabel = "", bool spaceOutLines = false)
        {
            spaceLines = spaceOutLines;
            label = windowLabel;
            sizeX = borderSizeX;
            sizeY = borderSizeY;
            offsetX = borderOffsetX;
            offsetY = borderOffsetY;
            oldMessages = new List<char>();

            menuBorder = new Border(sizeX, sizeY, offsetX, offsetY, label);
        }

        public void Write(string text = "", bool wipe = false)
        {
            if (wipe) { Wipe(); }

            if (text.Length > 0)
            {
                char[] newText = text.ToCharArray();

                oldMessages.Insert(0, ' ');
                oldMessages.Insert(0, '\n');
                oldMessages.InsertRange(0, text);
            }

            text = String.Join("", oldMessages.ToArray());
            Clear();

            string[] splitString = text.Split(new char[] { ' ' });

            Console.SetCursorPosition(offsetX + 1, offsetY + 1);

            for (int i = 0; i < splitString.Length; i++)
            {
                if (Console.CursorTop >= offsetY + sizeY - 2)
                {
                    break;
                }

                if (Console.CursorLeft + splitString[i].Length >= offsetX + sizeX)
                {
                    Console.SetCursorPosition(offsetX + 1, Console.CursorTop + 1);
                }

                if (Console.CursorLeft < offsetX + 1)
                {
                    Console.SetCursorPosition(offsetX + 1, Console.CursorTop + ((spaceLines) ? 1 : 0));
                }

                if (splitString[i].Contains('\n'))
                {
                    Console.Write(splitString[i]);
                }
                else
                {
                    Console.Write(splitString[i] + " ");
                }
            }

        }

        public void Redraw()
        {
            menuBorder.DrawBorder();
            Write();
        }

        public void Wipe()
        {
            oldMessages = new List<char>();
        }

        public void Clear()
        {
            for (int i = 0; i < sizeY - 2; i++)
            {
                Console.SetCursorPosition(offsetX + 1, offsetY + 1 + i);
                for (int j = 0; j < sizeX - 2; j++)
                {
                    Console.Write(" ");
                }
            }
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
            //RandomizeLayout(1);

            //Now that we have finished setting up the boardWalls position, we instantiate a new Border class with these values.
            //Doing this will draw out the walls on the screen.
            boardEdges = new Border(boardWalls, "DUNGEON");
        }

        public bool isWall(int xPos, int yPos)
        {
            //This is used by any moving object to see if they can move.
            //It is called by the Character class.
            return boardWalls[xPos, yPos];
        }

        void RandomizeLayout(int numberOfRooms)
        {
            //Unfinished
            bool[,] room = boardWalls;
            List<Room> rooms = new List<Room>();

            for (int i = 0; i < numberOfRooms; i++)
            {
                int roomHeight = rnd.Next(2, 7);
                int roomWidth = rnd.Next(2, 7);
                int roomX = 0;
                int roomY = 0;
                if (i != 0)
                {
                    int connectedRoom = rnd.Next(0, i - 1);
                    roomX = rnd.Next(rooms[connectedRoom].positionX - (rooms[i].roomWidth / 2), rooms[connectedRoom].positionX + (rooms[connectedRoom].roomWidth / 2));
                    roomY = rnd.Next(rooms[connectedRoom].positionY - (rooms[i].roomHeight / 2), rooms[connectedRoom].positionY + (rooms[connectedRoom].roomHeight / 2));
                }
                rooms.Add(new Room(roomX, roomY, roomWidth, roomHeight, this));
            }

            

            //for (int i = 0; i < boardWalls.GetLength(0); i++)
            //{
            //    for (int j = 0; j < boardWalls.GetLength(1); j++)
            //    {
            //        boardWalls[i, j] = false;
            //    }
            //}

            //for (int k = 0; k < numberOfRooms; k++)
            //{
            //    int roomHeight = rnd.Next(2, 7);
            //    int roomWidth = rnd.Next(2, 7);
            //    int roomX = rnd.Next((int)(boardWalls.GetLength(1)*0.25f) + roomWidth, (int)(boardWalls.GetLength(1) * 0.75f) - 1);
            //    int roomY = rnd.Next((int)(boardWalls.GetLength(0) * 0.25f) + roomHeight, (int)(boardWalls.GetLength(0) * 0.75f) - 1);

            //    rooms.Add(new Room(roomX, roomY));

            //    for (int i = 0; i < boardWalls.GetLength(0); i++)
            //    {
            //        for (int j = 0; j < boardWalls.GetLength(1); j++)
            //        {
            //            if (Math.Abs(j - roomX) <= roomWidth && Math.Abs(i - roomY) <= roomHeight)
            //            {
            //                boardWalls[i, j] = true;
            //            }
            //        }
            //    }
            //}

        }

        public class Room
        {
            public int positionX;
            public int positionY;
            public int roomWidth;
            public int roomHeight;
            public Room(int posX, int posY, int newWidth, int newHeight, Board owner)
            {
                positionX = posX;
                positionY = posY;
                roomWidth = newWidth;
                roomHeight = newHeight;

                for (int i = positionX - (roomWidth / 2); i < positionX + (roomWidth + 2); i++)
                {
                    for (int j = positionY - (roomHeight / 2); j < positionY + (roomHeight + 2); j++)
                    {
                        owner.boardWalls[j, i] = true;
                    }
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

        string title;
        int offsetX = 0;
        int offsetY = 0;
        bool[,] dimensions;

        public Border(bool[,] walls, string label = "")
        {
            title = label;
            dimensions = walls;
            DrawBorder();
        }

        public Border(int sizeX, int sizeY, int newOffsetX, int newOffsetY, string label = "")
        {
            title = label;
            SquareWalls(sizeX, sizeY);
            offsetX = newOffsetX;
            offsetY = newOffsetY;
            DrawBorder();
        }

        public Border(bool[,] walls, int newOffsetX, int newOffsetY, string label = "")
        {
            title = label;
            dimensions = walls;
            offsetX = newOffsetX;
            offsetY = newOffsetY;
            DrawBorder();
        }

        void SquareWalls(int x, int y)
        {
            dimensions = new bool[x, y];

            for (int i = 0; i < dimensions.GetLength(1); i++)
            {
                for (int j = 0; j < dimensions.GetLength(0); j++)
                {
                    if (i == 0 || j == 0 || i == dimensions.GetLength(1) - 1 || j == dimensions.GetLength(0) - 1)
                    {
                        //This loop ends up here if we are at the edges of the map.
                        //Every position out here is a wall, so we set the value at this position in the array to true.
                        dimensions[j, i] = true;
                    }
                    else
                    {
                        //If we end up here, this is not a wall.
                        dimensions[j, i] = false;
                    }
                }
            }
        }

        void DrawLabel()
        {
            if (title.Length != 0)
            {
                Console.SetCursorPosition(((offsetX + (dimensions.GetLength(0) / 2)) - (title.Length / 2)), offsetY);
                Console.Write(title);
            }
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
                Console.SetCursorPosition(offsetX, offsetY + i);

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
            }
            DrawLabel();
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
                player.playerInventory.addItem(this);
                UndrawObject();
                owner = player;

                gameManager.infoBox.Write($"Picked up {name}!");
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
            stats = new Stats(10, 0, 0);
        }

        public override void UseItem()
        {
            //owner.stats.Health += 5;
            owner.stats.Add(stats);
            owner.playerInventory.Bag.Remove(this);
        }
    }

    public class Inventory
    {
        public List<Item> Bag = new List<Item>();
        GameManager gameManager;
        Menu menu;

        public Inventory(GameManager newManager)
        {
            gameManager = newManager;
        }

        public void addItem(Item newItem)
        {
            Bag.Add(newItem);
        }

        public void UseInventory(Player player)
        {
            menu = new Menu(gameManager.infoBox.sizeX, gameManager.infoBox.sizeY, gameManager.infoBox.offsetX, gameManager.infoBox.offsetY, "INVENTORY");

            if (Bag.Count > 0)
            {
                for (int i = 0; i < Bag.Count; i++)
                {
                    menu.Write($"{i}: {Bag[i].name}");
                }
                menu.Write("ITEMS:");
            }
            else
            {
                menu.Write("Your bag is empty!");
            }

            ConsoleKeyInfo input;
            do
            {
                int result;
                input = Console.ReadKey(true);
                if (int.TryParse(input.KeyChar.ToString(), out result))
                {
                    for (int i = 0; i < Bag.Count; i++)
                    {
                        if (result == i)
                        {
                            menu.Clear();
                            menu.Wipe();
                            menu.Write($"Using {Bag[i].name}!");
                            Bag[i].UseItem();
                            break;
                        }
                    }
                }
                else { break; }
            } while (input.Key != ConsoleKey.Escape);

            menu.Clear();
            player.gameManager.infoBox.Redraw();
        }

    }

    public struct Stats
    {
        public int Health;
        public int Strength;
        public int Accuracy;

        public Stats(int newHealth, int newStrength, int newAccuracy)
        {
            Health = newHealth;
            Strength = newStrength;
            Accuracy = newAccuracy;
        }

        public Stats Add(Stats opponent)
        {
            Health += opponent.Health;
            Strength += opponent.Strength;
            Accuracy += opponent.Accuracy;

            return this;
        }

    }

}
