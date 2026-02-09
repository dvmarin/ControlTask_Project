using Xunit;
using Moq;
using Microsoft.AspNetCore.Mvc;
using ControlTask.API.Controllers;
using ControlTask.Application.DTOs;
using ControlTask.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ControlTask.API.Tests
{
    public class DashboardControllerTests
    {
        private readonly Mock<IDashboardService> _mockDashboardService;
        private readonly DashboardController _controller;

        public DashboardControllerTests()
        {
            _mockDashboardService = new Mock<IDashboardService>();
            _controller = new DashboardController(_mockDashboardService.Object);
        }

        [Fact]
        public async Task GetDeveloperWorkload_ReturnsOkResult()
        {
            // Arrange
            var workload = new List<DeveloperWorkloadDto>
            {
                new DeveloperWorkloadDto { DeveloperName = "John Doe", OpenTasksCount = 5, AverageEstimatedComplexity = 3.5m }
            };

            _mockDashboardService.Setup(s => s.GetDeveloperWorkloadAsync())
                .ReturnsAsync(workload);

            // Act
            var result = await _controller.GetDeveloperWorkload();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.IsType<List<DeveloperWorkloadDto>>(okResult.Value);
        }

        [Fact]
        public async Task GetDeveloperDelayRisk_ReturnsOkResult()
        {
            // Arrange
            var delayRisk = new List<DeveloperDelayRiskDto>
            {
                new DeveloperDelayRiskDto { DeveloperName = "John Doe", OpenTasksCount = 5, HighRiskFlag = true }
            };

            _mockDashboardService.Setup(s => s.GetDeveloperDelayRiskAsync())
                .ReturnsAsync(delayRisk);

            // Act
            var result = await _controller.GetDeveloperDelayRisk();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.IsType<List<DeveloperDelayRiskDto>>(okResult.Value);
        }

        [Fact]
        public async Task GetUpcomingTasks_ValidDays_ReturnsOkResult()
        {
            // Arrange
            var upcomingTasks = new List<UpcomingTaskDto>
            {
                new UpcomingTaskDto { Title = "Task 1", DaysUntilDue = 3 }
            };

            _mockDashboardService.Setup(s => s.GetUpcomingTasksAsync(7))
                .ReturnsAsync(upcomingTasks);

            // Act
            var result = await _controller.GetUpcomingTasks(days: 7);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.IsType<List<UpcomingTaskDto>>(okResult.Value);
        }

        [Fact]
        public async Task GetUpcomingTasks_InvalidDays_ReturnsBadRequest()
        {
            // Act
            var result = await _controller.GetUpcomingTasks(days: 0);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task GetProjectHealth_ReturnsOkResult()
        {
            // Arrange
            var projectHealth = new List<ProjectHealthDto>
            {
                new ProjectHealthDto { ProjectName = "Project A", OpenTasks = 5, CompletedTasks = 10 }
            };

            _mockDashboardService.Setup(s => s.GetProjectHealthAsync())
                .ReturnsAsync(projectHealth);

            // Act
            var result = await _controller.GetProjectHealth();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.IsType<List<ProjectHealthDto>>(okResult.Value);
        }
    }
}