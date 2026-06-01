using Microsoft.EntityFrameworkCore;
using Moq;
using WorkoutApp.API.Data;
using WorkoutApp.API.Models;
using workoutapp_API.services.Exercises;
using workoutapp_API.DTOs;
using workoutapp_API.services.External;

namespace WorkoutApp.Tests
{
    public class SaveExerciseServiceTests
    {
        private WorkoutDbContext CreateInMemoryContext()
        {
            var options = new DbContextOptionsBuilder<WorkoutDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            return new WorkoutDbContext(options);
        }

        [Fact]
        public async Task SaveExerciseAsync_NewExercise_SavesAndReturnsExercise()
        {
            // Arrange
            using var context = CreateInMemoryContext();

            var mockExternalService = new Mock<IExternalExercise>();
            mockExternalService.Setup(s => s.GetExercisesAsync())
                .ReturnsAsync(new List<ExternalExerciseDTO>
                {
                    new ExternalExerciseDTO
                    {
                        Id = "Bench_Press",
                        Name = "Bench Press",
                        PrimaryMuscles = new List<string> { "chest" }
                    }
                });

            var service = new SaveExerciseService(context, mockExternalService.Object);

            // Act
            var result = await service.SaveExerciseAsync("Bench_Press");

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Bench Press", result.ExerciseName);
            Assert.Equal("chest", result.PrimaryMuscle);
        }

        [Fact]
        public async Task SaveExerciseAsync_ExistingExercise_ReturnsExistingWithoutDuplicate()
        {
            // Arrange
            using var context = CreateInMemoryContext();
            context.Exercises.Add(new Exercise
            {
                ExerciseId = 1,
                ExternalExerciseId = "Bench_Press",
                ExerciseName = "Bench Press",
                PrimaryMuscle = "chest"
            });
            await context.SaveChangesAsync();

            var mockExternalService = new Mock<IExternalExercise>();
            var service = new SaveExerciseService(context, mockExternalService.Object);

            // Act
            var result = await service.SaveExerciseAsync("Bench_Press");

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Bench_Press", result.ExternalExerciseId);
            // Verify external API was never called since exercise already exists
            mockExternalService.Verify(s => s.GetExercisesAsync(), Times.Never);
        }

        [Fact]
        public async Task SaveExerciseAsync_NonExistentExternalId_ReturnsNull()
        {
            // Arrange
            using var context = CreateInMemoryContext();

            var mockExternalService = new Mock<IExternalExercise>();
            mockExternalService.Setup(s => s.GetExercisesAsync())
                .ReturnsAsync(new List<ExternalExerciseDTO>());

            var service = new SaveExerciseService(context, mockExternalService.Object);

            // Act
            var result = await service.SaveExerciseAsync("NonExistent_Exercise");

            // Assert
            Assert.Null(result);
        }
    }
}