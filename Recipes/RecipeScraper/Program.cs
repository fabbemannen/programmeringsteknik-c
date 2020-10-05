using HtmlAgilityPack;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading;

namespace RecipeScraper
{
    internal class Program
    {
        private static List<Recipe> recipeList = new List<Recipe>();

        private static readonly ConsoleColor originalColor = Console.ForegroundColor;
        private const ConsoleColor titleColor = ConsoleColor.Blue;
        private const ConsoleColor underTitleColor = ConsoleColor.Cyan;
        private const ConsoleColor variationColor = ConsoleColor.DarkGray;



        private static void Main(string[] args)
        {
            CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("sv-SE");

            //List of URLs to get recipes from
            List<string> recipeUrls = new List<string>
            {
                "https://www.koket.se/smorstekt-torskrygg-med-pestoslungad-blomkal-och-sparris",
                "https://www.koket.se/italiensk-kycklinggryta-med-vitt-vin",
                "https://www.koket.se/smakrik-o-lrisotto-med-vitlo-ksrostad-tomat-citronsparris-och-het-chorizo",
                "https://www.koket.se/smorbakad-spetskal-med-krispig-kyckling-och-graddskum"
            };

            foreach (string url in recipeUrls)
            {
                LoadRecipe(url);
            }

            Print(recipeList);
        }

        /// <summary>
        /// Scrape a webpage from koket.se for recipes
        /// </summary>
        /// <param name="url">URL to page with recipe</param>
        private static void LoadRecipe(string url)
        {
            try
            {
                HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(url);
                using (WebResponse webResponse = webRequest.GetResponse())
                {
                    Recipe recipe = new Recipe();
                    HtmlDocument htmlDocument = new HtmlDocument();

                    using (Stream content = webResponse.GetResponseStream())
                    {
                        using (MemoryStream buffer = new MemoryStream())
                        {

                            content.CopyTo(buffer);
                            buffer.Seek(0, SeekOrigin.Begin);
                            htmlDocument.Load(buffer);
                            buffer.Seek(0, SeekOrigin.Begin);
                            htmlDocument.Load(buffer);
                            JObject data = JsonConvert.DeserializeObject<JObject>(htmlDocument.GetElementbyId("__NEXT_DATA__").InnerHtml)
                                                            .SelectToken("props.pageProps.structuredData[?(@['\x40type']=='Recipe')]") as JObject;

                            recipe.Name = data["name"].ToObject<string>();

                            recipe.Description = data["description"].ToObject<string>();
                            recipe.ImageUrl = data["image"].ToObject<string>();
                            recipe.Ingredients = new List<Ingredient>();
                            foreach (JToken ingredient in data["recipeIngredient"] as JArray)
                            {
                                if (double.TryParse(((string)ingredient).Split(' ')[0], out double value))
                                {
                                    IEnumerable<string> name = ((string)ingredient).Split(' ').Skip(2);

                                    recipe.Ingredients.Add(new Ingredient
                                    {
                                        Value = value,
                                        Unit = ((string)ingredient).Split(' ')[1],
                                        Name = string.Join(" ", name)
                                    });
                                }
                                else
                                {
                                    recipe.Ingredients.Add(new Ingredient { Name = (string)ingredient });
                                }
                            }
                            recipe.Instructions = new List<string>();
                            foreach (JToken instruction in data["recipeInstructions"] as JArray)
                            {
                                recipe.Instructions.Add(new Regex("<[^>]*(>|$)").Replace((string)instruction["text"], string.Empty).Replace("\n", " "));
                            }

                            recipeList.Add(recipe);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ERROR: {ex.Message}");
            }
        }

        /// <summary>
        /// Print all entries in a list of recipes to the console
        /// </summary>
        /// <param name="recipeList">List of recipes</param>
        private static void Print(List<Recipe> recipeList)
        {
            foreach(var recipe in recipeList)
            {

                Console.ForegroundColor = titleColor;
                Console.WriteLine(@$"Recept:" + " " + recipe.Name.ToUpper());
                Console.ForegroundColor = originalColor;

                Console.WriteLine("-------");
                
                Console.WriteLine(recipe.Description.Trim());
                
                Console.WriteLine("-------");
                
                Console.ForegroundColor = underTitleColor;
                Console.WriteLine("Ingredienser:");
                Console.ForegroundColor = originalColor;

                int count = 0;
                foreach (var ingredient in recipe.Ingredients)
                {
                    Console.ForegroundColor = count % 2 == 0 ? originalColor : variationColor;

                    if (ingredient.Value == 0)
                    {
                        Console.WriteLine(ingredient.Name);
                    }
                    else
                    {
                        Console.WriteLine(ingredient.Value.ToString() + " " + ingredient.Unit + " " + ingredient.Name);
                    }
                    count++;
                }
                Console.ForegroundColor = originalColor;
                Console.WriteLine("-------");
                Console.ForegroundColor = underTitleColor;
                Console.WriteLine("Steg:");
                Console.ForegroundColor = originalColor;
                for (int i = 0; i < recipe.Instructions.Count; i++)
                {
                    Console.ForegroundColor = i % 2 == 0 ? originalColor : variationColor;
                    Console.WriteLine((i + 1).ToString() + ": " + recipe.Instructions[i]);
                }
                Console.ForegroundColor = originalColor;
                Console.WriteLine();
            }
        }
    }

    internal class Recipe
    {
        public string Name { get; set; }
        public string ImageUrl { get; set; }
        public string Description { get; set; }
        public List<Ingredient> Ingredients { get; set; }
        public List<string> Instructions { get; set; }
    }

    internal class Ingredient
    {
        public double Value { get; set; }
        public string Unit { get; set; }
        public string Name { get; set; }
    }
}
