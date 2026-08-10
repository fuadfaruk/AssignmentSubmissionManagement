using AssignmentSubmissionManagement.Core.Enums;

namespace AssignmentSubmissionManagement.Core.Entities
{
    public class User
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public UserRole Role { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties for related entities
        public ICollection<ClassTeacher> ClassTeachers { get; set; } = new List<ClassTeacher>();
        public ICollection<ClassStudent> ClassStudents { get; set; } = new List<ClassStudent>();
        public ICollection<Assignment> Assignments { get; set; } = new List<Assignment>();
        public ICollection<Submission> Submissions { get; set; } = new List<Submission>();
    }
}
