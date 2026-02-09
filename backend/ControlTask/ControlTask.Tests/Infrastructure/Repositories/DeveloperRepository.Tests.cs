using ControlTask.Domain.Entities;
using ControlTask.Infrastructure.Persistence;
using ControlTask.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace ControlTask.Infrastructure.Tests
{
    public class DeveloperRepositoryTests : IDisposable
    {
        private readonly ApplicationDbContext _context;
        private readonly DeveloperRepository _repository;

        public DeveloperRepositoryTests()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new ApplicationDbContext(options);
            SeedDatabase();
            _repository = new DeveloperRepository(_context);
        }

        private void SeedDatabase()
        {
            // Crear desarrolladores
            var developers = new List<Developer>
            {
                new Developer
                {
                    DeveloperId = 1,
                    FirstName = "John",
                    LastName = "Doe",
                    Email = "john@example.com",
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow.AddDays(-10)
                },
                new Developer
                {
                    DeveloperId = 2,
                    FirstName = "Jane",
                    LastName = "Smith",
                    Email = "jane@example.com",
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow.AddDays(-5)
                },
                new Developer
                {
                    DeveloperId = 3,
                    FirstName = "Bob",
                    LastName = "Johnson",
                    Email = "bob@example.com",
                    IsActive = false,
                    CreatedAt = DateTime.UtcNow.AddDays(-2)
                }
            };

            // Crear tareas para algunos desarrolladores
            var tasks = new List<TaskItem>
            {
                new TaskItem
                {
                    TaskId = 1,
                    Title = "Task 1",
                    AssigneeId = 1,
                    Status = "ToDo"
                },
                new TaskItem
                {
                    TaskId = 2,
                    Title = "Task 2",
                    AssigneeId = 1,
                    Status = "InProgress"
                },
                new TaskItem
                {
                    TaskId = 3,
                    Title = "Task 3",
                    AssigneeId = 2,
                    Status = "Completed"
                }
            };

            _context.Developers.AddRange(developers);
            _context.Tasks.AddRange(tasks);
            _context.SaveChanges();
        }

        [Fact]
        public async Task GetActiveAsync_ReturnsOnlyActiveDevelopers()
        {
            // Act
            var result = await _repository.GetActiveAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count()); // Solo John y Jane están activos
            Assert.All(result, d => Assert.True(d.IsActive));
            Assert.Contains(result, d => d.FirstName == "John");
            Assert.Contains(result, d => d.FirstName == "Jane");
            Assert.DoesNotContain(result, d => d.FirstName == "Bob");
        }

        [Fact]
        public async Task GetDevelopersWithTasksAsync_ReturnsDevelopersWithTasks()
        {
            // Act
            var result = await _repository.GetDevelopersWithTasksAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count()); // Solo desarrolladores activos

            var john = result.First(d => d.DeveloperId == 1);
            Assert.NotNull(john.Tasks);
            Assert.Equal(2, john.Tasks.Count); // John tiene 2 tareas

            var jane = result.First(d => d.DeveloperId == 2);
            Assert.NotNull(jane.Tasks);
            Assert.Single(jane.Tasks); // Jane tiene 1 tarea
        }

        [Fact]
        public async Task GetByIdAsync_ExistingId_ReturnsDeveloper()
        {
            // Act
            var result = await _repository.GetByIdAsync(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.DeveloperId);
            Assert.Equal("John", result.FirstName);
            Assert.Equal("Doe", result.LastName);
            Assert.True(result.IsActive);
        }

        [Fact]
        public async Task GetByIdAsync_NonExistingId_ReturnsNull()
        {
            // Act
            var result = await _repository.GetByIdAsync(999);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GetAllAsync_ReturnsAllDevelopers()
        {
            // Act
            var result = await _repository.GetAllAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(3, result.Count()); // Todos los desarrolladores
            Assert.Contains(result, d => d.FirstName == "John");
            Assert.Contains(result, d => d.FirstName == "Jane");
            Assert.Contains(result, d => d.FirstName == "Bob");
        }

        [Fact]
        public async Task FindAsync_WithPredicate_ReturnsFilteredDevelopers()
        {
            // Act - Buscar desarrolladores con email de ejemplo
            var result = await _repository.FindAsync(d => d.Email.Contains("example.com"));

            // Assert
            Assert.NotNull(result);
            Assert.Equal(3, result.Count());
            Assert.All(result, d => Assert.Contains("example.com", d.Email));
        }

        [Fact]
        public async Task AddAsync_AddsDeveloperToDatabase()
        {
            // Arrange
            var newDeveloper = new Developer
            {
                FirstName = "Alice",
                LastName = "Williams",
                Email = "alice@example.com",
                IsActive = true
            };

            // Act
            var result = await _repository.AddAsync(newDeveloper);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.DeveloperId > 0);
            Assert.Equal("Alice", result.FirstName);
            Assert.Equal(4, await _context.Developers.CountAsync());
        }

        [Fact]
        public async Task UpdateAsync_UpdatesDeveloperCorrectly()
        {
            // Arrange
            var developer = await _context.Developers.FindAsync(1);
            var originalEmail = developer.Email;
            developer.Email = "updated@example.com";

            // Act
            await _repository.UpdateAsync(developer);
            await _context.SaveChangesAsync();

            // Assert
            var updatedDeveloper = await _context.Developers.FindAsync(1);
            Assert.Equal("updated@example.com", updatedDeveloper.Email);
            Assert.NotEqual(originalEmail, updatedDeveloper.Email);
        }

        [Fact]
        public async Task ExistsAsync_ExistingId_ReturnsTrue()
        {
            // Act
            var result = await _repository.ExistsAsync(1);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task ExistsAsync_NonExistingId_ReturnsFalse()
        {
            // Act
            var result = await _repository.ExistsAsync(999);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task CountAsync_ReturnsCorrectCount()
        {
            // Act
            var result = await _repository.CountAsync();

            // Assert
            Assert.Equal(3, result); // 3 desarrolladores en la base de datos
        }

        [Fact]
        public async Task CountAsync_WithPredicate_ReturnsFilteredCount()
        {
            // Act
            var result = await _repository.CountAsync(d => d.IsActive);

            // Assert
            Assert.Equal(2, result); // 2 desarrolladores activos
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}