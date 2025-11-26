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

    public (List<Crew>? crew, int totalCount) SearchCrew(
        string query,
        List<Crew> crew)
    {
        using var db = new ImdbContext();

        var filteredQuery = crew.AsQueryable();

        if (!string.IsNullOrEmpty(query))
        {
            filteredQuery = filteredQuery.Where(c =>
                c.Fullname != null && c.Fullname.Contains(query));
        }

        var totalCount = filteredQuery.Count();
        var results = filteredQuery
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

    public (List<CrewTitlesModel>? titles, int totalCount) GetCrewTitles(string crewId)
    {
        using var db = new ImdbContext();

        var query = from attend in db.Attend
                    join title in db.Title on attend.TitleId equals title.Id
                    where attend.CrewId == crewId
                    select new CrewTitlesModel
                    {
                        TitleId = title.Id,
                        Title = title.OriginalTitle ?? string.Empty,
                        TitleType = title.TitleType,
                        Year = title.StartYear ?? string.Empty,
                        Rating = title.Rating ?? 0,
                        Url = $"/titles/{title.Id}"
                    };

        var titles = query.ToList();
        var totalCount = titles.Count;

        return (titles, totalCount);
    }
}