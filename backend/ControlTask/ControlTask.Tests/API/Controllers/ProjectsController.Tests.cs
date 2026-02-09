using ControlTask.API.Controllers;
using ControlTask.Application.DTOs;
using ControlTask.Application.Interfaces;
using ControlTask.Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace ControlTask.API.Tests
{
    public class ProjectsControllerTests
    {
        private readonly Mock<IProjectService> _mockProjectService;
        private readonly ProjectsController _controller;

        public ProjectsControllerTests()
        {
            _mockProjectService = new Mock<IProjectService>();
            _controller = new ProjectsController(_mockProjectService.Object);
        }

        #region GetProjects

        [Fact]
        public async Task GetProjects_ReturnsOkResultWithProjects()
        {
            var projects = new List<ProjectDto>
            {
                new ProjectDto { ProjectId = 1, Name = "Project Alpha", ClientName = "Client A" },
                new ProjectDto { ProjectId = 2, Name = "Project Beta", ClientName = "Client B" }
            };
            _mockProjectService.Setup(s => s.GetProjectsWithStatsAsync()).ReturnsAsync(projects);

            var result = await _controller.GetProjects();

            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<List<ProjectDto>>(okResult.Value);

            Assert.Equal(2, returnValue.Count);
        }

        [Fact]
        public async Task GetProjects_ServiceException_Returns500()
        {
            _mockProjectService.Setup(s => s.GetProjectsWithStatsAsync()).ThrowsAsync(new Exception("DB error"));

            var result = await _controller.GetProjects();

            var objResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, objResult.StatusCode);
            Assert.Contains("DB error", objResult.Value.ToString());
        }

        #endregion

        #region GetProject

        [Fact]
        public async Task GetProject_ExistingId_ReturnsOk()
        {
            var projectId = 1;
            var project = new ProjectDto { ProjectId = projectId, Name = "Project A" };
            _mockProjectService.Setup(s => s.GetProjectByIdAsync(projectId)).ReturnsAsync(project);

            var result = await _controller.GetProject(projectId);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<ProjectDto>(okResult.Value);
            Assert.Equal("Project A", returnValue.Name);
        }

        [Fact]
        public async Task GetProject_NonExistingId_ReturnsNotFound()
        {
            var projectId = 999;
            _mockProjectService.Setup(s => s.GetProjectByIdAsync(projectId)).ReturnsAsync((ProjectDto)null);

            var result = await _controller.GetProject(projectId);

            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Contains(projectId.ToString(), notFoundResult.Value.ToString());
        }

        [Fact]
        public async Task GetProject_ServiceException_Returns500()
        {
            _mockProjectService.Setup(s => s.GetProjectByIdAsync(1)).ThrowsAsync(new Exception("DB failure"));

            var result = await _controller.GetProject(1);

            var objResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, objResult.StatusCode);
        }

        #endregion

        #region GetAllProjectTasks

        [Fact]
        public async Task GetAllProjectTasks_ReturnsOk()
        {
            var projectId = 1;
            var tasks = new List<TaskDto>
            {
                new TaskDto { TaskId = 1, Title = "Task 1" },
                new TaskDto { TaskId = 2, Title = "Task 2" }
            };
            _mockProjectService.Setup(s => s.GetProjectTasksAsync(projectId, null, null)).ReturnsAsync(tasks);

            var result = await _controller.GetAllProjectTasks(projectId);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<List<TaskDto>>(okResult.Value);
            Assert.Equal(2, returnValue.Count);
        }

        [Fact]
        public async Task GetAllProjectTasks_NotFound_Returns404()
        {
            var projectId = 999;
            _mockProjectService.Setup(s => s.GetProjectTasksAsync(projectId, null, null))
                .ThrowsAsync(new KeyNotFoundException("Project not found"));

            var result = await _controller.GetAllProjectTasks(projectId);

            var notFound = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Contains("Project not found", notFound.Value.ToString());
        }

        [Fact]
        public async Task GetAllProjectTasks_ServiceException_Returns500()
        {
            _mockProjectService.Setup(s => s.GetProjectTasksAsync(1, null, null))
                .ThrowsAsync(new Exception("DB error"));

            var result = await _controller.GetAllProjectTasks(1);

            var objResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, objResult.StatusCode);
        }

        #endregion

        #region GetProjectHealth

        [Fact]
        public async Task GetProjectHealth_ReturnsOk()
        {
            var health = new List<ProjectHealthDto>
            {
                new ProjectHealthDto { ProjectId = 1, ProjectName = "Project A" }
            };
            _mockProjectService.Setup(s => s.GetProjectHealthAsync()).ReturnsAsync(health);

            var result = await _controller.GetProjectHealth();

            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<List<ProjectHealthDto>>(okResult.Value);
            Assert.Single(returnValue);
            Assert.Equal("Project A", returnValue[0].ProjectName);
        }

        [Fact]
        public async Task GetProjectHealth_ServiceException_Returns500()
        {
            _mockProjectService.Setup(s => s.GetProjectHealthAsync()).ThrowsAsync(new Exception("DB error"));

            var result = await _controller.GetProjectHealth();

            var objResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, objResult.StatusCode);
        }

        #endregion

        [Fact]
        public async Task GetProjectTasks_InvalidPage_ReturnsBadRequest()
        {
            var result = await _controller.GetProjectTasks(1, page: 0);
            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Contains("mayor a 0", badRequest.Value.ToString());
        }

        [Fact]
        public async Task GetProjectTasks_InvalidPageSize_ReturnsBadRequest()
        {
            var result = await _controller.GetProjectTasks(1, pageSize: 101);
            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Contains("entre 1 y 100", badRequest.Value.ToString());
        }

        [Fact]
        public async Task GetProjectTasks_InvalidStatus_ReturnsBadRequest()
        {
            var result = await _controller.GetProjectTasks(1, status: "UnknownStatus");
            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Contains("Status debe ser", badRequest.Value.ToString());
        }

        [Fact]
        public async Task GetAllProjectTasks_WithFilters_CallsServiceCorrectly()
        {
            var tasks = new List<TaskDto> { new TaskDto { TaskId = 1 } };
            _mockProjectService
                .Setup(s => s.GetProjectTasksAsync(1, "ToDo", 2))
                .ReturnsAsync(tasks);

            var result = await _controller.GetAllProjectTasks(1, "ToDo", 2);
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<List<TaskDto>>(okResult.Value);

            Assert.Single(returnValue);
        }

    }
}
