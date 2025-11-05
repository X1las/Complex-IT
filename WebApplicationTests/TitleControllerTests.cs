using DataServiceLayer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using WebServiceLayer;
using WebServiceLayer.Models;

namespace WebApplicationTests
{
    /// Unit tests for TitleController
    /// Demonstrates testing different layers: validation, service integration, pagination, and data format
    public class TitleControllerTests
    {
        private const string ValidTitleId = "tt33982100"; // Angel of the Warrior - exists in DB  
        private const string InvalidTitleId = "tt9999999";

        private TitleController CreateController()
        {
            var titleService = new TitleDataService();
            var controller = new TitleController(titleService);
            
            // Set up HttpContext for the controller
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    Request =
                    {
                        Scheme = "http",
                        Host = new HostString("localhost:5001")
                    }
                }
            };

            return controller;
        }

        /// Validation Layer - Tests that controller validates required parameters
        [Fact]
        public async Task GetTitle_WithEmptyId_ReturnsBadRequest()
        {
            // Arrange
            var controller = CreateController();

            // Act
            var result = await controller.GetTitle("");

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal(400, badRequestResult.StatusCode);
            
            var errorResponse = Assert.IsType<ErrorResponseDto>(badRequestResult.Value);
            Assert.Equal("Title ID is required", errorResponse.Error);
        }


        /// Validation Layer - Tests genre parameter validation

        [Fact]
        public async Task GetTitlesByGenre_WithEmptyGenre_ReturnsBadRequest()
        {
            // Arrange
            var controller = CreateController();

            // Act
            var result = await controller.GetTitlesBySpecificGenre("", 1, 10);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var errorResponse = Assert.IsType<ErrorResponseDto>(badRequestResult.Value);
            Assert.Equal("Genre is required", errorResponse.Error);
        }

        /// Integration Layer - Tests full integration with TitleDataService and database
        /// Verifies that controller can retrieve and return a valid title with all properties
        [Fact]
        public async Task GetTitle_WithValidId_ReturnsTitle()
        {
            // Arrange
            var controller = CreateController();

            // Act
            var result = await controller.GetTitle(ValidTitleId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var titleModel = Assert.IsType<TitleModel>(okResult.Value);
            
            Assert.Equal(ValidTitleId, titleModel.Id);
            Assert.NotEmpty(titleModel.Title);
            Assert.NotEmpty(titleModel.TitleType);
            // Verify DTO contains expected properties
            Assert.True(titleModel.Rating >= 0);
        }

        /// Integration Layer - Tests NotFound behavior when title doesn't exist
        [Fact]
        public async Task GetTitle_WithInvalidId_ReturnsNotFound()
        {
            // Arrange
            var controller = CreateController();

            // Act
            var result = await controller.GetTitle(InvalidTitleId);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal(404, notFoundResult.StatusCode);

            var errorResponse = Assert.IsType<ErrorResponseDto>(notFoundResult.Value);
            Assert.Equal("Title not found", errorResponse.Error);
        }
        
        /// Tests nested resource endpoint - title genres
        /// Validates that sub-resources are properly retrieved and formatted
        [Fact]
        public async Task GetTitleGenres_WithValidTitle_ReturnsGenres()
        {
            // Arrange
            var controller = CreateController();

            // Act
            var result = await controller.GetTitleGenres(ValidTitleId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var genres = Assert.IsType<List<DataServiceLayer.TitleGenres>>(okResult.Value);
            
            Assert.NotNull(genres);
            // Angel of the Warrior should have at least Action genre
            Assert.Contains(genres, g => g.Genre == "Action");
        }
        
        /// Tests GetGenres returns all available genres
        [Fact]
        public async Task GetGenres_ReturnsListOfGenres()
        {
            // Arrange
            var controller = CreateController();

            // Act
            var result = await controller.GetGenres();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var genres = Assert.IsType<List<string>>(okResult.Value);
            
            Assert.NotEmpty(genres);
            // Should contain common genres
            Assert.Contains("Drama", genres);
            Assert.Contains("Action", genres);
        }

        /// Tests alternate titles endpoint with pagination
        /// Validates additional title information is properly formatted
        [Fact]
        public async Task GetAlternateTitles_ReturnsCorrectFormat()
        {
            // Arrange
            var controller = CreateController();

            // Act
            var result = await controller.GetAlternateTitles(ValidTitleId, 1, 10);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var pagedResult = Assert.IsType<PagedResultDto<TitleAltsModel>>(okResult.Value);

            // Verify structure (may be empty if title has no alternates)
            Assert.NotNull(pagedResult);
            Assert.Equal(1, pagedResult.CurrentPage);
            Assert.True(pagedResult.TotalPages >= 1);
        }

        /// Tests that GetTitleGenres returns NotFound for invalid title
        /// Demonstrates proper error handling in nested resources
        [Fact]
        public async Task GetTitleGenres_WithInvalidTitle_ReturnsNotFound()
        {
            // Arrange
            var controller = CreateController();

            // Act
            var result = await controller.GetTitleGenres(InvalidTitleId);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            var errorResponse = Assert.IsType<ErrorResponseDto>(notFoundResult.Value);
            Assert.Equal("Title not found", errorResponse.Error);
        }

        /// Tests empty result handling - verifies system handles empty results gracefully
        [Fact]
        public async Task GetTitles_WithNoResults_ReturnsEmptyPagedResult()
        {
            // Arrange
            var controller = CreateController();
            string impossibleSearch = "XYZNONEXISTENTTITLE12345";

            // Act
            var result = await controller.GetTitles(1, 10, impossibleSearch);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var pagedResult = Assert.IsType<PagedResultDto<TitleModelShort>>(okResult.Value);
            
            // Should return empty list, not error
            Assert.NotNull(pagedResult.Items);
            Assert.Empty(pagedResult.Items);
            Assert.Equal(0, pagedResult.TotalItems);
        }
    }
}
