namespace ControlTask.Application.DTOs
{
    public class DeveloperDto
    {
        public int DeveloperId { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public bool IsActive { get; set; }
    }

    public class DeveloperWorkloadDto
    {
        public string DeveloperName { get; set; } = string.Empty;
        public int OpenTasksCount { get; set; }
        public decimal AverageEstimatedComplexity { get; set; }
    }

    public class DeveloperDelayRiskDto
    {
        public string DeveloperName { get; set; } = string.Empty;
        public int OpenTasksCount { get; set; }
        public decimal AvgDelayDays { get; set; }
        public DateTime? NearestDueDate { get; set; }
        public DateTime? LatestDueDate { get; set; }
        public DateTime? PredictedCompletionDate { get; set; }
        public bool HighRiskFlag { get; set; }
    }
}
