using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SuburbCalulator;
namespace tester;

public static class Function1
{
    [FunctionName("Function1")]
    public static async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "get", Route = null)] HttpRequest req,ILogger log)
    {
        string pcrg1 = req.Query["pcrg1"];
        string pcrg2 = req.Query["pcrg2"];

        string distance1 = req.Query["distance"];


        //int distance = Int32.Parse(distance1);


        double distance = SuburbCalc.DistanceBetweenSuburbs(pcrg1, pcrg2);

        //var result = SuburbCalc.GetSuburbsWithinDistance(pcrg1, distance);

        //        return new OkObjectResult(new { count = result.count, avaliableSuburbs = result.suburbs });

        //Bexley and Turrella
        return new OkObjectResult(distance);
    }
}
