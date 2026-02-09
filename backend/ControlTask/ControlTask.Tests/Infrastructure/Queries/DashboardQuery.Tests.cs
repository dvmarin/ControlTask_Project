using ControlTask.Domain.Entities;
using ControlTask.Infrastructure.Persistence;
using ControlTask.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Moq;

namespace ControlTask.Tests.Infrastructure.Queries
{
    public class DashboardQueryTests : IDisposable
    {
        private readonly ApplicationDbContext _context;
        private readonly Mock<IConfiguration> _mockConfiguration;
        private readonly DashboardQuery _dashboardQuery;

        public DashboardQueryTests()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new ApplicationDbContext(options);
            _mockConfiguration = new Mock<IConfiguration>();

            var configurationSection = new Mock<IConfigurationSection>();
            configurationSection.Setup(a => a.Value).Returns("fake-connection-string");
            _mockConfiguration.Setup(a => a.GetSection("ConnectionStrings:DefaultConnection"))
                .Returns(configurationSection.Object);

            _dashboardQuery = new DashboardQuery(_context, _mockConfiguration.Object);
            SeedDatabase();
        }

        private void SeedDatabase()
        {
            // Crear desarrolladores
            var developers = new List<Developer>
            {
                new Developer { DeveloperId = 1, FirstName = "John", LastName = "Doe", IsActive = true },
                new Developer { DeveloperId = 2, FirstName = "Jane", LastName = "Smith", IsActive = true },
                new Developer { DeveloperId = 3, FirstName = "Bob", LastName = "Johnson", IsActive = false }
            };

            // Crear proyectos
            var projects = new List<Project>
            {
                new Project { ProjectId = 1, Name = "Project A", ClientName = "Client A" },
                new Project { ProjectId = 2, Name = "Project B", ClientName = "Client B" }
            };

            // Crear tareas con diferentes estados y fechas
            var tasks = new List<TaskItem>
            {
                // Tareas para John Doe
                new TaskItem
                {
                    TaskId = 1,
                    ProjectId = 1,
                    AssigneeId = 1,
                    Title = "Task 1",
                    Status = "ToDo",
                    EstimatedComplexity = 3,
                    DueDate = DateTime.UtcNow.AddDays(5)
                },
                new TaskItem
                {
                    TaskId = 2,
                    ProjectId = 1,
                    AssigneeId = 1,
                    Title = "Task 2",
                    Status = "InProgress",
                    EstimatedComplexity = 5,
                    DueDate = DateTime.UtcNow.AddDays(2)
                },
                new TaskItem
                {
                    TaskId = 3,
                    ProjectId = 1,
                    AssigneeId = 1,
                    Title = "Task 3",
                    Status = "Completed",
                    EstimatedComplexity = 2,
                    DueDate = DateTime.UtcNow.AddDays(-3),
                    CompletionDate = DateTime.UtcNow
                },
                
                // Tareas para Jane Smith
                new TaskItem
                {
                    TaskId = 4,
                    ProjectId = 2,
                    AssigneeId = 2,
                    Title = "Task 4",
                    Status = "Blocked",
                    EstimatedComplexity = 4,
                    DueDate = DateTime.UtcNow.AddDays(1)
                },
                new TaskItem
                {
                    TaskId = 5,
                    ProjectId = 2,
                    AssigneeId = 2,
                    Title = "Task 5",
                    Status = "Completed",
                    EstimatedComplexity = 1,
                    DueDate = DateTime.UtcNow.AddDays(-5),
                    CompletionDate = DateTime.UtcNow.AddDays(-2) // Completada 3 días después
                }
            };

            _context.Developers.AddRange(developers);
            _context.Projects.AddRange(projects);
            _context.Tasks.AddRange(tasks);
            _context.SaveChanges();
        }

        [Fact]
        public async Task GetDeveloperWorkloadAsync_ReturnsWorkloadForActiveDevelopers()
        {
            // Act
            var result = await _dashboardQuery.GetDeveloperWorkloadAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count()); // Solo desarrolladores activos

            var johnWorkload = result.First(r => r.DeveloperName == "John Doe");
            Assert.Equal(2, johnWorkload.OpenTasksCount); // 2 tareas no completadas
            Assert.Equal(4, johnWorkload.AverageEstimatedComplexity); // (3 + 5) / 2 = 4

            var janeWorkload = result.First(r => r.DeveloperName == "Jane Smith");
            Assert.Equal(1, janeWorkload.OpenTasksCount);
            Assert.Equal(4, janeWorkload.AverageEstimatedComplexity);
        }

        [Fact]
        public async Task GetProjectHealthAsync_ReturnsHealthForAllProjects()
        {
            // Act
            var result = await _dashboardQuery.GetProjectHealthAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count());

            var projectA = result.First(r => r.ProjectName == "Project A");
            Assert.Equal(3, projectA.TotalTasks);
            Assert.Equal(2, projectA.OpenTasks);
            Assert.Equal(1, projectA.CompletedTasks);

            var projectB = result.First(r => r.ProjectName == "Project B");
            Assert.Equal(2, projectB.TotalTasks);
            Assert.Equal(1, projectB.OpenTasks);
            Assert.Equal(1, projectB.CompletedTasks);
        }

        [Fact]
        public async Task GetUpcomingTasksAsync_ReturnsTasksDueInNextDays()
        {
            // Act
            var result = await _dashboardQuery.GetUpcomingTasksAsync(3);

            // Assert
            Assert.NotNull(result);
            // Debería retornar tareas con due date en los próximos 3 días
            // Task 2 (2 días), Task 4 (1 día)
            Assert.Equal(2, result.Count());

            var task2 = result.First(r => r.Title == "Task 2");
            Assert.Equal(2, task2.DaysUntilDue);
            Assert.Equal("InProgress", task2.Status);

            var task4 = result.First(r => r.Title == "Task 4");
            Assert.Equal(1, task4.DaysUntilDue);
            Assert.Equal("Blocked", task4.Status);
        }

        [Fact]
        public async Task GetUpcomingTasksAsync_NoTasksDue_ReturnsEmptyList()
        {
            // Act - Buscar tareas para mañana solamente
            var result = await _dashboardQuery.GetUpcomingTasksAsync(0);

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}