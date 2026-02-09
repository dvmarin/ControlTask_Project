namespace ControlTask.Domain.Entities
{
    public class Developer
    {
        public int DeveloperId { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation property
        public ICollection<TaskItem> Tasks { get; set; } = new List<TaskItem>();
    }
}
