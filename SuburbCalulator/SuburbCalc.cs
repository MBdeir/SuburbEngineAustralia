using Dapper;
using Microsoft.Data.SqlClient;
namespace SuburbCalulator;

public static class SuburbCalc
{
    public static List<Suburb> GetSuburbsWithinDistance(string pcrg, double distance)
    {
        List<Suburb> allSuburbs = spGetSuburbs();

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
            }
        }

        return suburbsWithinDistance;
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
            var query = "SELECT name, postcode, rowguid, longitude, latitude FROM postcode";
            cn.Open();
            var suburbs = cn.Query<Suburb>(query).ToList();
            return suburbs;
        }
    }
}

