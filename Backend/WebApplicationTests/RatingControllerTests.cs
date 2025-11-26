using System.Security.Claims;
using DataServiceLayer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using WebServiceLayer.Controllers;
using WebServiceLayer.Models;
using WebServiceLayer.Utils;

namespace WebApplicationTests
{
    /// Unit tests for RatingController
    /// Demonstrates testing different layers: controller validation, service integration, and data format
    public class RatingControllerTests
    {
        private const string TestTitleId = "tt33982100"; // Angel of the Warrior - exists in DB

        private RatingController CreateAuthenticatedController(string username)
        {
            var ratingService = new UserRatingDataService();
            var logger = LoggerFactory.Create(builder => builder.AddConsole())
                .CreateLogger<RatingController>();
            
            var controller = new RatingController(ratingService, logger);
            
            var claims = new List<Claim> { new Claim(ClaimTypes.Name, username) };
            var identity = new ClaimsIdentity(claims, "TestAuth");
            var user = new ClaimsPrincipal(identity);
            
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = user,
                    Request = { Scheme = "http", Host = new HostString("localhost:5001") }
                }
            };

            return controller;
        }

        private string GenerateUniqueUsername(string prefix = "testuser")
        {
            return $"{prefix}_{Guid.NewGuid().ToString().Substring(0, 8)}";
        }

        // Demonstrates full integration with service and DB
        [Fact]
        public async Task CreateRating_WithValidData_ReturnsCreated()
        {
            // Arrange
            var uniqueUser = GenerateUniqueUsername();
            var userService = new UserDataService(new Hashing());
            userService.RegisterUser(uniqueUser, "TestPassword123!");
            
            var controller = CreateAuthenticatedController(uniqueUser);
            var request = new UserRatingDto { Username = uniqueUser, TitleId = TestTitleId, Rating = 8 };

            // Act
            var result = await controller.CreateRating(request);

            // Assert
            var createdResult = result as CreatedResult;
            Assert.NotNull(createdResult);
            Assert.Equal(201, createdResult.StatusCode);

            new UserRatingDataService().DeleteRating(uniqueUser, TestTitleId);
            userService.DeleteUser(uniqueUser);
        }

        // Input validation - demonstrates controller-level validation logic
        [Fact]
        public async Task CreateRating_WithInvalidRating_ReturnsBadRequest()
        {
            // Arrange
            var controller = CreateAuthenticatedController("validuser");
            var request = new UserRatingDto { Username = "validuser", TitleId = TestTitleId, Rating = 11 };

            // Act
            var result = await controller.CreateRating(request);

            // Assert
            var badRequestResult = result as BadRequestObjectResult;
            Assert.NotNull(badRequestResult);
            Assert.Equal(400, badRequestResult.StatusCode);
            
            var error = badRequestResult.Value as ErrorResponseDto;
            Assert.NotNull(error);
            Assert.Equal("Rating must be between 1 and 10", error.Error);
        }

        // Authorization - demonstrates security validation
        [Fact]
        public async Task CreateRating_WithDifferentUsername_ReturnsForbidden()
        {
            // Arrange - logged in as "user1"
            var controller = CreateAuthenticatedController("user1");
            var request = new UserRatingDto { Username = "user2", TitleId = TestTitleId, Rating = 7 };

            // Act - trying to create rating for "user2"
            var result = await controller.CreateRating(request);

            // Assert
            Assert.IsType<ForbidResult>(result);
        }

        // GET - demonstrates retrieval and data format
        [Fact]
        public async Task GetRating_WhenExists_ReturnsOkWithCorrectFormat()
        {
            // Arrange
            var uniqueUser = GenerateUniqueUsername();
            var userService = new UserDataService(new Hashing());
            userService.RegisterUser(uniqueUser, "TestPassword123!");
            
            var ratingService = new UserRatingDataService();
            ratingService.AddOrUpdateRating(uniqueUser, TestTitleId, 7);
            
            var controller = CreateAuthenticatedController(uniqueUser);

            // Act
            var result = await controller.GetRating(uniqueUser, TestTitleId);

            // Assert
            var okResult = result as OkObjectResult;
            Assert.NotNull(okResult);
            
            var response = okResult.Value as RatingDisplayItemDto;
            Assert.NotNull(response);
            Assert.Equal(TestTitleId, response.TitleId);
            Assert.Equal(7, response.Rating);
            Assert.Contains("http", response.Url); // correct format

            ratingService.DeleteRating(uniqueUser, TestTitleId);
            userService.DeleteUser(uniqueUser);
        }

        // Pagination - demonstrates UI data structure (PagedResultDto)
        [Fact]
        public async Task GetAllRatings_ReturnsPaginatedFormat()
        {
            // Arrange
            var uniqueUser = GenerateUniqueUsername();
            var userService = new UserDataService(new Hashing());
            userService.RegisterUser(uniqueUser, "TestPassword123!");
            
            var ratingService = new UserRatingDataService();
            ratingService.AddOrUpdateRating(uniqueUser, TestTitleId, 8);
            
            var controller = CreateAuthenticatedController(uniqueUser);

            // Act
            var result = await controller.GetAllRatings(uniqueUser, 1, 10);

            // Assert validate correct data format
            var okResult = result as OkObjectResult;
            Assert.NotNull(okResult);
            
            var response = okResult.Value as PagedResultDto<RatingDisplayItemDto>;
            Assert.NotNull(response);
            Assert.Equal(1, response.CurrentPage);
            Assert.Equal(10, response.PageSize);
            Assert.Equal(1, response.TotalItems);
            Assert.Single(response.Items);

            ratingService.DeleteRating(uniqueUser, TestTitleId);
            userService.DeleteUser(uniqueUser);
        }

        // Update operation - demonstrates modification logic
        [Fact]
        public async Task UpdateRating_ChangesExistingRating()
        {
            // Arrange
            var uniqueUser = GenerateUniqueUsername();
            var userService = new UserDataService(new Hashing());
            userService.RegisterUser(uniqueUser, "TestPassword123!");
            
            var ratingService = new UserRatingDataService();
            ratingService.AddOrUpdateRating(uniqueUser, TestTitleId, 5);
            
            var controller = CreateAuthenticatedController(uniqueUser);
            var updateDto = new UserRatingDto { Username = uniqueUser, TitleId = TestTitleId, Rating = 9 };

            // Act
            var result = await controller.UpdateRating(updateDto);

            // Assert
            var createdResult = result as CreatedResult;
            Assert.NotNull(createdResult);
            Assert.Equal(201, createdResult.StatusCode);
            
            // Verify the rating was actually updated
            var updatedRating = ratingService.GetUserRating(uniqueUser, TestTitleId);
            Assert.NotNull(updatedRating);
            Assert.Equal(9, updatedRating.Rating);

            ratingService.DeleteRating(uniqueUser, TestTitleId);
            userService.DeleteUser(uniqueUser);
        }

        // DELETE operation - demonstrates deletion logic
        [Fact]
        public async Task DeleteRating_RemovesExistingRating()
        {
            // Arrange
            var uniqueUser = GenerateUniqueUsername();
            var userService = new UserDataService(new Hashing());
            userService.RegisterUser(uniqueUser, "TestPassword123!");
            
            var ratingService = new UserRatingDataService();
            ratingService.AddOrUpdateRating(uniqueUser, TestTitleId, 7);
            
            var controller = CreateAuthenticatedController(uniqueUser);
            var deleteDto = new UserRatingDto { Username = uniqueUser, TitleId = TestTitleId };

            // Act
            var result = await controller.DeleteRating(deleteDto);

            // Assert
            var okResult = result as OkObjectResult;
            Assert.NotNull(okResult);
            Assert.Equal(200, okResult.StatusCode);
            
            // Verify rating was actually deleted
            var deletedRating = ratingService.GetUserRating(uniqueUser, TestTitleId);
            Assert.Null(deletedRating);

            userService.DeleteUser(uniqueUser);
        }
    }
}
