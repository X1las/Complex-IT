using Microsoft.EntityFrameworkCore;
using WebServiceLayer.Models;
namespace DataServiceLayer;

public class CrewDataService
{
    public Crew? GetCrew(string id)
    {
        using var db = new ImdbContext();
        return db.Crew
            .FirstOrDefault(c => c.CrewId == id);
    }

    public (List<Crew> crew, int totalCount) SearchCrew(
        string query
)
    {
        using var db = new ImdbContext();

        var crewQuery = db.Crew
            .Where(c => c.Fullname!.ToLower().Contains(query.ToLower()))
            .AsQueryable();

        var totalCount = crewQuery.Count();

        var results = crewQuery
            .OrderBy(c => c.Fullname)
            .ToList();

        return (results, totalCount);
    }

    public (List<Crew> crew, int totalCount) GetCrew()
    {
        using var db = new ImdbContext();

        var query = db.Crew.AsQueryable();
        var totalCount = query.Count();

        var crew = query
            .OrderBy(c => c.Fullname)
            .ToList();

        return (crew, totalCount);
    }

    public List<CrewTitlesModel> GetCrewTitles(string crewId)
    {
        using var db = new ImdbContext();

        return db.Attend
            .Where(a => a.CrewId == crewId)
            .Join(db.Title,
                a => a.TitleId,
                t => t.Id,
                (a, t) => new CrewTitlesModel
                {
                    TitleId = t.Id,
                    Title = t.Title ?? string.Empty,
                    TitleType = t.TitleType,
                    Year = t.Year ?? string.Empty,
                    Rating = t.Rating ?? 0,
                    Url = $"/api/titles/{t.Id}"
                })
            .ToList();
    }

    public bool CrewExists(string crewId)
    {
        using var db = new ImdbContext();
        return db.Crew.Any(c => c.CrewId == crewId);
    }
}