using ControlTask.Application.Interfaces;
using ControlTask.Application.Mappings;
using ControlTask.Application.Services;
using ControlTask.Domain.Interfaces;
using ControlTask.Infrastructure.Persistence;
using ControlTask.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
// Registrar DbContext con SQL Server y la cadena de conexión
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Registrar servicios de aplicación
builder.Services.AddScoped<IDashboardQuery, DashboardQuery>();
builder.Services.AddScoped<IDeveloperRepository, DeveloperRepository>();
builder.Services.AddScoped<IProjectRepository, ProjectRepository>();
builder.Services.AddScoped<ITaskService, TaskService>();

// Registrar repositorios 
builder.Services.AddScoped<IDashboardService, DashboardService>();
builder.Services.AddScoped<IDeveloperService, DeveloperService>();
builder.Services.AddScoped<IProjectService, ProjectService>();
builder.Services.AddScoped<ITaskRepository,TaskRepository>();


// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddAutoMapper(typeof(MappingProfile).Assembly);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy", policy =>
    {
        policy
            .AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader();
    });
});
builder.Configuration.AddJsonFile("appsettings.Docker.json", optional: true);

// Modificar la configuración de CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("DockerPolicy", policy =>
    {
        policy.WithOrigins(
                "http://localhost:4200",
                "http://frontend:80"
            )
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials();
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("DockerPolicy");
app.UseCors("CorsPolicy");
app.UseAuthorization();

app.MapControllers();

app.Run();
