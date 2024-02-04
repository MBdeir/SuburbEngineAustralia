using Dapper;
using Microsoft.Data.SqlClient;
namespace SuburbCalulator;

public static class SuburbCalc
{
    private static List<Suburb> allSuburbs = spGetSuburbs();
    public static int count;
    public static (List<Suburb> suburbs, int count) GetSuburbsWithinDistance(string pcrg, double distance)
    {
        count = 0;
        var startingSuburb = GetSuburb(pcrg, allSuburbs);
        if (startingSuburb == null)
        {
            throw new ArgumentException("Starting suburb not found.");
        }
        List<Suburb> suburbsWithinDistance = new List<Suburb>();
        foreach (var suburb in allSuburbs)
        {
            double dist = Haversine.Distance(
                startingSuburb.longitude, startingSuburb.latitude,
                suburb.longitude, suburb.latitude);

            if (dist <= distance)
            {
                suburbsWithinDistance.Add(suburb);
                count++;
            }
        }
        return (suburbsWithinDistance, count);
    }

    public static double DistanceBetweenSuburbs(string pcrg1, string pcrg2) 
    {
        var startingSuburb=GetSuburb(pcrg1, allSuburbs);
        var endingSuburb = GetSuburb(pcrg2, allSuburbs);
        return Haversine.Distance(startingSuburb.longitude, startingSuburb.latitude, endingSuburb.longitude, endingSuburb.latitude);
    }
    public static Suburb GetSuburb(string pcrg, List<Suburb> suburbs)
    {
        return suburbs.FirstOrDefault(s => s.rowguid == pcrg);
    }
    public static List<Suburb> spGetSuburbs()
    {
        var connectionString = Environment.GetEnvironmentVariable("WJconnectionString");
        using (var cn = new SqlConnection(connectionString))
        {
            var query = "SELECT name, postcode, rowguid, longitude, latitude FROM suburb";
            cn.Open();
            var suburbs = cn.Query<Suburb>(query).ToList();
            return suburbs;
        }
    }
}

