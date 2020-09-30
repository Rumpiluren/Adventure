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

            //Next, we spawn the player and draw them on the screen.
            Player player = new Player();
            player.board = board;
            player.RandomizePosition();
            player.DrawCharacter();

            //Next, we spawn enemies. Currently, we only spawn one on a fixed location.
            Character[] characters = new Character[1];
            characters[0] = new Skeleton();
            foreach (var character in characters)
            {
                character.board = board;
                character.DrawCharacter();
            }

            //This here loops indefinitely. We read player input, then redraw all characters in the scene.
            while (true)
            {
                player.ReadInput();
                foreach (var character in characters)
                {
                    character.DrawCharacter();
                }
                player.DrawCharacter();
            }
        }
    }

    public class Character
    {
        //Base class for each creature in the game, including the player.
        Random random = new Random();

        int strength = 0;
        int health = 0;
        int accuracy = 0;
        public string symbol = "%";
        public int xPosition = 5;
        public int yPosition = 10;
        public Board board;

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

        public void DrawCharacter()
        {
            //This here is where we draw our character onto the screen.
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

    }

    class Player : Character
    {
        public void ReadInput()
        {
            //This here is where we end up after each button press.
            //We read the user input,
            //and if the input equals any of the arrow keys, we call the 'Move' function from the parent class.
            var input = Console.ReadKey(false).Key;
            switch (input)
            {
                case ConsoleKey.UpArrow:
                    Move(-1, 0);
                    return;
                case ConsoleKey.DownArrow:
                    Move(1, 0);
                    return;
                case ConsoleKey.LeftArrow:
                    Move(0, -1);
                    return;
                case ConsoleKey.RightArrow:
                    Move(0, 1);
                    return;
            }


        }
    }

    class Skeleton : Character
    {
        public Skeleton()
        {
            symbol = "#";
        }
    }

    public class Board
    {
        //This class is in charge of knowing the layout of the level and the location of all the walls.
        //It owns an instance of the Border class, which in turn is in charge of drawing the walls visually.
        public bool[,] boardWalls;
        Border boardEdges;
        Random rnd = new Random();
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

        void DrawBorder()
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
}
