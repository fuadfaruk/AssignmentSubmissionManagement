using AssignmentSubmissionManagement.Core.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace AssignmentSubmissionManagement.Core.DTOs.Submissions
{
    public class SubmissionResponse
    {
        public Guid Id { get; set; }
        public Guid AssignmentId { get; set; }
        public string AssignmentTitle { get; set; } = string.Empty;
        public Guid StudentId { get; set; }
        public string StudentName { get; set; } = string.Empty;
        public string? TextContent { get; set; }
        public string? FileName { get; set; }
        public string? FilePath { get; set; }
        public SubmissionStatus Status { get; set; }
        public int? Marks { get; set; }
        public string? Feedback { get; set; }
        public DateTime? SubmittedAt { get; set; }
        public DateTime? GradedAt { get; set; }
    }
}
