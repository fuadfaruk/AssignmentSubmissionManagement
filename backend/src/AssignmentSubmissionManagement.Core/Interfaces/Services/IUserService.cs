using AssignmentSubmissionManagement.Core.DTOs.Users;
using AssignmentSubmissionManagement.Core.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace AssignmentSubmissionManagement.Core.Interfaces.Services
{
    public interface IUserService
    {
        Task<UserResponse?> GetByIdAsync(Guid userId);
        Task<IEnumerable<UserResponse>> GetAllAsync(UserRole? role = null);
        Task<UserResponse> CreateAsync(CreateUserRequest request);
        Task<UserResponse> UpdateAsync(Guid id, UpdateUserRequest request);
        Task DeleteAsync(Guid id);
    }
}
