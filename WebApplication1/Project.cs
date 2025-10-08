using System.Text.Json;
using System.Text.Json.Serialization;

namespace Assignment3;

// UrlParser Class to parse URLs with optional ID segments

public class UrlParser // parses simple REST-like URLs and optional id segment
{
    string[] ValidSuperUrls = ["api","test"];
    string[] ValidSubUrls = ["categories","products"];
    public bool HasId { get; set; } // true when an ID segment exists in the URL
    public string Path { get; set; } = ""; // normalized base path (e.g. "/api/categories")
    public string? Id { get; set; } // optional id segment as string

    public bool ParseUrl(string url)
    {
        if (string.IsNullOrWhiteSpace(url)) return false; // handle null/empty Urls

        // Split path into parts
        var parts = url.Split('/');

        if (parts.Length > 2)
        {
            Path = '/' + parts[1] + '/' + parts[2];
        }
        else
        {
            Path = '/' + parts[1];
        }

        // Check if ID exists
            if (parts.Length > 3)
            {    // must have 2 or 3 segments
                if (parts[3] != null && parts[3] != "")
                {
                    Id = parts[3];
                    HasId = true;
                }
                else
                {
                    HasId = false;
                }
            }
            else
            {
                HasId = false;
            }

        // Check for valid paths
            if (parts[1] == "api" || parts[1] == "test")
        {
            if (parts[2] == "categories" || parts[2] == "products")
            {
                return true;
            }
            else if (parts[1] == "test")
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        else
        {
            return false;
        }
    }
}

// public class UrlParser // parses simple REST-like URLs and optional id segment
// {
//     string[] ValidSuperUrls = ["api","test"];
//     string[] ValidSubUrls = ["categories","products"];
//     public bool HasId { get; set; } // true when an ID segment exists in the URL
//     public string Path { get; set; } = ""; // normalized base path (e.g. "/api/categories")
//     public string? Id { get; set; } // optional id segment as string

//     // Default value: minSegments = 2 (a valid URL must have at least 2 parts)
//     public bool ParseUrl(string url, int minSegments = 2) // split URL and extract path + optional id
//     {
//         if (string.IsNullOrWhiteSpace(url)) return false; // invalid or empty URL
//         var parts = url.Split('/', StringSplitOptions.RemoveEmptyEntries);
//         // split and drop empty segments
//         if (parts.Length < minSegments) return false; // not enough segments to form a valid path
//         // Named parameter used when building Path
//         Path = parts[0] + "/" + parts[1]; // build base path from first segments
//         HasId = parts.Length > minSegments; // determine if an ID segment exists
//         Id = HasId ? parts[minSegments] : null; // extract ID if present

//         //if (Path.StartsWith("/api/")) Path = Path[4..]; // remove leading slash for consistency

//         if (parts[0] == "api" || parts[0] == "test")
//         {
//             if (parts[1] == "categories" || parts[1] == "products")
//             {
//                 return true; // valid URL structure
//             }
//             else if (parts[0] == "test")
//             {
//                 return true; // valid test URL structure
//             }
//             else
//             {
//                 return false; // invalid sub-url
//             }
//         }
//         else
//         {
//             return false; // invalid super-url
//         }
//     }
// }

// Simple Request and Response classes for RequestValidator class
public class Request
{
    [JsonPropertyName("method")]
    public string? Method { get; set; }

    [JsonPropertyName("path")]
    public string? Path { get; set; }

    [JsonPropertyName("date")]
    public string? Date { get; set; }

    [JsonPropertyName("body")]
    public string? Body { get; set; }
} // simple DTO for incoming request fields
public class Response
{
    [JsonPropertyName("status")]
    public string Status { get; set; } = "";

    [JsonPropertyName("body")]
    public string? Body { get; set; }
} // simple DTO for validator response status

public class RequestValidator // validates basic request semantics and JSON body when required
{
    string[] ok = [ "create", "read", "update", "delete", "echo" ]; // allowed methods (case-insensitive)

    public Response ValidateRequest(Request r, string label = "1 Ok") // validate fields and return status label
    {   
        if (string.IsNullOrWhiteSpace(r.Date)) return new() { Status = "0 missing date" }; // date required
        if (string.IsNullOrWhiteSpace(r.Method)) return new() { Status = "0 missing method" }; // method required
        if (!ok.Contains(r.Method, StringComparer.OrdinalIgnoreCase)) return new()
        {
            Status = "0 illegal method"
        }; // must be in allowed set
        if (string.IsNullOrWhiteSpace(r.Path) && r.Method?.ToLower() != "echo") return new()
        {
            Status = "0 missing path"
        }; // path required
        if (!long.TryParse(r.Date, out _)) return new()
        {
            Status = "0 illegal date"
        }; // date must be unix seconds numeric

        // For methods that require a body, check presence and JSON validity (except for echo which accepts any body)
        if (new[]
            {"create", "update", "echo"}.Contains(r.Method, StringComparer.OrdinalIgnoreCase))
        {
            if (string.IsNullOrWhiteSpace(r.Body)) return new()
            {
                Status = "0 missing body"
            }; // body required
            if (!string.Equals(r.Method, "echo", StringComparison.OrdinalIgnoreCase))
            {
                // echo does not need JSON parse
                try
                {
                    JsonDocument.Parse(r.Body);
                }
                catch
                {
                    return new() { Status = "0 illegal body" };
                }
            }
        }

        return new()
        {
            Status = label
        }; // all checks passed
    }
}

public class Category // Plain Old CLR Object (POCO) representing a category record
{
    [JsonPropertyName("cid")]
    public int Id { get; set; } // category identifier
    [JsonPropertyName("name")]
    public string Name { get; set; } // category display name
    public Category(int id = 0, string name = "") => (Id, Name) = (id, name); // constructor with default values
}

public  class CategoryService // in-memory service managing a list of categories
{
    


    private List<Category> add_categories;

     public CategoryService(List<Category> initialCategories)
    {
        add_categories = new(initialCategories); // laver en kopi for at beskytte intern data
    }
    //  List<Category> add_categories = new()
    // {
    //     new(id: 1, name: "Beverages"), 
    //     new(id: 2, name: "Condiments"),
    //     new(id: 3, name: "Confections")
    // };

    //public List<Category> GetCategories() => new(add_categories);
    public List<Category> GetCategories() => new(add_categories); // return shallow copy of list to protect internals
    public  Category? GetCategory(int id) => add_categories.FirstOrDefault(c => c.Id == id); // find by id or null ?) is a non-nullable reference type when nullable annotations are enabled the compiler assumes it should never be null and will warn if you return or assign null to it.
    public  bool UpdateCategory(int id, string name)
    {
        var c = GetCategory(id); if (c == null) return false; c.Name = name; return true; // update name if found
    }
    public  bool DeleteCategory(int id) => add_categories.RemoveAll(c => c.Id == id) > 0; // remove by id
    public  bool CreateCategory(int id, string name)
    {
        if (add_categories.Any(c => c.Id == id)) return false; // prevent duplicate ids
        add_categories.Add(new Category(id, name)); // add new category
        return true; // success
    }
}


// public class CategoryService
// {
    // private List<Category> add_categories;

    // public CategoryService(List<Category> initialCategories)
    // {
    //     add_categories = new(initialCategories); // laver en kopi for at beskytte intern data
    // }

    //public List<Category> GetCategories() => new(add_categories);

