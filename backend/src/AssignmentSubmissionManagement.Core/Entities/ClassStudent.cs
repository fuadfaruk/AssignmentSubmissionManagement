namespace AssignmentSubmissionManagement.Core.Entities
{
    /// <summary>
    /// Join table between Class and Student entities, representing the many-to-many relationship between them (User with Role = Student).
    /// </summary>
    public class ClassStudent
    {
        public Guid ClassId { get; set; }
        public Class Class { get; set; } = null!;

        public Guid StudentId { get; set; }
        public User Student { get; set; } = null!;

        public DateTime EnrolledAt { get; set; } = DateTime.UtcNow;
    }
}
