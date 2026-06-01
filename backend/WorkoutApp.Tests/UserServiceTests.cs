using Moq;
using User_API.DTOs;
using User_API.Models;
using User_API.Repositories;
using User_API.Service;

namespace WorkoutApp.Tests
{
    public class UserServiceTests
    {
        private Mock<IUserRepository> CreateMockRepository()
        {
            return new Mock<IUserRepository>();
        }

        [Fact]
        public async Task CreateUserAsync_ValidDto_ReturnsUser()
        {
            // Arrange
            var mockRepo = CreateMockRepository();
            var service = new UserService(mockRepo.Object);
            var dto = new CreateUserDTO
            {
                FirstName = "Jordan",
                LastName = "Foose",
                UserName = "jfoose",
                Email = "jordan@test.com",
                Password = "Test.1234"
            };

            mockRepo.Setup(r => r.AddUserAsync(It.IsAny<User>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await service.CreateUserAsync(dto);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("jfoose", result.UserName);
            Assert.Equal("Jordan", result.FirstName);
        }

        [Fact]
        public async Task GetUserByIdAsync_ExistingUser_ReturnsUser()
        {
            // Arrange
            var mockRepo = CreateMockRepository();
            var user = new User
            {
                UserId = 1,
                FirstName = "Jordan",
                LastName = "Foose",
                UserName = "jfoose",
                Email = "jordan@test.com",
                PasswordHash = "hashedpassword"
            };

            mockRepo.Setup(r => r.GetUserByIdAsync(1))
                .ReturnsAsync(user);

            var service = new UserService(mockRepo.Object);

            // Act
            var result = await service.GetUserByIdAsync(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.UserId);
        }

        [Fact]
        public async Task GetUserByUsernameAsync_NonExistentUser_ReturnsNull()
        {
            // Arrange
            var mockRepo = CreateMockRepository();
            mockRepo.Setup(r => r.GetUserByUsernameAsync("nobody"))
                .ReturnsAsync((User?)null);

            var service = new UserService(mockRepo.Object);

            // Act
            var result = await service.GetUserByUsernameAsync("nobody");

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task DeleteUserAsync_ExistingUser_ReturnsTrue()
        {
            // Arrange
            var mockRepo = CreateMockRepository();
            var user = new User
            {
                UserId = 1,
                FirstName = "Jordan",
                LastName = "Foose",
                UserName = "jfoose",
                Email = "jordan@test.com",
                PasswordHash = "hashedpassword"
            };

            mockRepo.Setup(r => r.GetUserByIdAsync(1))
                .ReturnsAsync(user);
            mockRepo.Setup(r => r.DeleteUserAsync(user))
                .Returns(Task.CompletedTask);

            var service = new UserService(mockRepo.Object);

            // Act
            var result = await service.DeleteUserAsync(1);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task DeleteUserAsync_NonExistentUser_ReturnsFalse()
        {
            // Arrange
            var mockRepo = CreateMockRepository();
            mockRepo.Setup(r => r.GetUserByIdAsync(999))
                .ReturnsAsync((User?)null);

            var service = new UserService(mockRepo.Object);

            // Act
            var result = await service.DeleteUserAsync(999);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task UpdateUserAsync_ExistingUser_UpdatesAndReturnsUser()
        {
            // Arrange
            var mockRepo = CreateMockRepository();
            var user = new User
            {
                UserId = 1,
                FirstName = "Jordan",
                LastName = "Foose",
                UserName = "jfoose",
                Email = "jordan@test.com",
                PasswordHash = "hashedpassword"
            };

            mockRepo.Setup(r => r.GetUserByIdAsync(1))
                .ReturnsAsync(user);
            mockRepo.Setup(r => r.UpdateUserAsync(It.IsAny<User>()))
                .Returns(Task.CompletedTask);

            var service = new UserService(mockRepo.Object);
            var updateDto = new UpdateUserDTO
            {
                FirstName = "Jordan Updated",
                LastName = "Foose Updated",
                Email = "updated@test.com"
            };

            // Act
            var result = await service.UpdateUserAsync(1, updateDto);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Jordan Updated", result.FirstName);
            Assert.Equal("updated@test.com", result.Email);
        }
    }
}