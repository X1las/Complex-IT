using System;
using DataServiceLayer;

namespace WebServiceLayer;

public class UserModel // UserModel DTO
{
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public List<Bookmarks> Bookmarks { get; set; } = new();
    public List<UserRatings> Ratings { get; set; } = new();
    public List<UserHistory> History { get; set; } = new();
}

public class UserHistory // UserHistory DTO
{
    public string? TitleId { get; set; }
    public DateTime Date { get; set; }  
}

public class UserRatings // UserRatings DTO
{
    public string? TitleId { get; set; }
    public double Rating { get; set; }
}

public class UserBookmarks // Bookmarks DTO
{
    public string? TitleId { get; set; }
}