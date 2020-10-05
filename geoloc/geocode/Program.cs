using NGeoNames;
using NGeoNames.Entities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace geocode
{
    class Program
    {
        static readonly IEnumerable<ExtendedGeoName> _locationNames;
        static readonly ReverseGeoCode<ExtendedGeoName> _reverseGeoCodingService;

        static Program()
        {
            _locationNames = GeoFileReader.ReadExtendedGeoNames(".\\Resources\\SE.txt").ToArray();
            _reverseGeoCodingService = new ReverseGeoCode<ExtendedGeoName>(_locationNames);
        }

        static void Main(string[] args)
        {
            CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;
            
            // 1. Hitta de 10 närmsta platserna till Gävle (60.674622, 17.141830), sorterat på namn.
            
            var searchResult = _locationNames.Where(n => n.Name.Equals("Gävle Kommun", StringComparison.OrdinalIgnoreCase)).First();

            var results = _reverseGeoCodingService.RadialSearch(searchResult, 10);
            // Print the results
            var sortedResults = results.OrderBy(n => n.Name);
            foreach (var r in sortedResults)
            {
                Console.WriteLine(
                    string.Format(
                        CultureInfo.InvariantCulture, "{3:F4}km {2} ({0}, {1})",
                        r.Latitude, r.Longitude, r.Name, r.DistanceTo(searchResult) / 1000
                    )
                );
            }

            Console.WriteLine();

            // 2. Hitta alla platser inom 200 km radie till Uppsala (59.858562, 17.638927), sorterat på avstånd.
            
            searchResult = _locationNames.Where(n => n.Name.Equals("Uppsala", StringComparison.OrdinalIgnoreCase)).First();

            results = _reverseGeoCodingService.RadialSearch(searchResult, 200000.0, 50);
            // Print the results
            int count = 0;
            foreach (var r in results)
            {
                count++;
                Console.WriteLine(count + ": " +
                    string.Format(
                        CultureInfo.InvariantCulture, "{3:F4}km {2} ({0}, {1})",
                        r.Latitude, r.Longitude, r.Name, r.DistanceTo(searchResult) / 1000
                    )
                );
            }

            Console.WriteLine();

            // 3. Lista 10 platser baserat på användarinmatning lat long.
            while (true)
            {
                Console.WriteLine("Enter a longitud and latitud (e.g. 60.51515:17.151515 to search or press enter to exit");
                Console.Write("Longitud:Latitud = ");
                string searchQuery = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(searchQuery))
                {
                    break;
                }

                string[] coordinates = searchQuery.Split(':');
             
                double longitud = Convert.ToDouble(coordinates[0]);
                double latitud = Convert.ToDouble(coordinates[1]);

                results = _reverseGeoCodingService.RadialSearch(longitud, latitud, 10);

                // Print the results
                count = 0;
                foreach (var r in results)
                {
                    count++;
                    Console.WriteLine(count + ": " +
                        string.Format(
                            CultureInfo.InvariantCulture, "{3:F4}km {2} ({0}, {1})",
                            r.Latitude, r.Longitude, r.Name, r.DistanceTo(searchResult) / 1000
                        )
                    );
                }
            }
        }
    }
}
