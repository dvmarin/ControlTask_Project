namespace ControlTask.Application.DTOs
{
    public class TaskDto
    {
        public int TaskId { get; set; }
        public int ProjectId { get; set; }
        public string ProjectName { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int AssigneeId { get; set; }
        public string AssigneeName { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string Priority { get; set; } = string.Empty;
        public int? EstimatedComplexity { get; set; }
        public DateTime? DueDate { get; set; }
        public DateTime? CompletionDate { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class CreateTaskDto
    {
        public int ProjectId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int AssigneeId { get; set; }
        public string Status { get; set; } = "ToDo";
        public string Priority { get; set; } = "Medium";
        public int? EstimatedComplexity { get; set; }
        public DateTime? DueDate { get; set; }
    }

    public class UpdateTaskStatusDto
    {
        public string Status { get; set; } = string.Empty;
        public string? Priority { get; set; }
        public int? EstimatedComplexity { get; set; }
    }

    public class UpcomingTaskDto
    {
        public string Title { get; set; } = string.Empty;
        public string ProjectName { get; set; } = string.Empty;
        public string AssignedTo { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string Priority { get; set; } = string.Empty;
        public DateTime DueDate { get; set; }
        public int DaysUntilDue { get; set; }
    }
}
