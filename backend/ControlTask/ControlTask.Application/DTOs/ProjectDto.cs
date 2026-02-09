namespace ControlTask.Application.DTOs
{
    public class ProjectDto
    {
        public int ProjectId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string ClientName { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string Status { get; set; } = string.Empty;
        public int TotalTasks { get; set; }
        public int OpenTasks { get; set; }
        public int CompletedTasks { get; set; }
    }

    public class ProjectHealthDto
    {
        public int ProjectId { get; set; }
        public string ProjectName { get; set; } = string.Empty;
        public string ClientName { get; set; } = string.Empty;
        public int TotalTasks { get; set; }
        public int OpenTasks { get; set; }
        public int CompletedTasks { get; set; }
    }
}
