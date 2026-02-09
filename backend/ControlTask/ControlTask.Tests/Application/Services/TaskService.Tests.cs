using Xunit;
using Moq;
using AutoMapper;
using ControlTask.Application.DTOs;
using ControlTask.Application.Services;
using ControlTask.Application.Interfaces;
using ControlTask.Domain.Entities;
using ControlTask.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ControlTask.Application.Tests
{
    public class TaskServiceTests
    {
        private readonly Mock<ITaskRepository> _mockTaskRepository;
        private readonly Mock<IProjectRepository> _mockProjectRepository;
        private readonly Mock<IDeveloperRepository> _mockDeveloperRepository;
        private readonly Mock<IMapper> _mockMapper;
        private readonly TaskService _taskService;

        public TaskServiceTests()
        {
            _mockTaskRepository = new Mock<ITaskRepository>();
            _mockProjectRepository = new Mock<IProjectRepository>();
            _mockDeveloperRepository = new Mock<IDeveloperRepository>();
            _mockMapper = new Mock<IMapper>();
            _taskService = new TaskService(
                _mockTaskRepository.Object,
                _mockProjectRepository.Object,
                _mockDeveloperRepository.Object,
                _mockMapper.Object
            );
        }

        [Fact]
        public async Task GetTaskByIdAsync_ExistingTask_ReturnsTaskDto()
        {
            // Arrange
            var taskId = 1;
            var taskItem = new TaskItem { TaskId = taskId, Title = "Test Task" };
            var taskDto = new TaskDto { TaskId = taskId, Title = "Test Task" };

            _mockTaskRepository.Setup(r => r.GetByIdAsync(taskId))
                .ReturnsAsync(taskItem);
            _mockMapper.Setup(m => m.Map<TaskDto>(taskItem))
                .Returns(taskDto);

            // Act
            var result = await _taskService.GetTaskByIdAsync(taskId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(taskId, result.TaskId);
            Assert.Equal("Test Task", result.Title);
        }

        [Fact]
        public async Task GetTaskByIdAsync_NonExistingTask_ReturnsNull()
        {
            // Arrange
            var taskId = 999;
            _mockTaskRepository.Setup(r => r.GetByIdAsync(taskId))
                .ReturnsAsync((TaskItem)null);

            // Act
            var result = await _taskService.GetTaskByIdAsync(taskId);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task CreateTaskAsync_ValidData_CreatesTask()
        {
            // Arrange
            var createTaskDto = new CreateTaskDto
            {
                ProjectId = 1,
                Title = "New Task",
                AssigneeId = 1,
                Status = "ToDo",
                Priority = "Medium",
                DueDate = DateTime.UtcNow.AddDays(7)
            };

            var projectExists = true;
            var developer = new Developer { DeveloperId = 1, IsActive = true };
            var taskItem = new TaskItem { TaskId = 1 };
            var taskDto = new TaskDto { TaskId = 1 };

            _mockProjectRepository.Setup(r => r.ExistsAsync(createTaskDto.ProjectId))
                .ReturnsAsync(projectExists);
            _mockDeveloperRepository.Setup(r => r.GetByIdAsync(createTaskDto.AssigneeId))
                .ReturnsAsync(developer);
            _mockMapper.Setup(m => m.Map<TaskItem>(createTaskDto))
                .Returns(taskItem);
            _mockTaskRepository.Setup(r => r.AddAsync(taskItem))
                .ReturnsAsync(taskItem);
            _mockMapper.Setup(m => m.Map<TaskDto>(taskItem))
                .Returns(taskDto);

            // Act
            var result = await _taskService.CreateTaskAsync(createTaskDto);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.TaskId);
            _mockTaskRepository.Verify(r => r.AddAsync(It.IsAny<TaskItem>()), Times.Once);
        }

        [Fact]
        public async Task CreateTaskAsync_NonExistingProject_ThrowsKeyNotFoundException()
        {
            // Arrange
            var createTaskDto = new CreateTaskDto
            {
                ProjectId = 999,
                AssigneeId = 1
            };

            _mockProjectRepository.Setup(r => r.ExistsAsync(createTaskDto.ProjectId))
                .ReturnsAsync(false);

            // Act & Assert
            await Assert.ThrowsAsync<KeyNotFoundException>(() =>
                _taskService.CreateTaskAsync(createTaskDto));
        }

        [Fact]
        public async Task CreateTaskAsync_InactiveDeveloper_ThrowsArgumentException()
        {
            // Arrange
            var createTaskDto = new CreateTaskDto
            {
                ProjectId = 1,
                AssigneeId = 1
            };

            var projectExists = true;
            var developer = new Developer { DeveloperId = 1, IsActive = false };

            _mockProjectRepository.Setup(r => r.ExistsAsync(createTaskDto.ProjectId))
                .ReturnsAsync(projectExists);
            _mockDeveloperRepository.Setup(r => r.GetByIdAsync(createTaskDto.AssigneeId))
                .ReturnsAsync(developer);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ArgumentException>(() =>
                _taskService.CreateTaskAsync(createTaskDto));
            Assert.Contains("no está activo", exception.Message);
        }

        [Fact]
        public async Task UpdateTaskStatusAsync_ValidStatus_UpdatesTask()
        {
            // Arrange
            var taskId = 1;
            var updateDto = new UpdateTaskStatusDto
            {
                Status = "Completed",
                Priority = "High"
            };

            var existingTask = new TaskItem
            {
                TaskId = taskId,
                Status = "InProgress",
                Priority = "Medium"
            };

            var updatedTaskDto = new TaskDto { TaskId = taskId, Status = "Completed" };

            _mockTaskRepository.Setup(r => r.GetByIdAsync(taskId))
                .ReturnsAsync(existingTask);
            _mockTaskRepository.Setup(r => r.UpdateAsync(It.IsAny<TaskItem>()))
                .Returns(Task.CompletedTask);
            _mockMapper.Setup(m => m.Map<TaskDto>(It.IsAny<TaskItem>()))
                .Returns(updatedTaskDto);

            // Act
            var result = await _taskService.UpdateTaskStatusAsync(taskId, updateDto);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Completed", result.Status);
            _mockTaskRepository.Verify(r => r.UpdateAsync(It.IsAny<TaskItem>()), Times.Once);
        }

        [Fact]
        public async Task UpdateTaskStatusAsync_InvalidStatus_ThrowsArgumentException()
        {
            // Arrange
            var taskId = 1;
            var updateDto = new UpdateTaskStatusDto
            {
                Status = "InvalidStatus"
            };

            var existingTask = new TaskItem { TaskId = taskId };

            _mockTaskRepository.Setup(r => r.GetByIdAsync(taskId))
                .ReturnsAsync(existingTask);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ArgumentException>(() =>
                _taskService.UpdateTaskStatusAsync(taskId, updateDto));
            Assert.Contains("Status debe ser", exception.Message);
        }

        [Fact]
        public async Task GetPagedTasksByProjectAsync_ValidProject_ReturnsPagedResultDto()
        {
            // Arrange
            int projectId = 1;
            int page = 1;
            int pageSize = 2;

            // Creamos el resultado del repositorio (tipo PagedResult<TaskItem>)
            var pagedTasks = new PagedResult<TaskItem>
            {
                Items = new List<TaskItem>
        {
            new TaskItem { TaskId = 1, Title = "Task 1" },
            new TaskItem { TaskId = 2, Title = "Task 2" }
        },
                TotalCount = 2,
                PageNumber = page,
                PageSize = pageSize
            };

            // Configuramos el repo
            _mockProjectRepository.Setup(r => r.ExistsAsync(projectId))
                .ReturnsAsync(true);

            _mockTaskRepository.Setup(r => r.GetPagedTasksByProjectAsync(projectId, page, pageSize, null, null))
                .ReturnsAsync(pagedTasks);

            // Configuramos AutoMapper
            _mockMapper.Setup(m => m.Map<List<TaskDto>>(It.IsAny<List<TaskItem>>()))
                .Returns((List<TaskItem> source) =>
                    source.Select(t => new TaskDto { TaskId = t.TaskId, Title = t.Title }).ToList()
                );

            // Act
            var result = await _taskService.GetPagedTasksByProjectAsync(projectId, page, pageSize);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Items.Count);
            Assert.Equal("Task 1", result.Items[0].Title);
            Assert.Equal("Task 2", result.Items[1].Title);
            Assert.Equal(2, result.TotalCount);
            Assert.Equal(page, result.PageNumber);
            Assert.Equal(pageSize, result.PageSize);
        }

    }
}