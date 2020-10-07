using System;
using System.Collections.Generic;

namespace Adventure
{
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
            boardEdges = new Border(boardWalls, "DUNGEON");
        }

        public bool isWall(int xPos, int yPos)
        {
            //This is used by any moving object to see if they can move.
            //It is called by the Character class.
            return boardWalls[xPos, yPos];
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
}
