using AssignmentSubmissionManagement.Core.Enums;

namespace AssignmentSubmissionManagement.Core.Entities
{
    public class Submission
    {
        public Guid Id { get; set; }
        public string? TextContent { get; set; }
        public string? FilePath { get; set; }
        public string? FileName { get; set; }
        public SubmissionStatus Status { get; set; } = SubmissionStatus.NotSubmitted;
        public int? Marks { get; set; }
        public string? Feedback { get; set; }
        public DateTime? SubmittedAt { get; set; }
        public DateTime? GradedAt { get; set; }

        // Foreign key to the Assignment entity
        public Guid AssignmentId { get; set; }
        public Assignment Assignment { get; set; } = null!;

        public Guid StudentId { get; set; }
        public User Student { get; set; } = null!;
    }
}
