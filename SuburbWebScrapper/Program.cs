using HtmlAgilityPack;
using SuburbWebScrapper;

class Program
{
    HashSet<string> areasToIgnore = new HashSet<string>
    {
    };

    static async Task Main(string[] args)
    {
        var baseUrl = "https://postcodez.com.au";
        var client = new HttpClient();
        client.BaseAddress = new Uri(baseUrl);




        var areasResponse = await client.GetAsync("/postcodes/nsw/sydney");
        var areasContent = await areasResponse.Content.ReadAsStringAsync();

        var areasDocument = new HtmlDocument();
        areasDocument.LoadHtml(areasContent);

        var areaNodes = areasDocument.DocumentNode.SelectNodes("//div[@class='row']/div/p/a");
        foreach (var areaNode in areaNodes)
        {
            
            // if areasToIgnore => continue

            var areaHref = areaNode.Attributes["href"].Value;
            var areaUrl = areaHref.StartsWith("//") ? "https:" + areaHref : baseUrl + areaHref;

            var response = await client.GetAsync(areaUrl);
            var pageContent = await response.Content.ReadAsStringAsync();

            var document = new HtmlDocument();
            document.LoadHtml(pageContent);

            var nodes = document.DocumentNode.SelectNodes("//a[@class='blink']");
            foreach (var node in nodes)
            {
                Console.WriteLine($"");
                var href = node.Attributes["href"].Value;
                var suburbPage = await client.GetAsync(href);
                var suburbContent = await suburbPage.Content.ReadAsStringAsync();

                var suburbDocument = new HtmlDocument();
                suburbDocument.LoadHtml(suburbContent);

                var suburbNode = suburbDocument.DocumentNode.SelectSingleNode("//div[p/strong[contains(text(), 'Suburb:')]]/following-sibling::div[1]");
                var postcodeNode = suburbDocument.DocumentNode.SelectSingleNode("//div[p/strong[contains(text(), 'Postcode:')]]/following-sibling::div[1]");
                var latitudeNode = suburbDocument.DocumentNode.SelectSingleNode("//div[p/strong[contains(text(), 'Latitude:')]]/following-sibling::div[1]");
                var longitudeNode = suburbDocument.DocumentNode.SelectSingleNode("//div[p/strong[contains(text(), 'Longitude:')]]/following-sibling::div[1]");
                if (suburbNode != null && postcodeNode != null)
                {
                    var suburb = suburbNode.InnerText.Trim();
                    var postcode = postcodeNode.InnerText.Trim();

                    if (float.TryParse(latitudeNode.InnerText.Trim(), out float latitude) && float.TryParse(longitudeNode.InnerText.Trim(), out float longitude))
                    {
                        CacheControl.isCached(suburb, postcode, longitude, latitude);
                    }
                    else
                    {
                        Console.WriteLine($"Failed to parse latitude or longitude for {suburb}");
                    }
                }
                else
                {
                    Console.WriteLine($"Failed to find data for {href}");
                }
            }
        }
    }




}