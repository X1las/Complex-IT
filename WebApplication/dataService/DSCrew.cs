
using Microsoft.EntityFrameworkCore;

namespace DataServiceLayer;

public class DataService {

public Crew? GetCrewById(ImdbContext context, string crewId)
{
    return context.Crew.FirstOrDefault(c => c.CrewId == crewId);
}

    public List<Crew>? GetCrewByName(ImdbContext context, string crewName)
    {

        return context.Crew
            .Where(c => c.Fullname != null && c.Fullname.Contains(crewName))
            .ToList();
    }

public List<Titles>? GetTitlesByCrewId(ImdbContext context, string crewId)
{
    var titleIds = context.Attend
        .Where(tc => tc.CrewId == crewId)
        .Select(tc => tc.TitleId)
        .ToList();

    return context.Title
        .Where(t => titleIds.Contains(t.Id))
        .ToList();
}
}
