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
    /// <summary>
    /// Unit tests for RatingController
    /// Demonstrates testing different layers: controller validation, service integration, and data format
    /// </summary>
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
            var request = new CreateRatingDto { TitleId = TestTitleId, Rating = 8 };

            // Act
            var result = await controller.CreateRating(uniqueUser, request);

            // Assert
            var createdResult = result as CreatedAtActionResult;
            Assert.NotNull(createdResult);
            Assert.Equal(201, createdResult.StatusCode);
            
            var response = createdResult.Value as RatingDto;
            Assert.NotNull(response);
            Assert.Equal(TestTitleId, response.TitleId);
            Assert.Equal(8, response.Rating);

            // Cleanup
            new UserRatingDataService().DeleteRating(uniqueUser, TestTitleId);
            userService.DeleteUser(uniqueUser);
        }

        // Input validation - demonstrates controller-level validation logic
        [Fact]
        public async Task CreateRating_WithInvalidRating_ReturnsBadRequest()
        {
            // Arrange
            var controller = CreateAuthenticatedController("validuser");
            var request = new CreateRatingDto { TitleId = TestTitleId, Rating = 11 };

            // Act
            var result = await controller.CreateRating("validuser", request);

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
            var request = new CreateRatingDto { TitleId = TestTitleId, Rating = 7 };

            // Act - trying to create rating for "user2"
            var result = await controller.CreateRating("user2", request);

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
            
            var response = okResult.Value as RatingDto;
            Assert.NotNull(response);
            Assert.Equal(TestTitleId, response.TitleId);
            Assert.Equal(7, response.Rating);
            Assert.Contains("http", response.Url); // UI test: correct format

            // Cleanup
            ratingService.DeleteRating(uniqueUser, TestTitleId);
            userService.DeleteUser(uniqueUser);
        }

        // 5: Pagination - demonstrates UI data structure (PagedResultDto)
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

            // Assert - UI test: validate correct data format
            var okResult = result as OkObjectResult;
            Assert.NotNull(okResult);
            
            var response = okResult.Value as PagedResultDto<RatingDto>;
            Assert.NotNull(response);
            Assert.Equal(1, response.CurrentPage);
            Assert.Equal(10, response.PageSize);
            Assert.Equal(1, response.TotalItems);
            Assert.Single(response.Items);

            // Cleanup
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
            var updateDto = new UpdateRatingDto { Rating = 9 };

            // Act
            var result = await controller.UpdateRating(uniqueUser, TestTitleId, updateDto);

            // Assert
            var okResult = result as OkObjectResult;
            Assert.NotNull(okResult);
            
            var response = okResult.Value as RatingDto;
            Assert.NotNull(response);
            Assert.Equal(9, response.Rating); // Verify change persisted

            // Cleanup
            ratingService.DeleteRating(uniqueUser, TestTitleId);
            userService.DeleteUser(uniqueUser);
        }

        // Aggregate function - demonstrates TitleRatingController
        [Fact]
        public async Task GetAverageRating_CalculatesCorrectly()
        {
            // Arrange
            var user1 = GenerateUniqueUsername("user1");
            var user2 = GenerateUniqueUsername("user2");
            var userService = new UserDataService(new Hashing());
            userService.RegisterUser(user1, "TestPassword123!");
            userService.RegisterUser(user2, "TestPassword123!");
            
            var ratingService = new UserRatingDataService();
            ratingService.AddOrUpdateRating(user1, TestTitleId, 8);
            ratingService.AddOrUpdateRating(user2, TestTitleId, 6);
            
            var logger = LoggerFactory.Create(builder => builder.AddConsole())
                .CreateLogger<TitleRatingController>();
            var controller = new TitleRatingController(ratingService, logger);

            // Act
            var result = await controller.GetAverageRating(TestTitleId);

            // Assert
            var okResult = result as OkObjectResult;
            Assert.NotNull(okResult);
            
            var response = okResult.Value as AverageRatingDto;
            Assert.NotNull(response);
            Assert.Equal(7.0, response.AverageRating);
            Assert.Equal(2, response.TotalRatings);

            // Cleanup
            ratingService.DeleteRating(user1, TestTitleId);
            ratingService.DeleteRating(user2, TestTitleId);
            userService.DeleteUser(user1);
            userService.DeleteUser(user2);
        }
    }
}
