using AssignmentSubmissionManagement.Core.DTOs.Auth;
using System;
using System.Collections.Generic;
using System.Text;

namespace AssignmentSubmissionManagement.Core.Interfaces.Services
{
    public interface IAuthService
    {
        Task<LoginResponse?> LoginAsync(LoginRequest request);
        string HashPassword(string password);
        bool VerifyPassword(string password, string hash);
    }
}
