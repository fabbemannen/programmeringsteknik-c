using System;

namespace leapyearcalculator
{
    class Program
    {
        static void Main(string[] args)
        {
            // Räkna ut hur många skottår som passerat mellan två inmatade värden.

            // DateTime.IsLeapYear(year) är en metod man kan använda.

            ConsoleKeyInfo cki;
            do
            {
                int startYear;
                int endYear;
                while (Console.KeyAvailable == false)
                {
                    int leapYears = 0;
                    Console.WriteLine("Write two years (YYYY) to check how many leap years have passed between them. 'Escape' to exit");
                    Console.Write("Year 1: ");
                    try
                    {
                        startYear = Convert.ToInt32(Console.ReadLine());
                        Console.Write("Year 2: ");
                        endYear = Convert.ToInt32(Console.ReadLine());
                    }
                    catch
                    {
                        Console.WriteLine("Invalid input. Try something like 1984");
                        continue;
                    }

                    if(startYear < 1 || startYear > 9999 || endYear < 1 || endYear > 9999)
                    {
                        Console.WriteLine("Enter a year between 1 and 9999. This is a Christian server apparently.");
                    }

                    if(startYear > endYear)
                    {
                        int temp = startYear;
                        startYear = endYear;
                        endYear = temp;
                    }

                    for(int year = startYear; year <= endYear; year++)
                    {
                        if (DateTime.IsLeapYear(year))
                        {
                            leapYears++;
                        }
                    }
                    Console.WriteLine("Number of leap years between " + startYear + " and " + endYear + " is " + leapYears + ".");
                }
                cki = Console.ReadKey(true);

            } while (cki.Key != ConsoleKey.Escape);

        }
    }
}
