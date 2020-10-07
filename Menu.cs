using System;
using System.Collections.Generic;

namespace Adventure
{
    public class Menu
    {
        //Draw content within bounds.
        //Draw border around bounds.

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

            Clear();

            //If text was sent in, add the new text to the old text for future reference
            if (text.Length > 0)
            {
                oldMessages.Insert(0, ' ');
                oldMessages.Insert(0, '\n');
                oldMessages.InsertRange(0, text);
            }

            //Add old messages to the new text so that we can print it out.
            text = String.Join("", oldMessages.ToArray());

            //Split the string into separate words.
            string[] splitString = text.Split(new char[] { ' ' });

            Console.SetCursorPosition(offsetX + 1, offsetY + 1);

            for (int i = 0; i < splitString.Length; i++)
            {
                if (Console.CursorTop >= offsetY + sizeY - 2)
                {
                    //We have reached the bottom of the box. We cannot write anything more. Abort.
                    break;
                }

                if (Console.CursorLeft + splitString[i].Length >= offsetX + sizeX)
                {
                    //The current word will exceed the box's limits. Start a new line.
                    Console.SetCursorPosition(offsetX + 1, Console.CursorTop + 1);
                }

                if (Console.CursorLeft < offsetX + 1)
                {
                    //This is weird, but it happens. We are too far on the left, start a new line.
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
            //Wipe out all old messages!
            oldMessages = new List<char>();
        }

        public void Clear()
        {
            //remove all content from the menu
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

}
