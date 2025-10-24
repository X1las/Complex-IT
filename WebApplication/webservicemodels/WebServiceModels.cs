using System;
using DataServiceLayer;

namespace WebServiceLayer.Models;

public class PagedResultDto<T>
{
    public int CurrentPage { get; set; }
    public int PageSize { get; set; }
    public int TotalItems { get; set; }
    public int TotalPages { get; set; }
    public string? PreviousPage { get; set; }
    public string? NextPage { get; set; }
    public List<T> Items { get; set; } = new();
}

public class ErrorResponseDto
{
    public string Error { get; set; } = string.Empty;
    public string? Details { get; set; }
}

public class SuccessResponseDto
{
    public string Message { get; set; } = string.Empty;
    public object? Data { get; set; }
}