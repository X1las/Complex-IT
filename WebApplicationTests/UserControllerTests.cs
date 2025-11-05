using System.Security.Claims;
using DataServiceLayer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using WebServiceLayer;
using WebServiceLayer.Models;
using WebServiceLayer.Utils;

namespace WebApplicationTests
{
    /// Unit tests for UserController
    /// Focuses on UNIQUE functionality: JWT token generation and password hashing
    /// Note: Basic validation/integration patterns already demonstrated in other controller tests
    public class UserControllerTests
    {
        private UserController CreateController(string? authenticatedUsername = null)
        {
            var context = new ImdbContext();
            var dataService = new UserDataService(new Hashing());
            var hashing = new Hashing();
            
            // Setup configuration for JWT (64 character hex string for 512-bit HS512 algorithm)
            var inMemorySettings = new Dictionary<string, string>
            {
                {"Auth:Secret", "6DAFE0A8C4DCEA659EBE8F4085579886D1DA54893C8EF530B6F6C7AAAF44013A"}
            };
            IConfiguration configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(inMemorySettings!)
                .Build();
            
            var logger = LoggerFactory.Create(builder => builder.AddConsole())
                .CreateLogger<UserController>();
            
            var controller = new UserController(context, dataService, hashing, configuration, logger);
            
            // Setup authenticated user if provided
            if (!string.IsNullOrEmpty(authenticatedUsername))
            {
                var claims = new List<Claim> { new Claim(ClaimTypes.Name, authenticatedUsername) };
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
            }
            else
            {
                controller.ControllerContext = new ControllerContext
                {
                    HttpContext = new DefaultHttpContext
                    {
                        Request = { Scheme = "http", Host = new HostString("localhost:5001") }
                    }
                };
            }

            return controller;
        }

        private string GenerateUniqueUsername(string prefix = "testuser")
        {
            return $"{prefix}_{Guid.NewGuid().ToString().Substring(0, 8)}";
        }

        /// Demonstrates JWT token generation - UNIQUE to UserController
        /// Tests that Login endpoint generates valid JWT token with proper format
        [Fact]
        public async Task Login_WithValidCredentials_ReturnsJWTToken()
        {
            // Arrange
            var controller = CreateController();
            var uniqueUsername = GenerateUniqueUsername();
            var password = "SecurePassword123!";
            
            var dataService = new UserDataService(new Hashing());
            dataService.RegisterUser(uniqueUsername, password);

            try
            {
                var request = new UserCredentialsModel 
                { 
                    Username = uniqueUsername, 
                    Password = password 
                };

                // Act
                var result = await controller.Login(request);

                // Assert
                var okResult = Assert.IsType<OkObjectResult>(result);
                var response = Assert.IsType<UserLoginResponseModel>(okResult.Value);
                Assert.NotNull(response.Token);
                
                // Verify it's a valid JWT format (3 parts: header.payload.signature)
                var tokenParts = response.Token.Split('.');
                Assert.Equal(3, tokenParts.Length);
            }
            finally
            {
                dataService.DeleteUser(uniqueUsername);
            }
        }

        /// Demonstrates authentication failure handling
        [Fact]
        public async Task Login_WithInvalidCredentials_ReturnsBadRequest()
        {
            // Arrange
            var controller = CreateController();
            var request = new UserCredentialsModel 
            { 
                Username = "nonexistent_" + Guid.NewGuid().ToString().Substring(0, 8), 
                Password = "WrongPassword!" 
            };

            // Act
            var result = await controller.Login(request);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var errorResponse = Assert.IsType<ErrorResponseDto>(badRequestResult.Value);
            Assert.Equal("Invalid username or password", errorResponse.Error);
        }

        /// Demonstrates password hashing security - UNIQUE to UserController
        /// Verifies passwords are hashed with salt, not stored in plain text
        [Fact]
        public async Task CreateUser_HashesPasswordWithSalt()
        {
            // Arrange
            var controller = CreateController();
            var uniqueUsername = GenerateUniqueUsername();
            var plainPassword = "MySecretPassword123!";
            var request = new UserCredentialsModel 
            { 
                Username = uniqueUsername, 
                Password = plainPassword 
            };

            try
            {
                // Act
                await controller.CreateUser(request);

                // Assert - Verify password is hashed in database
                var dataService = new UserDataService(new Hashing());
                var user = dataService.GetUserByUsername(uniqueUsername);
                
                Assert.NotNull(user);
                Assert.NotEqual(plainPassword, user.HashedPassword); // NOT plain text
                Assert.NotNull(user.Salt);
                Assert.NotEmpty(user.Salt);
                
                // Verify hash can be verified with original password
                var hashing = new Hashing();
                Assert.True(hashing.Verify(plainPassword, user.HashedPassword, user.Salt));
            }
            finally
            {
                new UserDataService(new Hashing()).DeleteUser(uniqueUsername);
            }
        }

        /// Demonstrates duplicate user detection
        [Fact]
        public async Task CreateUser_WithDuplicateUsername_ReturnsConflict()
        {
            // Arrange
            var controller = CreateController();
            var uniqueUsername = GenerateUniqueUsername();
            var dataService = new UserDataService(new Hashing());
            dataService.RegisterUser(uniqueUsername, "Password123!");

            try
            {
                var request = new UserCredentialsModel 
                { 
                    Username = uniqueUsername, 
                    Password = "DifferentPassword!" 
                };

                // Act
                var result = await controller.CreateUser(request);

                // Assert
                var conflictResult = Assert.IsType<ConflictObjectResult>(result);
                Assert.Equal(409, conflictResult.StatusCode);
                
                var errorResponse = Assert.IsType<ErrorResponseDto>(conflictResult.Value);
                Assert.Equal("Username already exists.", errorResponse.Error);
            }
            finally
            {
                dataService.DeleteUser(uniqueUsername);
            }
        }

        /// Demonstrates authenticated user account deletion
        [Fact]
        public async Task DeleteOwnAccount_AuthenticatedUser_DeletesSuccessfully()
        {
            // Arrange
            var uniqueUsername = GenerateUniqueUsername();
            var dataService = new UserDataService(new Hashing());
            dataService.RegisterUser(uniqueUsername, "Password123!");
            
            var controller = CreateController(uniqueUsername); // Authenticate as this user

            // Act
            var result = await controller.DeleteOwnAccount();

            // Assert
            var okResult = Assert.IsType<OkResult>(result);
            
            // Verify user was deleted from database
            var deletedUser = dataService.GetUserByUsername(uniqueUsername);
            Assert.Null(deletedUser);
        }
    }
}
