using Dapper;
using Microsoft.Data.SqlClient;
namespace SuburbCalulator;

public static class SuburbCalc
{
    private static List<Suburb> allSuburbs = spGetSuburbs();
    public static int count;

    public const double grace = 0.5 * -1;  //leave
    public static (List<Suburb> suburbs, int count) GetSuburbsWithinDistance(string subrg, double distance)
    {
        count = 0;
        var startingSuburb = GetSuburb(subrg, allSuburbs);
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

            if (dist <= distance || (distance - dist > grace && distance - dist < 0))
            {
                if (distance - dist > grace && distance - dist < 0) //if in grace region (just outside range)
                {
                      suburb.isJustOutofRange = true;
                    suburbsWithinDistance.Add(suburb);
                    continue;
                }
                suburb.isJustOutofRange = false;
                suburbsWithinDistance.Add(suburb);
                count++;
            }
        }
        return (suburbsWithinDistance, count);
    }

    public static double DistanceBetweenSuburbs(string subrg1, string subrg2) 
    {
        var startingSuburb=GetSuburb(subrg1, allSuburbs);
        var endingSuburb = GetSuburb(subrg2, allSuburbs);
        return Haversine.Distance(startingSuburb.longitude, startingSuburb.latitude, endingSuburb.longitude, endingSuburb.latitude);
    }
    public static Suburb GetSuburb(string subrg, List<Suburb> suburbs)
    {
        return suburbs.FirstOrDefault(s => s.rowguid == subrg);
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

