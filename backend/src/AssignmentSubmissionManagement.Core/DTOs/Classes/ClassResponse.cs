using AssignmentSubmissionManagement.Core.DTOs.Users;
using System;
using System.Collections.Generic;
using System.Text;

namespace AssignmentSubmissionManagement.Core.DTOs.Classes
{
    public class ClassResponse
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public List<UserResponse> Teachers { get; set; } = new();
        public List<UserResponse> Students { get; set; } = new();
    }
}
