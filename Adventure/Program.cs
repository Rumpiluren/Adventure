using System;

namespace Adventure
{
    class Program
    {
        static void Main(string[] args)
        {

            Board board = new Board(60, 30);

            Console.ReadKey();
        }
    }

    public class Character
    {

        int strength = 0;
        int health = 0;
        int accuracy = 0;
        public string symbol = "%";
        public int xPosition = 5;
        public int yPosition = 10;

        protected int [,] Move(int moveY, int moveX)
        {
            xPosition += moveX;
            yPosition += moveY;
            return new int[xPosition, yPosition];
        }
    }

    class Player : Character
    {

        public Player()
        {

        }

        void ReadInput()
        {
            var input = Console.ReadKey(false).Key;
            switch (input)
            {
                case ConsoleKey.UpArrow:
                    Move(1, 0);
                    return;
            }
        }
    }

    public class Board
    {
        public Board(Int32 x, Int32 y)
        {
            Int32[,] area = new Int32[y, x];
            Character[] characters = new Character[1];
            characters[0] = new Player();

            characters[0].

            for (int i = 0; i < area.GetLength(0); i++)
            {
                for (int j = 0; j < area.GetLength(1); j++)
                {
                    if (i == 0 || j == 0 || i == area.GetLength(0) - 1 || j == area.GetLength(1) - 1)
                    {
                        Console.Write("X");
                    }
                    else if (j == characters[0].yPosition && i == characters[0].xPosition)
                    {
                        Console.Write(characters[0].symbol);
                    }
                    else
                    {
                        Console.Write(" ");
                    }
                }
                Console.WriteLine("");
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
