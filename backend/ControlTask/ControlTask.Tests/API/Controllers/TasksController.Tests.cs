using ControlTask.API.Controllers;
using ControlTask.Application.DTOs;
using ControlTask.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace ControlTask.Tests.API.Controllers
{
    public class TasksControllerTests
    {
        private readonly Mock<ITaskService> _mockTaskService;
        private readonly TasksController _controller;

        public TasksControllerTests()
        {
            _mockTaskService = new Mock<ITaskService>();
            _controller = new TasksController(_mockTaskService.Object);
        }

        #region GetTask

        [Fact]
        public async Task GetTask_ExistingId_ReturnsOkResult()
        {
            var taskId = 1;
            var taskDto = new TaskDto { TaskId = taskId, Title = "Test Task" };
            _mockTaskService.Setup(s => s.GetTaskByIdAsync(taskId)).ReturnsAsync(taskDto);

            var result = await _controller.GetTask(taskId);

            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(taskId, ((TaskDto)okResult.Value).TaskId);
        }

        [Fact]
        public async Task GetTask_NonExistingId_ReturnsNotFound()
        {
            _mockTaskService.Setup(s => s.GetTaskByIdAsync(999)).ReturnsAsync((TaskDto)null);

            var result = await _controller.GetTask(999);

            Assert.IsType<NotFoundObjectResult>(result);
        }

        [Fact]
        public async Task GetTask_ServiceException_Returns500()
        {
            _mockTaskService.Setup(s => s.GetTaskByIdAsync(1)).ThrowsAsync(new Exception("DB error"));

            var result = await _controller.GetTask(1);

            var objResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, objResult.StatusCode);
        }

        #endregion

        #region CreateTask

        [Fact]
        public async Task CreateTask_ValidData_ReturnsCreatedAtAction()
        {
            var createDto = new CreateTaskDto { ProjectId = 1, AssigneeId = 1, Title = "New Task" };
            var createdTask = new TaskDto { TaskId = 1, Title = "New Task" };
            _mockTaskService.Setup(s => s.CreateTaskAsync(createDto)).ReturnsAsync(createdTask);

            var result = await _controller.CreateTask(createDto);

            var createdAtResult = Assert.IsType<CreatedAtActionResult>(result);
            Assert.Equal(nameof(TasksController.GetTask), createdAtResult.ActionName);
            Assert.Equal(1, createdAtResult.RouteValues["id"]);
        }

        [Fact]
        public async Task CreateTask_KeyNotFoundException_ReturnsBadRequest()
        {
            var createDto = new CreateTaskDto { ProjectId = 999, AssigneeId = 1 };
            _mockTaskService.Setup(s => s.CreateTaskAsync(createDto))
                .ThrowsAsync(new KeyNotFoundException("Proyecto no encontrado"));

            var result = await _controller.CreateTask(createDto);

            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Contains("Proyecto no encontrado", badRequest.Value.ToString());
        }

        [Fact]
        public async Task CreateTask_ArgumentException_ReturnsBadRequest()
        {
            var createDto = new CreateTaskDto { ProjectId = 1, AssigneeId = 1 };
            _mockTaskService.Setup(s => s.CreateTaskAsync(createDto))
                .ThrowsAsync(new ArgumentException("Datos inválidos"));

            var result = await _controller.CreateTask(createDto);

            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Contains("Datos inválidos", badRequest.Value.ToString());
        }

        [Fact]
        public async Task CreateTask_ServiceException_Returns500()
        {
            var createDto = new CreateTaskDto { ProjectId = 1, AssigneeId = 1 };
            _mockTaskService.Setup(s => s.CreateTaskAsync(createDto))
                .ThrowsAsync(new Exception("DB error"));

            var result = await _controller.CreateTask(createDto);

            var objResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, objResult.StatusCode);
        }

        #endregion

        #region UpdateTaskStatus

        [Fact]
        public async Task UpdateTaskStatus_ValidUpdate_ReturnsOk()
        {
            var taskId = 1;
            var updateDto = new UpdateTaskStatusDto { Status = "Completed" };
            var updatedTask = new TaskDto { TaskId = taskId, Status = "Completed" };

            _mockTaskService.Setup(s => s.UpdateTaskStatusAsync(taskId, updateDto))
                .ReturnsAsync(updatedTask);

            var result = await _controller.UpdateTaskStatus(taskId, updateDto);

            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("Completed", ((TaskDto)okResult.Value).Status);
        }

        [Fact]
        public async Task UpdateTaskStatus_NotFound_Returns404()
        {
            var taskId = 999;
            var updateDto = new UpdateTaskStatusDto { Status = "Completed" };
            _mockTaskService.Setup(s => s.UpdateTaskStatusAsync(taskId, updateDto))
                .ThrowsAsync(new KeyNotFoundException("Tarea no encontrada"));

            var result = await _controller.UpdateTaskStatus(taskId, updateDto);

            var notFound = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Contains("Tarea no encontrada", notFound.Value.ToString());
        }

        [Fact]
        public async Task UpdateTaskStatus_ArgumentException_ReturnsBadRequest()
        {
            var taskId = 1;
            var updateDto = new UpdateTaskStatusDto { Status = "Invalid" };
            _mockTaskService.Setup(s => s.UpdateTaskStatusAsync(taskId, updateDto))
                .ThrowsAsync(new ArgumentException("Status inválido"));

            var result = await _controller.UpdateTaskStatus(taskId, updateDto);

            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Contains("Status inválido", badRequest.Value.ToString());
        }

        [Fact]
        public async Task UpdateTaskStatus_ServiceException_Returns500()
        {
            var taskId = 1;
            var updateDto = new UpdateTaskStatusDto { Status = "Completed" };
            _mockTaskService.Setup(s => s.UpdateTaskStatusAsync(taskId, updateDto))
                .ThrowsAsync(new Exception("DB error"));

            var result = await _controller.UpdateTaskStatus(taskId, updateDto);

            var objResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, objResult.StatusCode);
        }

        #endregion

        #region DeleteTask

        [Fact]
        public async Task DeleteTask_ExistingTask_ReturnsNoContent()
        {
            var taskId = 1;
            _mockTaskService.Setup(s => s.DeleteTaskAsync(taskId)).Returns(Task.CompletedTask);

            var result = await _controller.DeleteTask(taskId);

            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task DeleteTask_NotFound_Returns404()
        {
            _mockTaskService.Setup(s => s.DeleteTaskAsync(999))
                .ThrowsAsync(new KeyNotFoundException("Tarea no encontrada"));

            var result = await _controller.DeleteTask(999);

            var notFound = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Contains("Tarea no encontrada", notFound.Value.ToString());
        }

        [Fact]
        public async Task DeleteTask_ServiceException_Returns500()
        {
            _mockTaskService.Setup(s => s.DeleteTaskAsync(1)).ThrowsAsync(new Exception("DB error"));

            var result = await _controller.DeleteTask(1);

            var objResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, objResult.StatusCode);
        }

        #endregion

        #region GetUpcomingTasks

        [Fact]
        public async Task GetUpcomingTasks_ValidDays_ReturnsOk()
        {
            var tasks = new List<UpcomingTaskDto> { new UpcomingTaskDto { Title = "Task 1", DueDate = DateTime.UtcNow.AddDays(2) } };
            _mockTaskService.Setup(s => s.GetUpcomingTasksAsync(7)).ReturnsAsync(tasks);

            var result = await _controller.GetUpcomingTasks(7);

            var ok = Assert.IsType<OkObjectResult>(result);
            var list = Assert.IsType<List<UpcomingTaskDto>>(ok.Value);
            Assert.Single(list);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(31)]
        public async Task GetUpcomingTasks_InvalidDays_ReturnsBadRequest(int days)
        {
            var result = await _controller.GetUpcomingTasks(days);

            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task GetUpcomingTasks_ServiceException_Returns500()
        {
            _mockTaskService.Setup(s => s.GetUpcomingTasksAsync(7)).ThrowsAsync(new Exception("DB error"));

            var result = await _controller.GetUpcomingTasks(7);

            var objResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, objResult.StatusCode);
        }

        #endregion

        #region GetTasksByAssignee

        [Fact]
        public async Task GetTasksByAssignee_ReturnsOk()
        {
            var assigneeId = 1;
            var tasks = new List<TaskDto> { new TaskDto { TaskId = 1, Title = "Task A" } };
            _mockTaskService.Setup(s => s.GetTasksByAssigneeAsync(assigneeId)).ReturnsAsync(tasks);

            var result = await _controller.GetTasksByAssignee(assigneeId);

            var ok = Assert.IsType<OkObjectResult>(result);
            var list = Assert.IsType<List<TaskDto>>(ok.Value);
            Assert.Single(list);
        }

        [Fact]
        public async Task GetTasksByAssignee_ServiceException_Returns500()
        {
            _mockTaskService.Setup(s => s.GetTasksByAssigneeAsync(1)).ThrowsAsync(new Exception("DB error"));

            var result = await _controller.GetTasksByAssignee(1);

            var objResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, objResult.StatusCode);
        }

        #endregion
    }
}
