using Xunit;
using Moq;
using ControlTask.Application.Services;
using ControlTask.Application.Interfaces;
using ControlTask.Application.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using System;

namespace ControlTask.Application.Tests
{
    public class DashboardServiceTests
    {
        private readonly Mock<IDashboardQuery> _mockQuery;
        private readonly DashboardService _service;

        public DashboardServiceTests()
        {
            _mockQuery = new Mock<IDashboardQuery>();
            _service = new DashboardService(_mockQuery.Object);
        }

        [Fact]
        public async Task GetDeveloperWorkloadAsync_ReturnsCorrectData()
        {
            // Arrange
            var workload = new List<DeveloperWorkloadDto>
            {
                new DeveloperWorkloadDto { DeveloperName = "John Doe", OpenTasksCount = 2, AverageEstimatedComplexity = 4 },
                new DeveloperWorkloadDto { DeveloperName = "Jane Smith", OpenTasksCount = 1, AverageEstimatedComplexity = 3 }
            };

            _mockQuery.Setup(q => q.GetDeveloperWorkloadAsync())
                      .ReturnsAsync(workload);

            // Act
            var result = await _service.GetDeveloperWorkloadAsync();

            // Assert
            var list = Assert.IsAssignableFrom<IEnumerable<DeveloperWorkloadDto>>(result);
            Assert.Equal(2, list.Count());
            Assert.Contains(list, d => d.DeveloperName == "John Doe" && d.OpenTasksCount == 2);
            Assert.Contains(list, d => d.DeveloperName == "Jane Smith" && d.AverageEstimatedComplexity == 3);
        }

        [Fact]
        public async Task GetDeveloperDelayRiskAsync_ReturnsCorrectData()
        {
            // Arrange
            var riskList = new List<DeveloperDelayRiskDto>
            {
                new DeveloperDelayRiskDto
                {
                    DeveloperName = "John Doe", OpenTasksCount = 2, AvgDelayDays = 5,
                    HighRiskFlag = true, NearestDueDate = DateTime.UtcNow, LatestDueDate = DateTime.UtcNow.AddDays(3)
                },
                new DeveloperDelayRiskDto
                {
                    DeveloperName = "Jane Smith", OpenTasksCount = 1, AvgDelayDays = 0,
                    HighRiskFlag = false
                }
            };

            _mockQuery.Setup(q => q.GetDeveloperDelayRiskAsync())
                      .ReturnsAsync(riskList);

            // Act
            var result = await _service.GetDeveloperDelayRiskAsync();

            // Assert
            var list = Assert.IsAssignableFrom<IEnumerable<DeveloperDelayRiskDto>>(result);
            Assert.Equal(2, list.Count());
            Assert.Contains(list, d => d.DeveloperName == "John Doe" && d.HighRiskFlag);
            Assert.Contains(list, d => d.DeveloperName == "Jane Smith" && !d.HighRiskFlag);
        }

        [Fact]
        public async Task GetProjectHealthAsync_ReturnsCorrectData()
        {
            // Arrange
            var health = new List<ProjectHealthDto>
            {
                new ProjectHealthDto { ProjectId = 1, ProjectName = "Project A", ClientName = "Client X", TotalTasks = 10, OpenTasks = 3, CompletedTasks = 7 },
                new ProjectHealthDto { ProjectId = 2, ProjectName = "Project B", ClientName = "Client Y", TotalTasks = 5, OpenTasks = 2, CompletedTasks = 3 }
            };

            _mockQuery.Setup(q => q.GetProjectHealthAsync())
                      .ReturnsAsync(health);

            // Act
            var result = await _service.GetProjectHealthAsync();

            // Assert
            var list = Assert.IsAssignableFrom<IEnumerable<ProjectHealthDto>>(result);
            Assert.Equal(2, list.Count());
            Assert.Contains(list, p => p.ProjectName == "Project A" && p.OpenTasks == 3);
            Assert.Contains(list, p => p.ProjectName == "Project B" && p.CompletedTasks == 3);
        }

        [Fact]
        public async Task GetUpcomingTasksAsync_ReturnsCorrectData()
        {
            // Arrange
            var tasks = new List<UpcomingTaskDto>
            {
                new UpcomingTaskDto { Title = "Task 1", DaysUntilDue = 2 },
                new UpcomingTaskDto { Title = "Task 2", DaysUntilDue = 5 }
            };

            _mockQuery.Setup(q => q.GetUpcomingTasksAsync(7))
                      .ReturnsAsync(tasks);

            // Act
            var result = await _service.GetUpcomingTasksAsync(7);

            // Assert
            var list = Assert.IsAssignableFrom<IEnumerable<UpcomingTaskDto>>(result);
            Assert.Equal(2, list.Count());
            Assert.Contains(list, t => t.Title == "Task 1" && t.DaysUntilDue == 2);
            Assert.Contains(list, t => t.Title == "Task 2" && t.DaysUntilDue == 5);
        }
    }
}
