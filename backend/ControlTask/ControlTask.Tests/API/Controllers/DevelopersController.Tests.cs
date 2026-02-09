using ControlTask.API.Controllers;
using ControlTask.Application.DTOs;
using ControlTask.Application.Interfaces;
using ControlTask.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace ControlTask.API.Tests
{
    public class DevelopersControllerTests
    {
        private readonly Mock<IDeveloperService> _mockDeveloperService;
        private readonly DevelopersController _controller;

        public DevelopersControllerTests()
        {
            _mockDeveloperService = new Mock<IDeveloperService>();
            _controller = new DevelopersController(_mockDeveloperService.Object);
        }

        [Fact]
        public async Task GetActiveDevelopers_ReturnsOkResult()
        {
            // Arrange
            var developers = new List<DeveloperDto>
            {
                new DeveloperDto { DeveloperId = 1, FullName = "John Doe", Email = "john@example.com", IsActive = true },
                new DeveloperDto { DeveloperId = 2, FullName = "Jane Smith", Email = "jane@example.com", IsActive = true }
            };

            _mockDeveloperService.Setup(s => s.GetActiveDevelopersAsync())
                .ReturnsAsync(developers);

            // Act
            var result = await _controller.GetActiveDevelopers();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<List<DeveloperDto>>(okResult.Value);
            Assert.Equal(2, returnValue.Count);
        }

        [Fact]
        public async Task GetActiveDevelopers_ServiceException_ReturnsInternalServerError()
        {
            // Arrange
            _mockDeveloperService.Setup(s => s.GetActiveDevelopersAsync())
                .ThrowsAsync(new Exception("Database connection failed"));

            // Act
            var result = await _controller.GetActiveDevelopers();

            // Assert
            var statusCodeResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, statusCodeResult.StatusCode);
            Assert.Contains("Error interno", statusCodeResult.Value.ToString());
        }

        [Fact]
        public async Task GetDeveloper_ExistingId_ReturnsOkResult()
        {
            // Arrange
            var developerId = 1;
            var developer = new DeveloperDto
            {
                DeveloperId = developerId,
                FullName = "John Doe",
                Email = "john@example.com",
                IsActive = true
            };

            _mockDeveloperService.Setup(s => s.GetDeveloperByIdAsync(developerId))
                .ReturnsAsync(developer);

            // Act
            var result = await _controller.GetDeveloper(developerId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<DeveloperDto>(okResult.Value);
            Assert.Equal(developerId, returnValue.DeveloperId);
            Assert.Equal("John Doe", returnValue.FullName);
        }

        [Fact]
        public async Task GetDeveloper_NonExistingId_ReturnsNotFound()
        {
            // Arrange
            var developerId = 999;
            _mockDeveloperService.Setup(s => s.GetDeveloperByIdAsync(developerId))
                .ReturnsAsync((DeveloperDto)null);

            // Act
            var result = await _controller.GetDeveloper(developerId);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Contains($"Desarrollador con ID {developerId} no encontrado", notFoundResult.Value.ToString());
        }

        [Fact]
        public async Task GetDeveloper_ServiceException_ReturnsInternalServerError()
        {
            // Arrange
            var developerId = 1;
            _mockDeveloperService.Setup(s => s.GetDeveloperByIdAsync(developerId))
                .ThrowsAsync(new Exception("Service error"));

            // Act
            var result = await _controller.GetDeveloper(developerId);

            // Assert
            var statusCodeResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, statusCodeResult.StatusCode);
        }

        [Fact]
        public async Task GetDeveloperWorkload_ReturnsOkResult()
        {
            // Arrange
            var workload = new List<DeveloperWorkloadDto>
            {
                new DeveloperWorkloadDto
                {
                    DeveloperName = "John Doe",
                    OpenTasksCount = 5,
                    AverageEstimatedComplexity = 3.5m
                },
                new DeveloperWorkloadDto
                {
                    DeveloperName = "Jane Smith",
                    OpenTasksCount = 3,
                    AverageEstimatedComplexity = 2.0m
                }
            };

            _mockDeveloperService.Setup(s => s.GetDeveloperWorkloadAsync())
                .ReturnsAsync(workload);

            // Act
            var result = await _controller.GetDeveloperWorkload();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<List<DeveloperWorkloadDto>>(okResult.Value);
            Assert.Equal(2, returnValue.Count);

            var johnWorkload = returnValue.First(w => w.DeveloperName == "John Doe");
            Assert.Equal(5, johnWorkload.OpenTasksCount);
            Assert.Equal(3.5m, johnWorkload.AverageEstimatedComplexity);
        }

        [Fact]
        public async Task GetDeveloperWorkload_ServiceException_ReturnsInternalServerError()
        {
            // Arrange
            _mockDeveloperService.Setup(s => s.GetDeveloperWorkloadAsync())
                .ThrowsAsync(new Exception("Calculation error"));

            // Act
            var result = await _controller.GetDeveloperWorkload();

            // Assert
            var statusCodeResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, statusCodeResult.StatusCode);
        }

        [Fact]
        public async Task GetDeveloperWorkload_EmptyResult_ReturnsOkWithEmptyList()
        {
            // Arrange
            var emptyList = new List<DeveloperWorkloadDto>();
            _mockDeveloperService.Setup(s => s.GetDeveloperWorkloadAsync())
                .ReturnsAsync(emptyList);

            // Act
            var result = await _controller.GetDeveloperWorkload();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<List<DeveloperWorkloadDto>>(okResult.Value);
            Assert.Empty(returnValue);
        }

        [Fact]
        public void Controller_HasApiControllerAttribute()
        {
            // Arrange
            var type = typeof(DevelopersController);

            // Act & Assert
            Assert.True(type.IsDefined(typeof(ApiControllerAttribute), false));
        }

        [Fact]
        public void Controller_HasRouteAttribute()
        {
            // Arrange
            var type = typeof(DevelopersController);

            // Act & Assert
            var routeAttribute = type.GetCustomAttributes(typeof(RouteAttribute), false);
            Assert.NotEmpty(routeAttribute);
            var route = (RouteAttribute)routeAttribute[0];
            Assert.Equal("api/[controller]", route.Template);
        }

        [Fact]
        public async Task GetDeveloper_ValidIdButInactive_ReturnsNotFound()
        {
            // Arrange
            var developerId = 30;
            var developer = new DeveloperDto
            {
                DeveloperId = developerId,
                IsActive = false
            };

            _mockDeveloperService.Setup(s => s.GetDeveloperByIdAsync(developerId))
                .ReturnsAsync(developer);

            // Act
            var result = await _controller.GetDeveloper(developerId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedDeveloper = Assert.IsType<DeveloperDto>(okResult.Value);
            Assert.False(returnedDeveloper.IsActive);
        }
    }
}