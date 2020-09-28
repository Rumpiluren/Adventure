﻿using System;

namespace Adventure
{
    class Program
    {
        static void Main(string[] args)
        {
            Board board = new Board(60, 30);

            Player player = new Player();
            player.board = board;
            player.StartingPosition();

            Character[] characters = new Character[1];
            characters[0] = new Skeleton();

            foreach (var character in characters)
            {
                character.board = board;
            }

            while (true)
            {
                player.ReadInput();
                foreach (var character in characters)
                {
                    character.DrawCharacter();
                }
                player.DrawCharacter();
            }

            Console.ReadKey();
        }
    }

    public class Character
    {
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
            if (board.isWall(xPosition + moveX, yPosition + moveY) != true)
            {
                xPosition += moveX;
                yPosition += moveY;
            }
            return new int[yPosition, xPosition];
        }

        public void DrawCharacter()
        {
            Console.SetCursorPosition(xPosition, yPosition);
            Console.Write(symbol);

            Console.SetCursorPosition(xPosition, yPosition);
            Console.Write("");
        }

        public void StartingPosition()
        {
            yPosition = random.Next(1, board.area.GetLength(0) - 1);
            xPosition = random.Next(1, board.area.GetLength(1) - 1);
        }

    }

    class Player : Character
    {
        public void ReadInput()
        {
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
        public Int32[,] area;
        public Board(Int32 x, Int32 y)
        {
            area = new Int32[y, x];

            for (int i = 0; i < area.GetLength(0); i++)
            {
                for (int j = 0; j < area.GetLength(1); j++)
                {
                    if (i == 0 || j == 0 || i == area.GetLength(0) - 1 || j == area.GetLength(1) - 1)
                    {
                        Console.Write("X");
                    }
                    else
                    {
                        Console.Write(" ");
                    }
                }
                Console.WriteLine("");
            }
        }

        public bool isWall(int xPos, int yPos)
        {
            if (yPos == 0 || xPos == 0 || yPos == area.GetLength(0) - 1 || xPos == area.GetLength(1) - 1)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

    }

    public class Border
    {
        public static void Execute()
        {
            int topleft = 218;
            int hline = 196;
            int topright = 191;
            int vline = 179;
            int bottomleft = 192;
            int bottomright = 217;

            Console.OutputEncoding = System.Text.Encoding.GetEncoding(28591);
            //draw top left corner
            Write(topleft);
            //draw top horizontal line
            for (int i = 0; i < 10; i++)
                Write(hline);
            //draw top right corner
            Write(topright);
            Console.WriteLine();
            //draw left and right vertical lines
            for (int i = 0; i < 6; i++)
            {
                Write(vline);
                for (int k = 0; k < 10; k++)
                {
                    Console.Write(" ");
                }
                WriteLine(vline);
            }
            //draw bottom left coner
            Write(bottomleft);
            //draw bottom horizontal line
            for (int i = 0; i < 10; i++)
                Write(hline);
            //draw bottom right coner
            Write(bottomright);
            Console.ReadKey();
        }
        static void Write(int charcode)
        {
            Console.Write((char)charcode);
        }
        static void WriteLine(int charcode)
        {
            Console.WriteLine((char)charcode);
        }
    }

}
