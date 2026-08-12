using AssignmentSubmissionManagement.Core.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace AssignmentSubmissionManagement.Core.DTOs.Users
{
    public class UpdateUserRequest
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public UserRole Role { get; set; }
        /// <summary>
        /// Optional. If provided, the user's password will be updated. If not provided, the password will remain unchanged.
        /// </summary>
        public string? Password { get; set; } = string.Empty;
    }
}
