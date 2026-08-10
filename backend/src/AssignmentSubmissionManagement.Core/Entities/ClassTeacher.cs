namespace AssignmentSubmissionManagement.Core.Entities
{
    /// <summary>
    /// Join table between Class and Teacher entities, representing the many-to-many relationship between them (User with Role = Teacher).
    /// </summary>
    public class ClassTeacher
    {
        public Guid ClassId { get; set; }
        public Class Class { get; set; } = null!;

        public Guid TeacherId { get; set; }
        public User Teacher { get; set; } = null!;

        public DateTime AssignedAt { get; set; } = DateTime.UtcNow;
    }
}
