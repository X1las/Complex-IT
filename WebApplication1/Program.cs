using System;

namespace Assignment3
{
    class Program
    {
        public static void Main()
        {
            Console.WriteLine("Hello, World!");
        }
    }

    public class UrlParser
    {
        public bool HasId = false;
        public string? Path = null;
        public string? Id = null;
        public bool ParseUrl(string url)
        {
            string[] splitted = url.Split('/');
            Path = splitted[0] + "/" + splitted[1];
            if (splitted.Length > 2)
            {
                HasId = true;
                Id = splitted[2];
            }
            return true;
        }

    }

    public class RequestValidator
    {
        public Request ValidateRequest(Request request)
        {
            return request;
        }
    }

    public class Request
    {
        public string? Path = null;
        public string? Date = null;
        public bool IsValid = false;
        public string? Status = null;
        public string? Method = null;

    }

    public class CategoryService
    {

    }
}
