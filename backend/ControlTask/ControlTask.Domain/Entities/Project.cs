namespace ControlTask.Domain.Entities
{
    public class Project
    {
        public int ProjectId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string ClientName { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string Status { get; set; } = "Planned"; // Planned, InProgress, Completed
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Navigation property
        public ICollection<TaskItem> Tasks { get; set; } = new List<TaskItem>();
    }
}
