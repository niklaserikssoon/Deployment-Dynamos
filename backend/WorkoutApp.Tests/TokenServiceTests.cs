using Microsoft.Extensions.Configuration;
using Moq;
using User_API.Models;
using User_API.Service;
using Xunit;

namespace WorkoutApp.Tests
{
    public class TokenServiceTests
    {
        private readonly TokenService _tokenService;

        public TokenServiceTests()
        {
            var mockConfig = new Mock<IConfiguration>();
            mockConfig.Setup(c => c["Jwt:Key"])
                .Returns("my-super-secret-jwt-key-for-workout-app-2026-very-long-and-random-123!");
            mockConfig.Setup(c => c["Jwt:Issuer"])
                .Returns("UserApi");
            mockConfig.Setup(c => c["Jwt:Audience"])
                .Returns("WorkoutAppClients");

            _tokenService = new TokenService(mockConfig.Object);
        }

        [Fact]
        public void CreateToken_ValidUser_ReturnsToken()
        {
            // Arrange
            var user = new User
            {
                UserId = 1,
                UserName = "testuser",
                Email = "test@test.com",
                FirstName = "Test",
                LastName = "User",
                PasswordHash = "hashedpassword"
            };

            // Act
            var token = _tokenService.CreateToken(user);

            // Assert
            Assert.NotNull(token);
            Assert.NotEmpty(token);
        }

        [Fact]
        public void CreateToken_ValidUser_TokenContainsThreeParts()
        {
            // Arrange
            var user = new User
            {
                UserId = 1,
                UserName = "testuser",
                Email = "test@test.com",
                FirstName = "Test",
                LastName = "User",
                PasswordHash = "hashedpassword"
            };

            // Act
            var token = _tokenService.CreateToken(user);
            var parts = token.Split('.');

            // Assert
            Assert.Equal(3, parts.Length);
        }
    }
}
