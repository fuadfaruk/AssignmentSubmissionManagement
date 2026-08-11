namespace AssignmentSubmissionManagement.Core.Entities
{
    public class Assignment
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime DueDate { get; set; }
        public int MaxMarks { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        // Foreign key to the Course entity
        public Guid ClassId { get; set; }
        public Class Class { get; set; } = null!;

        public Guid TeacherId { get; set; }
        public User Teacher { get; set; } = null!;

        // Navigation property for the related submissions
        public ICollection<Submission> Submissions { get; set; } = new List<Submission>();
    }
}
