using System;
using System.Collections.Generic;
using System.Globalization;

namespace tgif
{
    class Program
    {
        static void Main(string[] args)
        {
            // Skriv en applikation som läser in ett datum via användarinmatning,
            // som sedan räknar ut hur många dagar det är till nästa fredag.

            CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("sv-SE");

            var input = string.Empty;
            DateTime parsedInput;

            while (true)
            {
                do
                {

                    Console.Write("Write a date (sv-SE): ");
                    input = Console.ReadLine();

                } while (!DateTime.TryParse(input, out parsedInput));

                var currentDay = parsedInput.DayOfWeek;

                //Method 1 - Utilize the built-in enumerator of DayOfWeek
                //int weekday = (int)parsedInput.DayOfWeek;

                //Method 2 - Loop until we find Friday
                DateTime daysUntilFridayDT = parsedInput;
                int daysUntilFridayInt = 0;
                while(daysUntilFridayDT.DayOfWeek != DayOfWeek.Friday) 
                {
                    daysUntilFridayDT = daysUntilFridayDT.AddDays(1);
                    daysUntilFridayInt++;
                }
                Console.WriteLine($"(Method 2) {daysUntilFridayInt} days until Friday");

                //Method 3 - A dictionary that keeps track for you
                Dictionary<string, int> daysUntilFriday = new Dictionary<string, int>();
                daysUntilFriday.Add("Monday", 4);
                daysUntilFriday.Add("Tuesday", 3);
                daysUntilFriday.Add("Wednesday", 2);
                daysUntilFriday.Add("Thursday", 1);
                daysUntilFriday.Add("Friday", 0);
                daysUntilFriday.Add("Saturday", 6);
                daysUntilFriday.Add("Sunday", 5);

                Console.WriteLine(String.Format("That day was a {0}. {1}",
                    currentDay,
                    currentDay == DayOfWeek.Friday ?
                    "Yay! Weekend!" :
                    daysUntilFriday[currentDay.ToString()] + " day(s) until friday..."));
            }
        }
    }
}
