using System;
using System.Collections.Generic;
using System.Text;

namespace AssignmentSubmissionManagement.Core.DTOs.Assignments
{
    public class CreateAssignmentRequest
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime DueDate { get; set; }
        public int MaxMarks { get; set; }
        public Guid ClassId { get; set; }
    }
}
