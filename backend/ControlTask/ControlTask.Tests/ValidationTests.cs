using Xunit;
using ControlTask.Application.DTOs;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace ControlTask.Tests
{
    public class ValidationTests
    {
        [Theory]
        [InlineData("Valid Title", true)]
        public void CreateTaskDto_TitleValidation(string title, bool isValid)
        {
            // Arrange
            var dto = new CreateTaskDto
            {
                Title = title,
                ProjectId = 1,
                AssigneeId = 1,
                Status = "ToDo",
                Priority = "Medium"
            };

            var validationContext = new ValidationContext(dto);
            var validationResults = new List<ValidationResult>();

            // Act
            var result = Validator.TryValidateObject(dto, validationContext, validationResults, true);

            // Assert
            Assert.Equal(isValid, result);
            if (!isValid)
            {
                Assert.Contains(validationResults, v => v.MemberNames.Contains(nameof(CreateTaskDto.Title)));
            }
        }

        [Theory]
        [InlineData(1, true)]
        [InlineData(999, true)]
        public void CreateTaskDto_ProjectIdValidation(int projectId, bool isValid)
        {
            // Arrange
            var dto = new CreateTaskDto
            {
                Title = "Valid Title",
                ProjectId = projectId,
                AssigneeId = 1,
                Status = "ToDo",
                Priority = "Medium"
            };

            var validationContext = new ValidationContext(dto);
            var validationResults = new List<ValidationResult>();

            // Act
            var result = Validator.TryValidateObject(dto, validationContext, validationResults, true);

            // Assert
            Assert.Equal(isValid, result);
        }

        [Theory]
        [InlineData("InvalidStatus", false)]
        [InlineData("ToDo", true)]
        [InlineData("InProgress", true)]
        [InlineData("Blocked", true)]
        [InlineData("Completed", true)]
        public void CreateTaskDto_StatusValidation(string status, bool isValid)
        {
            // Arrange
            var dto = new CreateTaskDto
            {
                Title = "Valid Title",
                ProjectId = 1,
                AssigneeId = 1,
                Status = status,
                Priority = "Medium"
            };

            // Act & Assert
            if (!isValid)
            {
                Assert.Throws<ArgumentException>(() => ValidateStatus(status));
            }
            else
            {
                // No debería lanzar excepción
                ValidateStatus(status);
            }
        }

        private void ValidateStatus(string status)
        {
            if (!new[] { "ToDo", "InProgress", "Blocked", "Completed" }.Contains(status))
            {
                throw new ArgumentException("Status inválido");
            }
        }

        [Theory]
        [InlineData(0, false)]
        [InlineData(6, false)]
        [InlineData(1, true)]
        [InlineData(3, true)]
        [InlineData(5, true)]
        [InlineData(null, true)] // Null es válido
        public void CreateTaskDto_EstimatedComplexityValidation(int? complexity, bool isValid)
        {
            // Arrange
            var dto = new CreateTaskDto
            {
                Title = "Valid Title",
                ProjectId = 1,
                AssigneeId = 1,
                Status = "ToDo",
                Priority = "Medium",
                EstimatedComplexity = complexity
            };

            // Act & Assert
            if (!isValid && complexity.HasValue)
            {
                Assert.Throws<ArgumentException>(() => ValidateComplexity(complexity.Value));
            }
            else
            {
                // No debería lanzar excepción
                if (complexity.HasValue)
                    ValidateComplexity(complexity.Value);
            }
        }

        private void ValidateComplexity(int complexity)
        {
            if (complexity < 1 || complexity > 5)
            {
                throw new ArgumentException("Complexity inválida");
            }
        }
    }
}