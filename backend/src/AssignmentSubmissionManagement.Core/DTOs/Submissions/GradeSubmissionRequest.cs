using System;
using System.Collections.Generic;
using System.Text;

namespace AssignmentSubmissionManagement.Core.DTOs.Submissions
{
    public class GradeSubmissionRequest
    {
        public int Marks { get; set; }
        public string? Feedback { get; set; }
    }
}
