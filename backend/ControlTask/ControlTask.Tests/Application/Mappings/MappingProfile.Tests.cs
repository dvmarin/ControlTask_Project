using Xunit;
using AutoMapper;
using ControlTask.Application.Mappings;
using ControlTask.Application.DTOs;
using ControlTask.Domain.Entities;
using System;
using System.Collections.Generic;

namespace ControlTask.Tests.Application.Mappings
{
    public class MappingProfileTests
    {
        private readonly IMapper _mapper;

        public MappingProfileTests()
        {
            var configuration = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new MappingProfile());
            });

            _mapper = configuration.CreateMapper();
        }

        [Fact]
        public void Map_DeveloperToDeveloperDto_MapsCorrectly()
        {
            // Arrange
            var developer = new Developer
            {
                DeveloperId = 1,
                FirstName = "John",
                LastName = "Doe",
                Email = "john.doe@example.com",
                IsActive = true
            };

            // Act
            var developerDto = _mapper.Map<DeveloperDto>(developer);

            // Assert
            Assert.Equal(1, developerDto.DeveloperId);
            Assert.Equal("John Doe", developerDto.FullName);
            Assert.Equal("john.doe@example.com", developerDto.Email);
            Assert.True(developerDto.IsActive);
        }

        [Fact]
        public void Map_ProjectToProjectDto_CalculatesTaskStats()
        {
            // Arrange
            var project = new Project
            {
                ProjectId = 1,
                Name = "Test Project",
                ClientName = "Test Client",
                Tasks = new List<TaskItem>
                {
                    new TaskItem { Status = "ToDo" },
                    new TaskItem { Status = "InProgress" },
                    new TaskItem { Status = "Completed" },
                    new TaskItem { Status = "Completed" }
                }
            };

            // Act
            var projectDto = _mapper.Map<ProjectDto>(project);

            // Assert
            Assert.Equal(1, projectDto.ProjectId);
            Assert.Equal("Test Project", projectDto.Name);
            Assert.Equal(4, projectDto.TotalTasks);
            Assert.Equal(2, projectDto.OpenTasks); // ToDo + InProgress
            Assert.Equal(2, projectDto.CompletedTasks);
        }

        [Fact]
        public void Map_TaskItemToTaskDto_MapsNavigationProperties()
        {
            // Arrange
            var taskItem = new TaskItem
            {
                TaskId = 1,
                Title = "Test Task",
                Project = new Project { Name = "Project A" },
                Assignee = new Developer { FirstName = "John", LastName = "Doe" }
            };

            // Act
            var taskDto = _mapper.Map<TaskDto>(taskItem);

            // Assert
            Assert.Equal(1, taskDto.TaskId);
            Assert.Equal("Test Task", taskDto.Title);
            Assert.Equal("Project A", taskDto.ProjectName);
            Assert.Equal("John Doe", taskDto.AssigneeName);
        }

        [Fact]
        public void Map_CreateTaskDtoToTaskItem_MapsAllProperties()
        {
            // Arrange
            var createTaskDto = new CreateTaskDto
            {
                ProjectId = 1,
                Title = "New Task",
                Description = "Task Description",
                AssigneeId = 1,
                Status = "ToDo",
                Priority = "High",
                EstimatedComplexity = 3,
                DueDate = DateTime.UtcNow.AddDays(7)
            };

            // Act
            var taskItem = _mapper.Map<TaskItem>(createTaskDto);

            // Assert
            Assert.Equal(1, taskItem.ProjectId);
            Assert.Equal("New Task", taskItem.Title);
            Assert.Equal("Task Description", taskItem.Description);
            Assert.Equal(1, taskItem.AssigneeId);
            Assert.Equal("ToDo", taskItem.Status);
            Assert.Equal("High", taskItem.Priority);
            Assert.Equal(3, taskItem.EstimatedComplexity);
            Assert.Equal(createTaskDto.DueDate, taskItem.DueDate);
        }

        [Fact]
        public void Map_UpdateTaskStatusDtoToTaskItem_IgnoresNullProperties()
        {
            // Arrange
            var updateDto = new UpdateTaskStatusDto
            {
                Status = "Completed",
                Priority = null,
                EstimatedComplexity = 5
            };

            var existingTaskItem = new TaskItem
            {
                Status = "InProgress",
                Priority = "Medium",
                EstimatedComplexity = 3
            };

            // Act
            _mapper.Map(updateDto, existingTaskItem);

            // Assert
            Assert.Equal("Completed", existingTaskItem.Status);
            Assert.Equal("Medium", existingTaskItem.Priority); // No cambió porque era null
            Assert.Equal(5, existingTaskItem.EstimatedComplexity); // Cambió porque tenía valor
        }
    }
}