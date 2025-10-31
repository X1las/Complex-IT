namespace WebServiceLayer.Models;

// User Authentication & Profile
public class UserRegistrationDto
{
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

public class UserLoginDto
{
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

public class UserLoginResponseDto
{
    public string Url { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string? Token { get; set; }
}

public class UserProfileDto
{
    public string? Url { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
}

public class UserUpdateDto
{
    public string? NewUsername { get; set; }
    public string? NewPassword { get; set; }
}

// GET /api/users?page=0&pageSize=10
public class UserListResponseDto
{
    public List<UserProfileDto> Users { get; set; } = new();
    public PaginationDto Pagination { get; set; } = new();
}
public class PaginationDto
{
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalCount { get; set; }
    public int TotalPages { get; set; }
    public string? PreviousPage { get; set; }
    public string? NextPage { get; set; }
}