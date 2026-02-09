using AutoMapper;
using ControlTask.Application.DTOs;
using ControlTask.Application.Services;
using ControlTask.Domain.Entities;
using ControlTask.Domain.Interfaces;
using Moq;

namespace ControlTask.Application.Tests
{
    public class DeveloperServiceTests
    {
        private readonly Mock<IDeveloperRepository> _mockDeveloperRepository;
        private readonly Mock<IMapper> _mockMapper;
        private readonly DeveloperService _developerService;

        public DeveloperServiceTests()
        {
            _mockDeveloperRepository = new Mock<IDeveloperRepository>();
            _mockMapper = new Mock<IMapper>();
            _developerService = new DeveloperService(
                _mockDeveloperRepository.Object,
                _mockMapper.Object
            );
        }

        [Fact]
        public async Task GetActiveDevelopersAsync_ReturnsActiveDevelopers()
        {
            // Arrange
            var developers = new List<Developer>
            {
                new Developer { DeveloperId = 1, FirstName = "John", LastName = "Doe", IsActive = true },
                new Developer { DeveloperId = 2, FirstName = "Jane", LastName = "Smith", IsActive = true }
            };

            var developerDtos = new List<DeveloperDto>
            {
                new DeveloperDto { DeveloperId = 1, FullName = "John Doe" },
                new DeveloperDto { DeveloperId = 2, FullName = "Jane Smith" }
            };

            _mockDeveloperRepository.Setup(r => r.GetActiveAsync())
                .ReturnsAsync(developers);
            _mockMapper.Setup(m => m.Map<IEnumerable<DeveloperDto>>(developers))
                .Returns(developerDtos);

            // Act
            var result = await _developerService.GetActiveDevelopersAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
            Assert.Contains(result, d => d.FullName == "John Doe");
        }

        [Fact]
        public async Task GetDeveloperByIdAsync_ActiveDeveloper_ReturnsDeveloperDto()
        {
            // Arrange
            var developerId = 1;
            var developer = new Developer
            {
                DeveloperId = developerId,
                FirstName = "John",
                LastName = "Doe",
                IsActive = true
            };

            var developerDto = new DeveloperDto
            {
                DeveloperId = developerId,
                FullName = "John Doe"
            };

            _mockDeveloperRepository.Setup(r => r.GetByIdAsync(developerId))
                .ReturnsAsync(developer);
            _mockMapper.Setup(m => m.Map<DeveloperDto>(developer))
                .Returns(developerDto);

            // Act
            var result = await _developerService.GetDeveloperByIdAsync(developerId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(developerId, result.DeveloperId);
            Assert.Equal("John Doe", result.FullName);
        }

        [Fact]
        public async Task GetDeveloperByIdAsync_InactiveDeveloper_ReturnsNull()
        {
            // Arrange
            var developerId = 1;
            var developer = new Developer
            {
                DeveloperId = developerId,
                IsActive = false
            };

            _mockDeveloperRepository.Setup(r => r.GetByIdAsync(developerId))
                .ReturnsAsync(developer);

            // Act
            var result = await _developerService.GetDeveloperByIdAsync(developerId);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GetDeveloperWorkloadAsync_CalculatesWorkloadCorrectly()
        {
            // Arrange
            var developers = new List<Developer>
            {
                new Developer
                {
                    DeveloperId = 1,
                    FirstName = "John",
                    LastName = "Doe",
                    Tasks = new List<TaskItem>
                    {
                        new TaskItem { Status = "ToDo", EstimatedComplexity = 3 },
                        new TaskItem { Status = "InProgress", EstimatedComplexity = 5 },
                        new TaskItem { Status = "Completed", EstimatedComplexity = 2 }
                    }
                }
            };

            _mockDeveloperRepository.Setup(r => r.GetDevelopersWithTasksAsync())
                .ReturnsAsync(developers);

            // Act
            var result = await _developerService.GetDeveloperWorkloadAsync();

            // Assert
            Assert.NotNull(result);
            var workload = result.First();
            Assert.Equal("John Doe", workload.DeveloperName);
            Assert.Equal(2, workload.OpenTasksCount); // 2 tareas no completadas
            Assert.Equal(4, workload.AverageEstimatedComplexity); // (3 + 5) / 2 = 4
        }

        [Fact]
        public async Task GetDeveloperWorkloadAsync_NoOpenTasks_ReturnsZeroComplexity()
        {
            // Arrange
            var developers = new List<Developer>
            {
                new Developer
                {
                    DeveloperId = 1,
                    FirstName = "John",
                    LastName = "Doe",
                    Tasks = new List<TaskItem>
                    {
                        new TaskItem { Status = "Completed", EstimatedComplexity = 3 },
                        new TaskItem { Status = "Completed", EstimatedComplexity = 5 }
                    }
                }
            };

            _mockDeveloperRepository.Setup(r => r.GetDevelopersWithTasksAsync())
                .ReturnsAsync(developers);

            // Act
            var result = await _developerService.GetDeveloperWorkloadAsync();

            // Assert
            Assert.NotNull(result);
            var workload = result.First();
            Assert.Equal(0, workload.OpenTasksCount);
            Assert.Equal(0, workload.AverageEstimatedComplexity);
        }
    }
}