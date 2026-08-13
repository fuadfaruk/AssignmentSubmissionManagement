using System;
using System.Collections.Generic;
using System.Text;

namespace AssignmentSubmissionManagement.Core.DTOs.Assignments
{
    public class AssignmentResponse
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime DueDate { get; set; }
        public int MaxMarks { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public Guid ClassId { get; set; }
        public string ClassName { get; set; } = string.Empty;
        public Guid TeacherId { get; set; }
        public string TeacherName { get; set; } = string.Empty;
        public int SubmissionCount { get; set; }
    }
}
