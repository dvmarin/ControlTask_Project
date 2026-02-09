using ControlTask.Domain.Entities;
using ControlTask.Infrastructure.Persistence;
using ControlTask.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace ControlTask.Tests.Infrastructure.Repositories
{
    public class TaskRepositoryTests : IDisposable
    {
        private readonly ApplicationDbContext _context;
        private readonly TaskRepository _repository;

        public TaskRepositoryTests()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new ApplicationDbContext(options);
            SeedDatabase();
            _repository = new TaskRepository(_context);
        }

        private void SeedDatabase()
        {
            // Crear datos de prueba
            var developer = new Developer
            {
                DeveloperId = 1,
                FirstName = "Test",
                LastName = "Developer",
                Email = "test@example.com",
                IsActive = true
            };

            var project = new Project
            {
                ProjectId = 1,
                Name = "Test Project",
                ClientName = "Test Client",
                StartDate = DateTime.UtcNow
            };

            var tasks = new List<TaskItem>
            {
                new TaskItem
                {
                    TaskId = 1,
                    ProjectId = 1,
                    AssigneeId = 1,
                    Title = "Task 1",
                    Status = "ToDo",
                    DueDate = DateTime.UtcNow.AddDays(3)
                },
                new TaskItem
                {
                    TaskId = 2,
                    ProjectId = 1,
                    AssigneeId = 1,
                    Title = "Task 2",
                    Status = "InProgress",
                    DueDate = DateTime.UtcNow.AddDays(1)
                },
                new TaskItem
                {
                    TaskId = 3,
                    ProjectId = 1,
                    AssigneeId = 1,
                    Title = "Task 3",
                    Status = "Completed",
                    DueDate = DateTime.UtcNow.AddDays(-1)
                }
            };

            _context.Developers.Add(developer);
            _context.Projects.Add(project);
            _context.Tasks.AddRange(tasks);
            _context.SaveChanges();
        }

        [Fact]
        public async Task GetTasksByProjectAsync_ReturnsTasksForProject()
        {
            // Act
            var tasks = await _repository.GetTasksByProjectAsync(1);

            // Assert
            Assert.NotNull(tasks);
            Assert.Equal(3, tasks.Count());
            Assert.All(tasks, t => Assert.Equal(1, t.ProjectId));
        }

        [Fact]
        public async Task GetTasksByProjectAsync_WithStatusFilter_ReturnsFilteredTasks()
        {
            // Act
            var tasks = await _repository.GetTasksByProjectAsync(1, "ToDo");

            // Assert
            Assert.NotNull(tasks);
            Assert.Single(tasks);
            Assert.All(tasks, t => Assert.Equal("ToDo", t.Status));
        }

        [Fact]
        public async Task GetUpcomingTasksAsync_ReturnsTasksDueInNextDays()
        {
            // Act
            var tasks = await _repository.GetUpcomingTasksAsync(5);

            // Assert
            Assert.NotNull(tasks);
            Assert.Equal(2, tasks.Count()); // Task 1 y Task 2 (Task 3 está completada)
            Assert.All(tasks, t => Assert.True(t.DueDate >= DateTime.UtcNow.Date));
        }

        [Fact]
        public async Task GetByIdAsync_WithIncludes_ReturnsTaskWithNavigationProperties()
        {
            // Act
            var task = await _repository.GetByIdAsync(1);

            // Assert
            Assert.NotNull(task);
            Assert.NotNull(task.Project);
            Assert.NotNull(task.Assignee);
            Assert.Equal("Test Project", task.Project.Name);
            Assert.Equal("Test Developer", $"{task.Assignee.FirstName} {task.Assignee.LastName}");
        }

        [Fact]
        public async Task AddAsync_AddsTaskToDatabase()
        {
            // Arrange
            var newTask = new TaskItem
            {
                TaskId = 4,
                ProjectId = 1,
                AssigneeId = 1,
                Title = "New Task",
                Status = "ToDo"
            };

            // Act
            var result = await _repository.AddAsync(newTask);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("New Task", result.Title);
            Assert.True(result.TaskId > 0);
            Assert.Equal(1, await _context.Tasks.CountAsync(t => t.Title == "New Task"));
        }

        [Fact]
        public async Task UpdateAsync_UpdatesTaskCorrectly()
        {
            // Arrange
            var task = await _context.Tasks.FindAsync(1);
            var originalUpdatedAt = task.UpdatedAt;
            task.Title = "Updated Title";

            // Act
            await _repository.UpdateAsync(task);
            await _context.SaveChangesAsync();

            // Assert
            var updatedTask = await _context.Tasks.FindAsync(1);
            Assert.Equal("Updated Title", updatedTask.Title);
            Assert.True(updatedTask.UpdatedAt > originalUpdatedAt);
        }

        [Fact]
        public async Task GetTasksByProjectAsync_WithStatusAndAssignee_ReturnsFilteredTasks()
        {
            var tasks = await _repository.GetTasksByProjectAsync(1, "ToDo", 1);

            Assert.Single(tasks);
            var task = tasks.First();
            Assert.Equal("ToDo", task.Status);
            Assert.Equal(1, task.AssigneeId);
        }

        [Fact]
        public async Task GetPagedTasksByProjectAsync_ReturnsCorrectPage()
        {
            var pagedResult = await _repository.GetPagedTasksByProjectAsync(1, pageNumber: 1, pageSize: 2);

            Assert.Equal(3, pagedResult.TotalCount);
            Assert.Equal(2, pagedResult.Items.Count);
        }

        [Fact]
        public async Task DeleteAsync_RemovesTask()
        {
            await _repository.DeleteAsync(1);

            var task = await _repository.GetByIdAsync(1);
            Assert.Null(task);
            Assert.False(await _repository.ExistsAsync(1));
        }

        [Fact]
        public async Task CountAsync_ReturnsTotalAndFilteredCounts()
        {
            var total = await _repository.CountAsync();
            var filtered = await _repository.CountAsync(t => t.Status == "ToDo");

            Assert.Equal(3, total);
            Assert.Equal(1, filtered);
        }

        [Fact]
        public async Task GetTasksByAssigneeAsync_ReturnsCorrectTasks()
        {
            var tasks = await _repository.GetTasksByAssigneeAsync(1);

            Assert.Equal(3, tasks.Count());
            Assert.All(tasks, t => Assert.Equal(1, t.AssigneeId));
        }

        [Fact]
        public async Task GetTasksDueBeforeAsync_ReturnsTasksBeforeDate()
        {
            var targetDate = DateTime.UtcNow.AddDays(2);
            var tasks = await _repository.GetTasksDueBeforeAsync(targetDate);

            Assert.Equal(2, tasks.Count()); // Task 2 y Task 3
            Assert.All(tasks, t => Assert.True(t.DueDate <= targetDate));
        }

        [Fact]
        public async Task ExistsAsync_ReturnsTrueOrFalse()
        {
            Assert.True(await _repository.ExistsAsync(1));
            Assert.False(await _repository.ExistsAsync(999));
        }


        public void Dispose()
        {
            _context.Dispose();
        }
    }
}