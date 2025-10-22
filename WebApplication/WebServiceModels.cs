
namespace WebServiceLayer;

public class TitleModel
{
    public string Id { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Year { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
}

public class UserModel
{
    public string Username { get; set; } = string.Empty;
    public string Pswd { get; set; } = string.Empty;
}

public class CrewModel
{
    public string TitleId { get; set; } = string.Empty;
    public string PersonId { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
}

public class SearchModel
{
    public string Query { get; set; } = string.Empty;
}

