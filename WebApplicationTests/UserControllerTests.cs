// using System.Net;
// using System.Net.Http;
// using System.Text;
// using Newtonsoft.Json;
// using Newtonsoft.Json.Linq;
// using Xunit;

// namespace WebServiceTests.Tests
// {
//     public class UserControllerTests
//     {
//         private const string BaseUrl = "http://localhost:5001/api/users";

//         [Fact]
//         public void PostBookmark_ForUser_ReturnsCreated()
//         {
//             var username = "testuser_integration";
//             var payload = new { TitleId = "tt1234567" };

//             var (respObj, status) = PostData($"{BaseUrl}/{username}/bookmarks", payload);

//             Assert.Equal(HttpStatusCode.Created, status);
//         }

//         [Fact]
//         public void GetBookmarks_ForUser_ReturnsListOrNotFound()
//         {
//             var username = "testuser_integration";

//             var (array, status) = GetArray($"{BaseUrl}/{username}/bookmarks");

//             // Either NotFound (no bookmarks) or OK with an array
//             Assert.True(status == HttpStatusCode.OK || status == HttpStatusCode.NotFound);
//             if (status == HttpStatusCode.OK)
//             {
//                 Assert.True(array.Count >= 0);
//             }
//         }

//         // Helpers
//         (JObject, HttpStatusCode) PostData(string url, object content)
//         {
//             var client = new HttpClient();
//             var requestContent = new StringContent(
//                 JsonConvert.SerializeObject(content),
//                 Encoding.UTF8,
//                 "application/json");
//             var response = client.PostAsync(url, requestContent).Result;
//             var data = response.Content.ReadAsStringAsync().Result;
//             return ((JObject)JsonConvert.DeserializeObject(data), response.StatusCode);
//         }

//         (JArray, HttpStatusCode) GetArray(string url)
//         {
//             var client = new HttpClient();
//             var response = client.GetAsync(url).Result;
//             var data = response.Content.ReadAsStringAsync().Result;
//             return (string.IsNullOrWhiteSpace(data) ? new JArray() : (JArray)JsonConvert.DeserializeObject(data), response.StatusCode);
//         }
//     }
// }
