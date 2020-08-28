using Microsoft.VisualBasic.CompilerServices;
using System;

namespace WordsApp
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Write("Skriv något vackert: ");
            //Read input and change to lowercase for simplicity
            string input = Console.ReadLine().ToLower();

            //Split input into words
            var inputArray = input.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            var wordCount = 0;
            var vocalCount = 0;
            string longestWord = "";
            char[] charArray;

            //List of vowels
            char[] vowels = new char[] { 'a', 'e', 'i', 'o', 'u', 'y', 'å', 'ä', 'ö' };

            foreach(var word in inputArray)
            {
                //Console.WriteLine(word);

                //Count words
                wordCount++;
                charArray = word.ToCharArray();

                //Check if current word is longer than the length of the longest word
                if (charArray.Length > longestWord.Length)
                {
                    longestWord = word;
                }

                //Go through each letter in each word and then iterate through list of vowels to check for matches
                foreach (var letter in charArray)
                {
                    foreach(var vowel in vowels)
                    {
                        //Comparison might be improved on?
                        if(vowel.CompareTo(letter) == 0)
                        {
                            vocalCount++;
                            continue;
                        }
                    }
                }
            }

            Console.WriteLine("Words: " + wordCount + " Vocals: " + vocalCount + " Longest: " + longestWord);
        }
    }
}
