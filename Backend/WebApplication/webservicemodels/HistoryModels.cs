namespace WebServiceLayer.Models;

public class HistoryDisplayItemDto
{
    public string? Url { get; set; }
    public int Id { get; set; }
    public string TitleId { get; set; } = string.Empty;
    public string? TitleName { get; set; }
    public DateTime ViewedAt { get; set; }
}

public class UserHistoryDto
{
    public string Username { get; set; } = string.Empty;
    public string TitleId { get; set; } = string.Empty;
    public DateTime? Date { get; set; }
}

