using ControlTask.API.Middleware;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using Xunit;

namespace ControlTask.Tests.Middleware
{
    public class ExceptionHandlingMiddlewareTests
    {
        [Theory]
        [InlineData(typeof(KeyNotFoundException), 404)]
        [InlineData(typeof(ArgumentException), 400)]
        [InlineData(typeof(UnauthorizedAccessException), 401)]
        [InlineData(typeof(Exception), 500)]
        public async Task Middleware_ReturnsExpectedStatusCodeAndJson(Type exceptionType, int expectedStatusCode)
        {
            // Arrange
            var loggerMock = new Mock<ILogger<ExceptionHandlingMiddleware>>();
            var envMock = new Mock<IWebHostEnvironment>();
            envMock.Setup(e => e.EnvironmentName).Returns("Development"); // Dev mode

            var context = new DefaultHttpContext();
            context.Response.Body = new MemoryStream();

            RequestDelegate next = _ => throw (Exception)Activator.CreateInstance(exceptionType, "Test exception")!;

            var middleware = new ExceptionHandlingMiddleware(next, loggerMock.Object, envMock.Object);

            // Act
            await middleware.InvokeAsync(context);

            // Leer el response
            context.Response.Body.Seek(0, SeekOrigin.Begin);
            var responseBody = await new StreamReader(context.Response.Body).ReadToEndAsync();

            var responseJson = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(responseBody, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            // Assert
            Assert.Equal(expectedStatusCode, context.Response.StatusCode);
            Assert.Equal("Test exception", responseJson["message"].GetString());
            Assert.NotNull(responseJson["details"]);
            Assert.True(responseJson.ContainsKey("timestamp"));
        }

        [Fact]
        public async Task Middleware_DoesNotIncludeDetails_WhenNotDevelopment()
        {
            // Arrange
            var loggerMock = new Mock<ILogger<ExceptionHandlingMiddleware>>();
            var envMock = new Mock<IWebHostEnvironment>();
            envMock.Setup(e => e.EnvironmentName).Returns("Production"); // No Dev mode

            var context = new DefaultHttpContext();
            context.Response.Body = new MemoryStream();

            RequestDelegate next = _ => throw new Exception("Prod exception");

            var middleware = new ExceptionHandlingMiddleware(next, loggerMock.Object, envMock.Object);

            // Act
            await middleware.InvokeAsync(context);

            // Leer el response
            context.Response.Body.Seek(0, SeekOrigin.Begin);
            var responseBody = await new StreamReader(context.Response.Body).ReadToEndAsync();

            var responseJson = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(responseBody, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            // Assert
            Assert.Equal(500, context.Response.StatusCode);
            Assert.Equal("Prod exception", responseJson["message"].GetString());
            Assert.True(responseJson.ContainsKey("details"));
            Assert.Equal(JsonValueKind.Null, responseJson["details"].ValueKind); // Detalles solo en desarrollo
        }
    }
}
