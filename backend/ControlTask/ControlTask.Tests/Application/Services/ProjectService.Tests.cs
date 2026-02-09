using Xunit;
using Moq;
using AutoMapper;
using ControlTask.Application.DTOs;
using ControlTask.Application.Services;
using ControlTask.Domain.Entities;
using ControlTask.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ControlTask.Application.Tests
{
    public class ProjectServiceTests
    {
        private readonly Mock<IProjectRepository> _mockProjectRepository;
        private readonly Mock<ITaskRepository> _mockTaskRepository;
        private readonly Mock<IMapper> _mockMapper;
        private readonly ProjectService _projectService;

        public ProjectServiceTests()
        {
            _mockProjectRepository = new Mock<IProjectRepository>();
            _mockTaskRepository = new Mock<ITaskRepository>();
            _mockMapper = new Mock<IMapper>();
            _projectService = new ProjectService(
                _mockProjectRepository.Object,
                _mockTaskRepository.Object,
                _mockMapper.Object
            );
        }

        [Fact]
        public async Task GetProjectsWithStatsAsync_ReturnsProjectsWithStats()
        {
            // Arrange
            var projects = new List<Project>
            {
                new Project
                {
                    ProjectId = 1,
                    Name = "Project A",
                    ClientName = "Client A",
                    Tasks = new List<TaskItem>
                    {
                        new TaskItem { Status = "ToDo" },
                        new TaskItem { Status = "InProgress" },
                        new TaskItem { Status = "Completed" }
                    }
                }
            };

            _mockProjectRepository.Setup(r => r.GetProjectsWithTasksAsync())
                .ReturnsAsync(projects);

            // Act
            var result = await _projectService.GetProjectsWithStatsAsync();

            // Assert
            Assert.NotNull(result);
            var projectDto = result.First();
            Assert.Equal(1, projectDto.ProjectId);
            Assert.Equal("Project A", projectDto.Name);
            Assert.Equal(3, projectDto.TotalTasks);
            Assert.Equal(2, projectDto.OpenTasks);
            Assert.Equal(1, projectDto.CompletedTasks);
        }

        [Fact]
        public async Task GetProjectByIdAsync_ExistingProject_ReturnsProjectDto()
        {
            // Arrange
            var projectId = 1;
            var project = new Project
            {
                ProjectId = projectId,
                Name = "Project A",
                ClientName = "Client A",
                Tasks = new List<TaskItem>
                {
                    new TaskItem { Status = "ToDo" },
                    new TaskItem { Status = "Completed" }
                }
            };

            _mockProjectRepository.Setup(r => r.GetProjectWithTasksByIdAsync(projectId))
                .ReturnsAsync(project);

            // Act
            var result = await _projectService.GetProjectByIdAsync(projectId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(projectId, result.ProjectId);
            Assert.Equal("Project A", result.Name);
            Assert.Equal(2, result.TotalTasks);
            Assert.Equal(1, result.OpenTasks);
            Assert.Equal(1, result.CompletedTasks);
        }

        [Fact]
        public async Task GetProjectByIdAsync_NonExistingProject_ReturnsNull()
        {
            // Arrange
            var projectId = 999;
            _mockProjectRepository.Setup(r => r.GetProjectWithTasksByIdAsync(projectId))
                .ReturnsAsync((Project)null);

            // Act
            var result = await _projectService.GetProjectByIdAsync(projectId);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GetProjectHealthAsync_ReturnsHealthStats()
        {
            // Arrange
            var projects = new List<Project>
            {
                new Project
                {
                    ProjectId = 1,
                    Name = "Project A",
                    ClientName = "Client A",
                    Tasks = new List<TaskItem>
                    {
                        new TaskItem { Status = "ToDo" },
                        new TaskItem { Status = "Completed" }
                    }
                }
            };

            _mockProjectRepository.Setup(r => r.GetProjectsWithTasksAsync())
                .ReturnsAsync(projects);

            // Act
            var result = await _projectService.GetProjectHealthAsync();

            // Assert
            Assert.NotNull(result);
            var health = result.First();
            Assert.Equal(1, health.ProjectId);
            Assert.Equal("Project A", health.ProjectName);
            Assert.Equal("Client A", health.ClientName);
            Assert.Equal(2, health.TotalTasks);
            Assert.Equal(1, health.OpenTasks);
            Assert.Equal(1, health.CompletedTasks);
        }

        [Fact]
        public async Task GetProjectTasksPagedAsync_ValidProject_ReturnsPagedTasks()
        {
            // Arrange
            var projectId = 1;
            var page = 1;
            var pageSize = 10;

            var pagedResult = new Domain.Interfaces.PagedResult<TaskItem>
            {
                Items = new List<TaskItem>
                {
                    new TaskItem { TaskId = 1, Title = "Task 1" }
                },
                TotalCount = 1,
                PageNumber = page,
                PageSize = pageSize
            };

            var taskDtos = new List<TaskDto>
            {
                new TaskDto { TaskId = 1, Title = "Task 1" }
            };

            _mockProjectRepository.Setup(r => r.ExistsAsync(projectId))
                .ReturnsAsync(true);
            _mockTaskRepository.Setup(r => r.GetPagedTasksByProjectAsync(projectId, page, pageSize, null, null))
                .ReturnsAsync(pagedResult);
            _mockMapper.Setup(m => m.Map<List<TaskDto>>(pagedResult.Items))
                .Returns(taskDtos);

            // Act
            var result = await _projectService.GetProjectTasksPagedAsync(projectId, page, pageSize);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.TotalCount);
            Assert.Equal(page, result.PageNumber);
            Assert.Equal(pageSize, result.PageSize);
            Assert.Single(result.Items);
            Assert.Equal("Task 1", result.Items.First().Title);
        }

        [Fact]
        public async Task GetProjectTasksPagedAsync_NonExistingProject_ThrowsKeyNotFoundException()
        {
            // Arrange
            var projectId = 999;
            _mockProjectRepository.Setup(r => r.ExistsAsync(projectId))
                .ReturnsAsync(false);

            // Act & Assert
            await Assert.ThrowsAsync<KeyNotFoundException>(() =>
                _projectService.GetProjectTasksPagedAsync(projectId, 1, 10));
        }

        [Fact]
        public async Task GetProjectTasksAsync_WithFilters_ReturnsFilteredTasks()
        {
            // Arrange
            var projectId = 1;
            var status = "ToDo";
            var assigneeId = 1;

            var tasks = new List<TaskItem>
            {
                new TaskItem { TaskId = 1, Title = "Task 1", Status = "ToDo", AssigneeId = 1 }
            };

            var taskDtos = new List<TaskDto>
            {
                new TaskDto { TaskId = 1, Title = "Task 1" }
            };

            _mockProjectRepository.Setup(r => r.ExistsAsync(projectId))
                .ReturnsAsync(true);
            _mockTaskRepository.Setup(r => r.GetTasksByProjectAsync(projectId, status, assigneeId))
                .ReturnsAsync(tasks);
            _mockMapper.Setup(m => m.Map<IEnumerable<TaskDto>>(tasks))
                .Returns(taskDtos);

            // Act
            var result = await _projectService.GetProjectTasksAsync(projectId, status, assigneeId);

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
            Assert.Equal("Task 1", result.First().Title);
        }

        [Fact]
        public async Task GetProjectTasksAsync_NonExistingProject_ThrowsKeyNotFoundException()
        {
            // Arrange
            var projectId = 999;
            _mockProjectRepository.Setup(r => r.ExistsAsync(projectId))
                .ReturnsAsync(false);

            // Act & Assert
            await Assert.ThrowsAsync<KeyNotFoundException>(() =>
                _projectService.GetProjectTasksAsync(projectId));
        }
    }
}