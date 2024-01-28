using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuburbWebScrapper;

public static class CacheControl
{
    private static readonly string filePath = "C:\\Users\\Moey\\Documents\\SuburbWebScrapper\\SuburbWebScrapper\\cache.txt";
    public static bool isCached(string Suburb, string Postcode, float longatude, float latitude) 
    {

        if (!File.Exists(filePath))
        {
            File.WriteAllText(filePath, $"{Suburb}:{Postcode}:{longatude}:{latitude}\n");
            return false;
        }

        var lines = File.ReadAllLines(filePath);

        if (lines.Any(line => line.StartsWith(Suburb + ":" +Postcode + ":")))
        {
            Console.Write($"{Suburb} cached");
            return true;
        }
        else
        {
            File.AppendAllText(filePath, $"{Suburb}:{Postcode}:{longatude}:{latitude} \n");
            Console.Write($"{Suburb}:{Postcode}:{longatude}:{latitude} \n");
            return false;
        }

    }
}
