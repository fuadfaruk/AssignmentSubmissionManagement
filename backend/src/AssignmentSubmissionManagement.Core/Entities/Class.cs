namespace AssignmentSubmissionManagement.Core.Entities
{
    public class Class
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation property for the related assignments
        public ICollection<ClassTeacher> ClassTeachers { get; set; } = new List<ClassTeacher>();
        public ICollection<ClassStudent> ClassStudents { get; set; } = new List<ClassStudent>();
        public ICollection<Assignment> Assignments { get; set; } = new List<Assignment>();
    }
}
