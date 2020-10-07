using System;

namespace Adventure
{
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
            //Draw out a box with borders along the edges
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
            //Write the title of the box up top
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
                    return "═";
                case 4:
                case 8:
                case 12:
                    return "║";
                case 5:
                    return "╔";
                case 6:
                    return "╗";
                case 9:
                    return "╚";
                case 10:
                    return "╝";
                case 13:
                    return "╠";
                case 14:
                    return "╣";
                case 7:
                    return "╦";
                case 11:
                    return "╩";
                case 15:
                    return "╬";
                default:
                    throw new Exception("This should not happen");
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
}
