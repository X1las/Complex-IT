namespace WebServiceLayer.Models;

public class HistoryItemDto
{
    public string? Url { get; set; }
    public int Id { get; set; }
    public string TitleId { get; set; } = string.Empty;
    public string? TitleName { get; set; }
    public DateTime ViewedAt { get; set; }
}