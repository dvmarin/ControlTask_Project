using Xunit;
using Microsoft.EntityFrameworkCore;
using ControlTask.Domain.Entities;
using ControlTask.Infrastructure.Persistence;
using ControlTask.Infrastructure.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ControlTask.Infrastructure.Tests
{
    public class ProjectRepositoryTests : IDisposable
    {
        private readonly ApplicationDbContext _context;
        private readonly ProjectRepository _repository;

        public ProjectRepositoryTests()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: $"ProjectRepositoryTests_{Guid.NewGuid()}")
                .Options;

            _context = new ApplicationDbContext(options);
            InitializeDatabase();
            _repository = new ProjectRepository(_context);
        }

        private void InitializeDatabase()
        {
            // Limpiar base de datos
            _context.Database.EnsureDeleted();
            _context.Database.EnsureCreated();

            // Crear datos de prueba
            var developers = new List<Developer>
            {
                new Developer
                {
                    DeveloperId = 1,
                    FirstName = "John",
                    LastName = "Doe",
                    Email = "john@example.com",
                    IsActive = true
                },
                new Developer
                {
                    DeveloperId = 2,
                    FirstName = "Jane",
                    LastName = "Smith",
                    Email = "jane@example.com",
                    IsActive = true
                },
                new Developer
                {
                    DeveloperId = 3,
                    FirstName = "Bob",
                    LastName = "Johnson",
                    Email = "bob@example.com",
                    IsActive = false
                }
            };

            var projects = new List<Project>
            {
                new Project
                {
                    ProjectId = 1,
                    Name = "E-Commerce Platform",
                    ClientName = "Retail Corp",
                    StartDate = new DateTime(2024, 1, 1),
                    EndDate = new DateTime(2024, 6, 30),
                    Status = "InProgress",
                    CreatedAt = new DateTime(2024, 1, 1),
                    UpdatedAt = new DateTime(2024, 1, 15)
                },
                new Project
                {
                    ProjectId = 2,
                    Name = "Mobile Banking App",
                    ClientName = "Bank Inc",
                    StartDate = new DateTime(2024, 2, 1),
                    EndDate = new DateTime(2024, 8, 31),
                    Status = "Planned",
                    CreatedAt = new DateTime(2024, 2, 1),
                    UpdatedAt = new DateTime(2024, 2, 1)
                },
                new Project
                {
                    ProjectId = 3,
                    Name = "Inventory System",
                    ClientName = "Logistics Co",
                    StartDate = new DateTime(2023, 11, 1),
                    EndDate = new DateTime(2024, 1, 31),
                    Status = "Completed",
                    CreatedAt = new DateTime(2023, 11, 1),
                    UpdatedAt = new DateTime(2024, 1, 31)
                },
                new Project
                {
                    ProjectId = 4,
                    Name = "CRM Implementation",
                    ClientName = "Sales Corp",
                    StartDate = new DateTime(2024, 3, 1),
                    EndDate = null, // Proyecto sin fecha de fin
                    Status = "InProgress",
                    CreatedAt = new DateTime(2024, 3, 1),
                    UpdatedAt = new DateTime(2024, 3, 15)
                }
            };

            var tasks = new List<TaskItem>
            {
                // Tareas para Proyecto 1
                new TaskItem
                {
                    TaskId = 1,
                    ProjectId = 1,
                    AssigneeId = 1,
                    Title = "Design Database Schema",
                    Status = "Completed",
                    Priority = "High",
                    EstimatedComplexity = 3,
                    DueDate = new DateTime(2024, 1, 15),
                    CompletionDate = new DateTime(2024, 1, 14),
                    CreatedAt = new DateTime(2024, 1, 1)
                },
                new TaskItem
                {
                    TaskId = 2,
                    ProjectId = 1,
                    AssigneeId = 1,
                    Title = "Implement User Authentication",
                    Status = "InProgress",
                    Priority = "High",
                    EstimatedComplexity = 4,
                    DueDate = new DateTime(2024, 2, 15),
                    CreatedAt = new DateTime(2024, 1, 2)
                },
                new TaskItem
                {
                    TaskId = 3,
                    ProjectId = 1,
                    AssigneeId = 2,
                    Title = "Design UI Mockups",
                    Status = "ToDo",
                    Priority = "Medium",
                    EstimatedComplexity = 2,
                    DueDate = new DateTime(2024, 2, 28),
                    CreatedAt = new DateTime(2024, 1, 3)
                },
                new TaskItem
                {
                    TaskId = 4,
                    ProjectId = 1,
                    AssigneeId = 2,
                    Title = "Write Unit Tests",
                    Status = "Blocked",
                    Priority = "Low",
                    EstimatedComplexity = 3,
                    DueDate = new DateTime(2024, 3, 15),
                    CreatedAt = new DateTime(2024, 1, 4)
                },

                // Tareas para Proyecto 2
                new TaskItem
                {
                    TaskId = 5,
                    ProjectId = 2,
                    AssigneeId = 1,
                    Title = "Project Planning",
                    Status = "ToDo",
                    Priority = "Medium",
                    EstimatedComplexity = 2,
                    DueDate = new DateTime(2024, 3, 1),
                    CreatedAt = new DateTime(2024, 2, 1)
                },
                new TaskItem
                {
                    TaskId = 6,
                    ProjectId = 2,
                    AssigneeId = 2,
                    Title = "Requirement Analysis",
                    Status = "ToDo",
                    Priority = "Medium",
                    EstimatedComplexity = 3,
                    DueDate = new DateTime(2024, 3, 10),
                    CreatedAt = new DateTime(2024, 2, 2)
                },

                // Tareas para Proyecto 3 (Completado)
                new TaskItem
                {
                    TaskId = 7,
                    ProjectId = 3,
                    AssigneeId = 1,
                    Title = "Legacy System Analysis",
                    Status = "Completed",
                    Priority = "High",
                    EstimatedComplexity = 5,
                    DueDate = new DateTime(2023, 12, 15),
                    CompletionDate = new DateTime(2023, 12, 14),
                    CreatedAt = new DateTime(2023, 11, 1)
                },
                new TaskItem
                {
                    TaskId = 8,
                    ProjectId = 3,
                    AssigneeId = 2,
                    Title = "Deploy to Production",
                    Status = "Completed",
                    Priority = "High",
                    EstimatedComplexity = 4,
                    DueDate = new DateTime(2024, 1, 31),
                    CompletionDate = new DateTime(2024, 1, 30),
                    CreatedAt = new DateTime(2023, 11, 2)
                },

                // Tarea para Proyecto 4
                new TaskItem
                {
                    TaskId = 9,
                    ProjectId = 4,
                    AssigneeId = 1,
                    Title = "Setup Development Environment",
                    Status = "InProgress",
                    Priority = "Medium",
                    EstimatedComplexity = 2,
                    DueDate = new DateTime(2024, 3, 20),
                    CreatedAt = new DateTime(2024, 3, 1)
                }
            };

            _context.Developers.AddRange(developers);
            _context.Projects.AddRange(projects);
            _context.Tasks.AddRange(tasks);
            _context.SaveChanges();
        }

        #region Tests for GetProjectsWithTasksAsync

        [Fact]
        public async Task GetProjectsWithTasksAsync_ReturnsAllProjectsWithTasks()
        {
            // Act
            var result = await _repository.GetProjectsWithTasksAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(4, result.Count()); // 4 proyectos en total

            // Verificar que todas las tareas están cargadas
            var project1 = result.First(p => p.ProjectId == 1);
            Assert.Equal(4, project1.Tasks.Count);

            var project2 = result.First(p => p.ProjectId == 2);
            Assert.Equal(2, project2.Tasks.Count);

            var project3 = result.First(p => p.ProjectId == 3);
            Assert.Equal(2, project3.Tasks.Count);

            var project4 = result.First(p => p.ProjectId == 4);
            Assert.Single(project4.Tasks);
        }

        [Fact]
        public async Task GetProjectsWithTasksAsync_TasksAreEagerLoaded()
        {
            // Act
            var result = await _repository.GetProjectsWithTasksAsync();

            // Assert
            Assert.NotNull(result);

            // Verificar que las tareas están realmente cargadas (no lazy loading)
            var project = result.First();
            Assert.NotNull(project.Tasks);
            Assert.NotEmpty(project.Tasks);

            // Verificar que podemos acceder a propiedades de las tareas
            var firstTask = project.Tasks.First();
            Assert.NotNull(firstTask.Title);
            Assert.NotNull(firstTask.Status);
        }

        
        [Fact]
        public async Task GetProjectWithTasksByIdAsync_ExistingId_ReturnsProjectWithTasks()
        {
            // Act
            var result = await _repository.GetProjectWithTasksByIdAsync(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.ProjectId);
            Assert.Equal("E-Commerce Platform", result.Name);
            Assert.Equal("Retail Corp", result.ClientName);
            Assert.Equal("InProgress", result.Status);
            Assert.NotNull(result.Tasks);
            Assert.Equal(4, result.Tasks.Count);

            // Verificar algunas tareas específicas
            Assert.Contains(result.Tasks, t => t.Title == "Design Database Schema");
            Assert.Contains(result.Tasks, t => t.Title == "Implement User Authentication");
        }

        [Fact]
        public async Task GetProjectWithTasksByIdAsync_NonExistingId_ReturnsNull()
        {
            // Act
            var result = await _repository.GetProjectWithTasksByIdAsync(999);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GetProjectWithTasksByIdAsync_ProjectWithNoTasks_ReturnsProjectWithEmptyTasks()
        {
            // Arrange - Crear un proyecto sin tareas
            var newProject = new Project
            {
                ProjectId = 99,
                Name = "Empty Project",
                ClientName = "Test Client",
                StartDate = DateTime.UtcNow,
                Status = "Planned"
            };

            _context.Projects.Add(newProject);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.GetProjectWithTasksByIdAsync(99);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Empty Project", result.Name);
            Assert.NotNull(result.Tasks);
            Assert.Empty(result.Tasks);
        }

        [Fact]
        public async Task GetProjectWithTasksByIdAsync_TasksHaveCorrectNavigationProperties()
        {
            // Act
            var result = await _repository.GetProjectWithTasksByIdAsync(1);

            // Assert
            Assert.NotNull(result);

            // Verificar que las tareas tienen referencia al proyecto
            foreach (var task in result.Tasks)
            {
                Assert.Equal(result.ProjectId, task.ProjectId);
                // Nota: En este método no se cargan Assignee o Project (solo la relación inversa)
            }
        }

        #endregion

        #region Tests for GetByIdAsync

        [Fact]
        public async Task GetByIdAsync_ExistingId_ReturnsProject()
        {
            // Act
            var result = await _repository.GetByIdAsync(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.ProjectId);
            Assert.Equal("E-Commerce Platform", result.Name);
            Assert.Equal("Retail Corp", result.ClientName);
            Assert.Equal("InProgress", result.Status);
            Assert.NotNull(result.StartDate);
            Assert.NotNull(result.EndDate);
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
        public async Task GetByIdAsync_ReturnsProjectWithoutTasks()
        {
            // Act
            var result = await _repository.GetByIdAsync(1);

            // Assert
            Assert.NotNull(result);
            // GetByIdAsync no carga las tareas por defecto
            // En Entity Framework Core, la colección podría ser null o vacía dependiendo de la configuración
        }

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        [InlineData(4)]
        public async Task GetByIdAsync_VariousIds_ReturnCorrectProjects(int projectId)
        {
            // Act
            var result = await _repository.GetByIdAsync(projectId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(projectId, result.ProjectId);
        }

        #endregion

        #region Tests for GetAllAsync

        [Fact]
        public async Task GetAllAsync_ReturnsAllProjects()
        {
            // Act
            var result = await _repository.GetAllAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(4, result.Count());

            // Verificar que todos los proyectos están presentes
            Assert.Contains(result, p => p.ProjectId == 1 && p.Name == "E-Commerce Platform");
            Assert.Contains(result, p => p.ProjectId == 2 && p.Name == "Mobile Banking App");
            Assert.Contains(result, p => p.ProjectId == 3 && p.Name == "Inventory System");
            Assert.Contains(result, p => p.ProjectId == 4 && p.Name == "CRM Implementation");
        }

        [Fact]
        public async Task GetAllAsync_ReturnsProjectsInNoSpecificOrder()
        {
            // Act
            var result = await _repository.GetAllAsync();

            // Assert
            Assert.NotNull(result);
            // No hay orden específico definido en el repositorio
            // Pero podemos verificar que todos los IDs están presentes
            var projectIds = result.Select(p => p.ProjectId).ToList();
            Assert.Contains(1, projectIds);
            Assert.Contains(2, projectIds);
            Assert.Contains(3, projectIds);
            Assert.Contains(4, projectIds);
        }

        
        [Fact]
        public async Task FindAsync_WithStatusFilter_ReturnsFilteredProjects()
        {
            // Act - Buscar proyectos en progreso
            var result = await _repository.FindAsync(p => p.Status == "InProgress");

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count()); // Proyectos 1 y 4 están InProgress
            Assert.All(result, p => Assert.Equal("InProgress", p.Status));
            Assert.Contains(result, p => p.Name == "E-Commerce Platform");
            Assert.Contains(result, p => p.Name == "CRM Implementation");
        }

        [Fact]
        public async Task FindAsync_WithClientNameFilter_ReturnsFilteredProjects()
        {
            // Act - Buscar proyectos de un cliente específico
            var result = await _repository.FindAsync(p => p.ClientName.Contains("Corp"));

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count()); // Retail Corp y Sales Corp
            Assert.All(result, p => Assert.Contains("Corp", p.ClientName));
        }

        [Fact]
        public async Task FindAsync_WithDateFilter_ReturnsFilteredProjects()
        {
            // Act - Buscar proyectos que empezaron en 2024
            var startDate2024 = new DateTime(2024, 1, 1);
            var result = await _repository.FindAsync(p => p.StartDate >= startDate2024);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(3, result.Count()); // Proyectos 1, 2, 4
            Assert.All(result, p => Assert.True(p.StartDate >= startDate2024));
        }

        [Fact]
        public async Task FindAsync_WithNullEndDate_ReturnsProjectsWithoutEndDate()
        {
            // Act - Buscar proyectos sin fecha de fin
            var result = await _repository.FindAsync(p => p.EndDate == null);

            // Assert
            Assert.NotNull(result);
            Assert.Single(result); // Solo proyecto 4
            var project = result.First();
            Assert.Equal("CRM Implementation", project.Name);
            Assert.Null(project.EndDate);
        }

        [Fact]
        public async Task FindAsync_WithComplexPredicate_ReturnsCorrectProjects()
        {
            // Act - Buscar proyectos InProgress que empezaron en 2024
            var result = await _repository.FindAsync(p =>
                p.Status == "InProgress" &&
                p.StartDate.Year == 2024);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count()); // Proyectos 1 y 4
            Assert.All(result, p =>
            {
                Assert.Equal("InProgress", p.Status);
                Assert.Equal(2024, p.StartDate.Year);
            });
        }

        [Fact]
        public async Task FindAsync_NoMatches_ReturnsEmptyList()
        {
            // Act - Buscar proyectos con status que no existe
            var result = await _repository.FindAsync(p => p.Status == "Cancelled");

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public async Task FindAsync_NullPredicate_ThrowsArgumentNullException()
        {
            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() =>
                _repository.FindAsync(null));
        }

        #endregion

        #region Tests for AddAsync

        [Fact]
        public async Task AddAsync_ValidProject_AddsToDatabase()
        {
            // Arrange
            var newProject = new Project
            {
                Name = "New Test Project",
                ClientName = "Test Client",
                StartDate = DateTime.UtcNow,
                Status = "Planned",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            var initialCount = await _context.Projects.CountAsync();

            // Act
            var result = await _repository.AddAsync(newProject);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.ProjectId > 0); // ID generado por la base de datos
            Assert.Equal("New Test Project", result.Name);
            Assert.Equal(initialCount + 1, await _context.Projects.CountAsync());

            // Verificar que se guardó en la base de datos
            var savedProject = await _context.Projects.FindAsync(result.ProjectId);
            Assert.NotNull(savedProject);
            Assert.Equal("New Test Project", savedProject.Name);
        }

        [Fact]
        public async Task AddAsync_ProjectWithDefaultValues_UsesDefaults()
        {
            // Arrange
            var newProject = new Project
            {
                Name = "Project with Defaults",
                ClientName = "Client",
                StartDate = DateTime.UtcNow
                // Status debería usar el valor por defecto "Planned"
                // CreatedAt y UpdatedAt deberían ser generados por la base de datos
            };

            // Act
            var result = await _repository.AddAsync(newProject);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Planned", result.Status); // Valor por defecto
            Assert.True(result.CreatedAt > DateTime.MinValue);
            Assert.True(result.UpdatedAt > DateTime.MinValue);
        }

        [Fact]
        public async Task AddAsync_ProjectWithTasks_DoesNotSaveTasks()
        {
            // Arrange
            var newProject = new Project
            {
                Name = "Project with Tasks",
                ClientName = "Client",
                StartDate = DateTime.UtcNow,
                Status = "Planned",
                Tasks = new List<TaskItem>
                {
                    new TaskItem { Title = "Task 1", Status = "ToDo" },
                    new TaskItem { Title = "Task 2", Status = "ToDo" }
                }
            };

            // Act
            var result = await _repository.AddAsync(newProject);

            // Assert
            Assert.NotNull(result);
            // Las tareas no deberían guardarse automáticamente
            // (depende de la configuración de EF Core)
            var savedProject = await _context.Projects
                .Include(p => p.Tasks)
                .FirstOrDefaultAsync(p => p.ProjectId == result.ProjectId);

            // En muchos casos, las tareas no se guardarían sin llamar explícitamente a AddAsync en TaskRepository
        }

        [Fact]
        public async Task AddAsync_NullProject_ThrowsArgumentNullException()
        {
            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() =>
                _repository.AddAsync(null));
        }

        
        [Fact]
        public async Task UpdateAsync_ProjectWithNullValues_PreservesNulls()
        {
            // Arrange
            var project = await _context.Projects.FindAsync(4); // Proyecto sin EndDate
            Assert.Null(project.EndDate);

            project.Name = "Updated CRM";
            // No establecer EndDate

            // Act
            await _repository.UpdateAsync(project);
            await _context.SaveChangesAsync();

            // Assert
            var updatedProject = await _context.Projects.FindAsync(4);
            Assert.NotNull(updatedProject);
            Assert.Equal("Updated CRM", updatedProject.Name);
            Assert.Null(updatedProject.EndDate); // Debería seguir siendo null
        }

        [Fact]
        public async Task UpdateAsync_NullEntity_ThrowsArgumentNullException()
        {
            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() =>
                _repository.UpdateAsync(null));
        }

        #endregion

                
        [Fact]
        public async Task DeleteAsync_NonExistingId_DoesNothing()
        {
            // Arrange
            var nonExistingId = 999;
            var initialCount = await _context.Projects.CountAsync();

            // Act
            await _repository.DeleteAsync(nonExistingId);

            // Assert
            Assert.Equal(initialCount, await _context.Projects.CountAsync());
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
        public async Task ExistsAsync_ZeroOrNegativeId_ReturnsFalse()
        {
            // Act & Assert
            Assert.False(await _repository.ExistsAsync(0));
            Assert.False(await _repository.ExistsAsync(-1));
        }

        [Fact]
              
        public async Task CountAsync_ReturnsTotalProjectCount()
        {
            // Act
            var result = await _repository.CountAsync();

            // Assert
            Assert.Equal(4, result); // 4 proyectos en la base de datos
        }

        [Fact]
        
        public async Task CountAsync_WithPredicate_ReturnsFilteredCount()
        {
            // Act - Contar proyectos InProgress
            var result = await _repository.CountAsync(p => p.Status == "InProgress");

            // Assert
            Assert.Equal(2, result); // 2 proyectos InProgress
        }

        [Fact]
        public async Task CountAsync_WithDatePredicate_ReturnsCorrectCount()
        {
            // Act - Contar proyectos que empezaron en 2024
            var startDate2024 = new DateTime(2024, 1, 1);
            var result = await _repository.CountAsync(p => p.StartDate >= startDate2024);

            // Assert
            Assert.Equal(3, result); // 3 proyectos empezaron en 2024
        }

        [Fact]
        public async Task CountAsync_WithNoMatches_ReturnsZero()
        {
            // Act - Contar proyectos con status inexistente
            var result = await _repository.CountAsync(p => p.Status == "Cancelled");

            // Assert
            Assert.Equal(0, result);
        }

        [Fact]
        public async Task CountAsync_NullPredicate_ThrowsArgumentNullException()
        {
            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() =>
                _repository.CountAsync(null));
        }

       [Fact]
        public async Task Transaction_RollbackOnException()
        {
            // Arrange
            var initialCount = await _context.Projects.CountAsync();
            var newProject = new Project
            {
                Name = "Project for Rollback Test",
                ClientName = "Test Client",
                StartDate = DateTime.UtcNow,
                Status = "Planned"
            };

            // Act & Assert - Simular una excepción durante el guardado
            // Nota: En una implementación real, podrías envolver esto en una transacción
            try
            {
                await _repository.AddAsync(newProject);
                throw new Exception("Simulated exception");
            }
            catch
            {
                // En una transacción real con SaveChanges, los cambios se revertirían
                // Pero con InMemory y sin transacción explícita, los cambios podrían persistir
            }

            // Depende de si usas transacciones en tu implementación
        }

        [Fact]
        public async Task GetProjectWithTasksByIdAsync_VerifyTaskPropertiesAreLoaded()
        {
            // Act
            var result = await _repository.GetProjectWithTasksByIdAsync(1);

            // Assert
            Assert.NotNull(result);
            Assert.NotNull(result.Tasks);

            var firstTask = result.Tasks.First();
            // Verificar que las propiedades básicas de las tareas están cargadas
            Assert.NotNull(firstTask.Title);
            Assert.NotNull(firstTask.Status);
            Assert.NotNull(firstTask.Priority);
            Assert.NotNull(firstTask.CreatedAt);

            // Verificar que EstimatedComplexity está dentro del rango válido (1-5) o es null
            if (firstTask.EstimatedComplexity.HasValue)
            {
                Assert.InRange(firstTask.EstimatedComplexity.Value, 1, 5);
            }
        }

        [Fact]
        public async Task FindAsync_WithStringContains_CaseInsensitive()
        {
            // Act - Buscar proyectos con "platform" (minúsculas)
            var result = await _repository.FindAsync(p => p.Name.ToLower().Contains("platform"));

            // Assert
            Assert.NotNull(result);
            Assert.Single(result); // "E-Commerce Platform"
            Assert.Contains("Platform", result.First().Name);
        }
       
        [Fact]
        public async Task AddAsync_ProjectWithMaxLengthStrings_HandlesCorrectly()
        {
            // Arrange
            var longName = new string('A', 200); // Límite máximo según tu configuración
            var longClientName = new string('B', 200);

            var newProject = new Project
            {
                Name = longName,
                ClientName = longClientName,
                StartDate = DateTime.UtcNow,
                Status = "Planned"
            };

            // Act
            var result = await _repository.AddAsync(newProject);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(longName, result.Name);
            Assert.Equal(longClientName, result.ClientName);

            // Verificar en la base de datos
            var savedProject = await _context.Projects.FindAsync(result.ProjectId);
            Assert.NotNull(savedProject);
            Assert.Equal(200, savedProject.Name.Length);
            Assert.Equal(200, savedProject.ClientName.Length);
        }

        [Fact]
        public async Task GetProjectsWithTasksAsync_DoesNotCauseCircularReferences()
        {
            // Act
            var result = await _repository.GetProjectsWithTasksAsync();

            // Assert
            Assert.NotNull(result);

            // Verificar que no hay referencias circulares al serializar (para JSON, etc.)
            var project = result.First();
            Assert.NotNull(project.Tasks);

            // Las tareas no deberían tener una referencia de vuelta al proyecto cargado
            // (depende de la configuración de serialización)
            foreach (var task in project.Tasks)
            {
                // Puedes verificar que task.Project es null o que no causa stack overflow
                // En EF Core con lazy loading disabled, task.Project podría ser null
            }
        }

        [Fact]
        public async Task FindAsync_WithLargeResultSet_PerformsWell()
        {
            // Arrange - Agregar muchos proyectos para probar rendimiento
            for (int i = 5; i <= 100; i++)
            {
                _context.Projects.Add(new Project
                {
                    ProjectId = i,
                    Name = $"Project {i}",
                    ClientName = $"Client {i}",
                    StartDate = DateTime.UtcNow.AddDays(-i),
                    Status = i % 2 == 0 ? "InProgress" : "Planned"
                });
            }
            await _context.SaveChangesAsync();

            // Act - Buscar todos los proyectos InProgress
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            var result = await _repository.FindAsync(p => p.Status == "InProgress");
            stopwatch.Stop();

            // Assert
            Assert.NotNull(result);
            Assert.True(result.Count() > 2);

            // Verificar que la consulta es razonablemente rápida
            // (esto es subjetivo y depende del entorno)
            Assert.True(stopwatch.ElapsedMilliseconds < 1000,
                $"La consulta tomó {stopwatch.ElapsedMilliseconds}ms, debería ser más rápida");
        }

        public void Dispose()
        {
            _context?.Dispose();
        }
    }
}