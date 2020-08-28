using System;
using System.Reflection.Metadata.Ecma335;

namespace ChessApp
{
    class Program
    {
        static void Main(string[] args)
        {
            var scale = 3; //width scale
            var width = 8;
            var height = 8;
            var alternater = 0;
            var widthChecker = 1;


            for(int h = 0; h < height; h++)
            {
                alternater++;
                for (int i = 0; i < width; i++)
                {
                    if(alternater % 2 != 0) //if odd
                    {

                        for (int j = 0; j < scale; j++)
                        {
                            Console.ForegroundColor = ConsoleColor.Yellow;
                            Console.Write("░");

                        }
                    }
                    else
                    {
                        for (int j = 0; j < scale; j++)
                        {
                            Console.ForegroundColor = ConsoleColor.Cyan;
                            Console.Write("▓");
                        }
                    }

                    alternater++;


                    if (width % width == 0 && widthChecker == width)
                    {
                        Console.WriteLine();
                        Console.ResetColor();
                        widthChecker = 0;
                    }
                    widthChecker++;
                }
            }
        }
    }
}
