using System.Security.Claims;
using DataServiceLayer;
using DataServiceLayer.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using WebServiceLayer.Models;
using WebServiceLayer.Utils;

namespace WebApplicationTests
{
    /// Unit tests for BookmarksController
    /// Tests controller with logic from BookmarkDataService against the database
    /// Each test creates unique test data and cleans up after itself
    public class BookmarksControllerTests
    {
    private const string TestTitleId = "tt33982100"; // Angel of the Warrior - exists in DB

        // Helper to create a controller with an authenticated user
        private BookmarksController CreateAuthenticatedController(string username)
        {
            var logger = LoggerFactory.Create(builder => builder.AddConsole())
                .CreateLogger<BookmarksController>();
            
            var controller = new BookmarksController(logger);
            
            // Simulate authenticated user using Claims
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, username)
            };
            var identity = new ClaimsIdentity(claims, "TestAuth");
            var user = new ClaimsPrincipal(identity);
            
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = user,
                    Request =
                    {
                        Scheme = "http",
                        Host = new HostString("localhost:5001")
                    }
                }
            };

            return controller;
        }

        // Helper to generate unique username for each test to avoid conflicts
        private string GenerateUniqueUsername(string prefix = "testuser")
        {
            return $"{prefix}_{Guid.NewGuid().ToString().Substring(0, 8)}";
        }

        // Helper to generate unique title ID
        private string GenerateUniqueTitleId()
        {
            return $"tt{DateTime.Now.Ticks.ToString().Substring(0, 7)}";
        }

        [Fact]
        public async Task GetBookmarks_WhenUserHasNoBookmarks_ReturnsNotFound()
        {
            // Arrange
            var uniqueUser = "user_" + Guid.NewGuid().ToString().Substring(0, 8);
            var controller = CreateAuthenticatedController(uniqueUser);

            // Act
            var result = await controller.GetUserBookmarks(uniqueUser, 1, 10);

            // Assert
            var notFoundResult = result as NotFoundObjectResult;
            Assert.NotNull(notFoundResult);
            Assert.Equal(404, notFoundResult.StatusCode);
        }

        [Fact]
        public async Task GetBookmarks_WithDifferentUsername_ReturnsForbidden()
        {
            // Arrange - logged in as "user1"
            var controller = CreateAuthenticatedController("user1");

            // Act - trying to access "user2" bookmarks
            var result = await controller.GetUserBookmarks("user2", 1, 10);

            // Assert
            Assert.IsType<ForbidResult>(result);
        }

        [Fact]
        public async Task GetBookmarks_WithEmptyUsername_ReturnsForbidden()
        {
            // Arrange
            var controller = CreateAuthenticatedController("validuser");

            // Act
            var result = await controller.GetUserBookmarks("", 1, 10);

            // Assert
            // Empty username doesn't match authenticated user, so controller returns Forbid
            Assert.IsType<ForbidResult>(result);
        }

        [Fact]
        public async Task AddBookmark_WithValidData_ReturnsCreated()
        {
            // Arrange
            var uniqueUser = "testuser_" + Guid.NewGuid().ToString().Substring(0, 8);
            var titleId = "tt33982100"; // Angel of the Warrior - exists in DB
            
            var userService = new UserDataService(new Hashing());
            var createdUser = userService.RegisterUser(uniqueUser, "TestPassword123!");
            Assert.NotNull(createdUser); // Ensure user was created
            
            var controller = CreateAuthenticatedController(uniqueUser);
            var request = new UserBookmarkDto { Username = uniqueUser, TitleId = titleId };

            // Act
            var result = await controller.AddBookmark(request);

            // Assert
            var createdResult = result as CreatedResult;
            Assert.NotNull(createdResult);
            Assert.Equal(201, createdResult.StatusCode);

            var bookmarkService = new BookmarkDataService();
            bookmarkService.RemoveBookmark(uniqueUser, titleId);
            userService.DeleteUser(uniqueUser);
        }
        
        [Fact]
        public async Task AddBookmark_WhenDuplicate_ReturnsConflict()
        {
            // Arrange
            var uniqueUser = "testuser_" + Guid.NewGuid().ToString().Substring(0, 8);
            var titleId = "tt33982100"; // exists in DB
            
            // Create user first
            var userService = new UserDataService(new Hashing());
            userService.RegisterUser(uniqueUser, "TestPassword123!");
            
            var controller = CreateAuthenticatedController(uniqueUser);
            var request = new UserBookmarkDto { Username = uniqueUser, TitleId = titleId };

            // Add bookmark first time
            var bookmarkService = new BookmarkDataService();
            bookmarkService.AddBookmark(uniqueUser, titleId);

            // Act - try to add same bookmark again
            var result = await controller.AddBookmark(request);

            // Assert
            var conflictResult = result as ConflictObjectResult;
            Assert.NotNull(conflictResult);
            Assert.Equal(409, conflictResult.StatusCode);
            
            var error = conflictResult.Value as ErrorResponseDto;
            Assert.NotNull(error);
            Assert.Equal("Bookmark already exists", error.Error);

            bookmarkService.RemoveBookmark(uniqueUser, titleId);
            userService.DeleteUser(uniqueUser);
        }

        [Fact]
        public async Task AddBookmark_WithDifferentUsername_ReturnsForbidden()
        {
            // Arrange - logged in as "user1"
            var controller = CreateAuthenticatedController("user1");
            var request = new UserBookmarkDto { Username = "user2", TitleId = TestTitleId };

            // Act - trying to add bookmark for "user2"
            var result = await controller.AddBookmark(request);

            // Assert
            Assert.IsType<ForbidResult>(result);
        }

        [Fact]
        public async Task AddBookmark_WithNullRequest_ReturnsBadRequest()
        {
            // Arrange
            var controller = CreateAuthenticatedController("validuser");

            // Act
            var result = await controller.AddBookmark(null!);

            // Assert
            var badRequestResult = result as BadRequestObjectResult;
            Assert.NotNull(badRequestResult);
            Assert.Equal(400, badRequestResult.StatusCode);
        }

        [Fact]
        public async Task AddBookmark_WithEmptyTitleId_ReturnsBadRequest()
        {
            // Arrange
            var controller = CreateAuthenticatedController("validuser");
            var request = new UserBookmarkDto { Username = "validuser", TitleId = "" };

            // Act
            var result = await controller.AddBookmark(request);

            // Assert
            var badRequestResult = result as BadRequestObjectResult;
            Assert.NotNull(badRequestResult);
            Assert.Equal(400, badRequestResult.StatusCode);
        }

        [Fact]
        public async Task RemoveBookmark_WithValidData_ReturnsOk()
        {
            // Arrange
            var uniqueUser = "testuser_" + Guid.NewGuid().ToString().Substring(0, 8);
            var titleId = "tt33982100";
            
            // Create user first
            var userService = new UserDataService(new Hashing());
            userService.RegisterUser(uniqueUser, "TestPassword123!");
            
            var controller = CreateAuthenticatedController(uniqueUser);

            // Add a bookmark first
            var bookmarkService = new BookmarkDataService();
            bookmarkService.AddBookmark(uniqueUser, titleId);

            // Act
            var request = new UserBookmarkDto { Username = uniqueUser, TitleId = titleId };
            var result = await controller.RemoveBookmark(request);

            // Assert
            var okResult = result as OkResult;
            Assert.NotNull(okResult);
            Assert.Equal(200, okResult.StatusCode);
            
            userService.DeleteUser(uniqueUser);
        }

        [Fact]
        public async Task RemoveBookmark_WhenNotFound_ReturnsOk()
        {
            // Arrange
            var uniqueUser = "user_" + Guid.NewGuid().ToString().Substring(0, 8);
            var nonExistentTitle = "tt9999999";
            var controller = CreateAuthenticatedController(uniqueUser);

            // Act
            var request = new UserBookmarkDto { Username = uniqueUser, TitleId = nonExistentTitle };
            var result = await controller.RemoveBookmark(request);

            // Assert - RemoveBookmark returns Ok even if bookmark doesn't exist (idempotent operation)
            var okResult = result as OkResult;
            Assert.NotNull(okResult);
            Assert.Equal(200, okResult.StatusCode);
        }

        [Fact]
        public async Task RemoveBookmark_WithDifferentUsername_ReturnsForbidden()
        {
            // Arrange - logged in as "user1"
            var controller = CreateAuthenticatedController("user1");

            // Act - trying to delete "user2" bookmark
            var request = new UserBookmarkDto { Username = "user2", TitleId = TestTitleId };
            var result = await controller.RemoveBookmark(request);

            // Assert
            Assert.IsType<ForbidResult>(result);
        }

        [Fact]
        public async Task RemoveBookmark_WithEmptyUsername_ReturnsForbidden()
        {
            // Arrange
            var controller = CreateAuthenticatedController("validuser");

            // Act
            var request = new UserBookmarkDto { Username = "", TitleId = TestTitleId };
            var result = await controller.RemoveBookmark(request);

            // Assert: username doesn't match authenticated user => Forbid
            Assert.IsType<ForbidResult>(result);
        }

        [Fact]
        public async Task RemoveBookmark_WithEmptyTitleId_ReturnsBadRequest()
        {
            // Arrange
            var controller = CreateAuthenticatedController("validuser");

            // Act
            var request = new UserBookmarkDto { Username = "validuser", TitleId = "" };
            var result = await controller.RemoveBookmark(request);

            // Assert
            var badRequestResult = result as BadRequestObjectResult;
            Assert.NotNull(badRequestResult);
            Assert.Equal(400, badRequestResult.StatusCode);
        }

        [Fact]
        public async Task GetBookmarks_WithPagination_ReturnsCorrectStructure()
        {
            // Arrange
            var uniqueUser = "testuser_" + Guid.NewGuid().ToString().Substring(0, 8);
            
            // Create user first
            var userService = new UserDataService(new Hashing());
            userService.RegisterUser(uniqueUser, "TestPassword123!");
            
            var controller = CreateAuthenticatedController(uniqueUser);
            var bookmarkService = new BookmarkDataService();

            // Use real title IDs that exist (repeat the same known title)
            var titleId = "tt33982100";
            
            // Can only add 1 bookmark per user per title due to composite key
            bookmarkService.AddBookmark(uniqueUser, titleId);

            // Act - get page 1 with page size 10
            var result = await controller.GetUserBookmarks(uniqueUser, 1, 10);

            // Assert
            var okResult = result as OkObjectResult;
            Assert.NotNull(okResult);
            
            var response = okResult.Value as PagedResultDto<BookmarkDisplayItemDto>;
            Assert.NotNull(response);
            Assert.Equal(1, response.CurrentPage);
            Assert.Equal(10, response.PageSize);
            Assert.Equal(1, response.TotalItems);
            Assert.Equal(1, response.TotalPages);
            Assert.Single(response.Items);

            bookmarkService.RemoveBookmark(uniqueUser, titleId);
            userService.DeleteUser(uniqueUser);
        }

        [Fact]
        public async Task GetBookmarks_DefaultPagination_Uses10ItemsPerPage()
        {
            // Arrange
            var uniqueUser = "testuser_" + Guid.NewGuid().ToString().Substring(0, 8);
            
            // Create user first
            var userService = new UserDataService(new Hashing());
            userService.RegisterUser(uniqueUser, "TestPassword123!");
            
            var controller = CreateAuthenticatedController(uniqueUser);
            var bookmarkService = new BookmarkDataService();

            // Add 1 bookmark
            var titleId = "tt33982100";
            bookmarkService.AddBookmark(uniqueUser, titleId);

            // Act - default page size
            var result = await controller.GetUserBookmarks(uniqueUser, 1, 10);

            // Assert
            var okResult = result as OkObjectResult;
            Assert.NotNull(okResult);
            
            var response = okResult.Value as PagedResultDto<BookmarkDisplayItemDto>;
            Assert.NotNull(response);
            Assert.Equal(10, response.PageSize); // Default is 10

            bookmarkService.RemoveBookmark(uniqueUser, titleId);
            userService.DeleteUser(uniqueUser);
        }

        [Fact]
        public async Task GetBookmarks_Response_ContainsUrlReferences()
        {
            // Arrange
            var uniqueUser = "testuser_" + Guid.NewGuid().ToString().Substring(0, 8);
            var titleId = TestTitleId;
            
            // Create user first
            var userService = new UserDataService(new Hashing());
            userService.RegisterUser(uniqueUser, "TestPassword123!");
            
            var controller = CreateAuthenticatedController(uniqueUser);
            var bookmarkService = new BookmarkDataService();

            // Add a bookmark
            bookmarkService.AddBookmark(uniqueUser, titleId);

            // Act
            var result = await controller.GetUserBookmarks(uniqueUser, 1, 10);

            // Assert
            var okResult = result as OkObjectResult;
            Assert.NotNull(okResult);
            
            var response = okResult.Value as PagedResultDto<BookmarkDisplayItemDto>;
            Assert.NotNull(response);
            Assert.True(response.Items.Count > 0);
            
            // Check first item has URL
            var firstBookmark = response.Items[0];
            Assert.NotNull(firstBookmark.Url);
            Assert.Contains("http", firstBookmark.Url);
            Assert.Contains(uniqueUser, firstBookmark.Url);

            bookmarkService.RemoveBookmark(uniqueUser, titleId);
            userService.DeleteUser(uniqueUser);
        }
    }
}