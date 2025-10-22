using System;
using DataServiceLayer;

namespace WebServiceLayer;

public class PagedResult<T> // Page Wrapper
{
    public List<T> Items { get; set; } = new();    
    public int Total { get; set; } 
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int NumberOfPages => (int)Math.Ceiling((double)Total / PageSize);
    public string? Previous { get; set; }
    public string? Next { get; set; }
    public string? Current { get; set; }
}