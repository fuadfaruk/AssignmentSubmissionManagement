using System;
using System.Collections.Generic;
using System.Text;

namespace AssignmentSubmissionManagement.Core.DTOs.Submissions
{
    public class CreateSubmissionRequest
    {
        public Guid AssignmentId { get; set; }
        public string? TextContent { get; set; }
    }
}
