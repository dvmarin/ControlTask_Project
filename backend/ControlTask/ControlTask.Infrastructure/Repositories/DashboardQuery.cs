using ControlTask.Application.DTOs;
using ControlTask.Application.Interfaces;
using ControlTask.Infrastructure.Persistence;
using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace ControlTask.Infrastructure.Repositories
{
    public class DashboardQuery : IDashboardQuery
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;

        public DashboardQuery(ApplicationDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        public async Task<IEnumerable<DeveloperDelayRiskDto>> GetDeveloperDelayRiskAsync()
        {
            var connectionString = _configuration.GetConnectionString("DefaultConnection");

            await using var connection = new SqlConnection(connectionString);

            var sql = @"
            WITH DeveloperCompletedTasks AS (
                SELECT 
                    t.AssigneeId,
                    AVG(CASE 
                        WHEN t.CompletionDate > t.DueDate 
                        THEN DATEDIFF(DAY, t.DueDate, t.CompletionDate)
                        ELSE 0 
                    END) AS AvgDelayDays
                FROM app.Tasks t
                WHERE t.Status = 'Completed'
                    AND t.CompletionDate IS NOT NULL
                    AND t.DueDate IS NOT NULL
                GROUP BY t.AssigneeId
            ),
            DeveloperOpenTasks AS (
                SELECT 
                    t.AssigneeId,
                    COUNT(*) AS OpenTasksCount,
                    MIN(t.DueDate) AS NearestDueDate,
                    MAX(t.DueDate) AS LatestDueDate
                FROM app.Tasks t
                WHERE t.Status <> 'Completed'
                    AND t.DueDate IS NOT NULL
                GROUP BY t.AssigneeId
            )
            SELECT 
                CONCAT(d.FirstName, ' ', d.LastName) AS DeveloperName,
                ISNULL(dot.OpenTasksCount, 0) AS OpenTasksCount,
                ISNULL(dct.AvgDelayDays, 0) AS AvgDelayDays,
                dot.NearestDueDate,
                dot.LatestDueDate,
                CASE 
                    WHEN dot.LatestDueDate IS NOT NULL
                    THEN DATEADD(DAY, ISNULL(dct.AvgDelayDays, 0), dot.LatestDueDate)
                    ELSE NULL 
                END AS PredictedCompletionDate,
                CASE 
                    WHEN (ISNULL(dct.AvgDelayDays, 0) > 3) 
                        OR (DATEADD(DAY, ISNULL(dct.AvgDelayDays, 0), ISNULL(dot.LatestDueDate, GETDATE())) > ISNULL(dot.LatestDueDate, GETDATE()))
                    THEN 1 
                    ELSE 0 
                END AS HighRiskFlag
            FROM app.Developers d
            LEFT JOIN DeveloperCompletedTasks dct ON d.DeveloperId = dct.AssigneeId
            LEFT JOIN DeveloperOpenTasks dot ON d.DeveloperId = dot.AssigneeId
            WHERE d.IsActive = 1
            ORDER BY HighRiskFlag DESC, OpenTasksCount DESC";

            return await connection.QueryAsync<DeveloperDelayRiskDto>(sql);
        }

        public async Task<IEnumerable<DeveloperWorkloadDto>> GetDeveloperWorkloadAsync()
        {
            return await _context.Developers
                .Where(d => d.IsActive)
                .Select(d => new DeveloperWorkloadDto
                {
                    DeveloperName = d.FirstName + " " + d.LastName,
                    OpenTasksCount = d.Tasks.Count(t => t.Status != "Completed"),
                    AverageEstimatedComplexity =
                        d.Tasks.Where(t => t.Status != "Completed" && t.EstimatedComplexity.HasValue)
                               .Average(t => (decimal?)t.EstimatedComplexity) ?? 0
                })
                .ToListAsync();
        }

        public async Task<IEnumerable<ProjectHealthDto>> GetProjectHealthAsync()
        {
            return await _context.Projects
               .Select(p => new ProjectHealthDto
               {
                   ProjectId = p.ProjectId,
                   ProjectName = p.Name,
                   ClientName = p.ClientName,
                   TotalTasks = p.Tasks.Count,
                   OpenTasks = p.Tasks.Count(t => t.Status != "Completed"),
                   CompletedTasks = p.Tasks.Count(t => t.Status == "Completed")
               })
               .ToListAsync();
        }

        public async Task<IEnumerable<UpcomingTaskDto>> GetUpcomingTasksAsync(int days)
        {
            var today = DateTime.UtcNow.Date;
            var targetDate = today.AddDays(days);

            return await _context.Tasks
                .Include(t => t.Project)
                .Include(t => t.Assignee)
                .Where(t => t.Status != "Completed" &&
                            t.DueDate.HasValue &&
                            t.DueDate.Value.Date >= today &&
                            t.DueDate.Value.Date <= targetDate)
                .OrderBy(t => t.DueDate)
                .Select(t => new UpcomingTaskDto
                {
                    Title = t.Title,
                    ProjectName = t.Project.Name,
                    AssignedTo = t.Assignee.FirstName + " " + t.Assignee.LastName,
                    Status = t.Status,
                    Priority = t.Priority,
                    DueDate = t.DueDate.Value,
                    DaysUntilDue = (t.DueDate.Value.Date - today).Days
                })
                .ToListAsync();
        }
    }
}
