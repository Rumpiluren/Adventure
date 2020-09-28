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

    public class Board
    {
        public Board(Int32 x, Int32 y)
        {
            Int32[,] area = new Int32[y, x];

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
