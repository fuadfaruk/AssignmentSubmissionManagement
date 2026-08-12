using AssignmentSubmissionManagement.Core.DTOs.Users;
using AssignmentSubmissionManagement.Core.Entities;
using AssignmentSubmissionManagement.Core.Enums;
using AssignmentSubmissionManagement.Core.Interfaces.Repositories;
using AssignmentSubmissionManagement.Core.Interfaces.Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace AssignmentSubmissionManagement.Infrastructure.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IAuthService _authService;

        public UserService(IUserRepository userRepository, IAuthService authService)
        {
            _userRepository = userRepository;
            _authService = authService;
        }

        public async Task<UserResponse?> GetByIdAsync(Guid id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            return user is null ? null : MapToResponse(user);
        }

        public async Task<IEnumerable<UserResponse>> GetAllAsync(UserRole? role = null)
        {
            var users = await _userRepository.GetAllAsync(role);
            return users.Select(MapToResponse);
        }

        public async Task<UserResponse> CreateAsync(CreateUserRequest request)
        {
            if (await _userRepository.EmailExistsAsync(request.Email))
                throw new InvalidOperationException($"A user with email '{request.Email}' already exists.");

            var user = new User
            {
                FirstName = request.FirstName,
                LastName = request.LastName,
                Email = request.Email.ToLower(),
                PasswordHash = _authService.HashPassword(request.Password),
                Role = request.Role
            };

            var created = await _userRepository.CreateAsync(user);
            return MapToResponse(created);
        }

        public async Task<UserResponse> UpdateAsync(Guid id, UpdateUserRequest request)
        {
            var user = await _userRepository.GetByIdAsync(id)
                ?? throw new KeyNotFoundException($"User with id '{id}' not found.");

            if (await _userRepository.EmailExistsAsync(request.Email, id))
                throw new InvalidOperationException($"A user with email '{request.Email}' already exists.");

            user.FirstName = request.FirstName;
            user.LastName = request.LastName;
            user.Email = request.Email.ToLower();
            user.Role = request.Role;

            if (!string.IsNullOrWhiteSpace(request.Password))
                user.PasswordHash = _authService.HashPassword(request.Password);

            var updated = await _userRepository.UpdateAsync(user);
            return MapToResponse(updated);
        }

        public async Task DeleteAsync(Guid id)
        {
            if (!await _userRepository.ExistsAsync(id))
                throw new KeyNotFoundException($"User with id '{id}' not found.");

            await _userRepository.DeleteAsync(id);
        }

        private static UserResponse MapToResponse(User user) => new()
        {
            Id = user.Id,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Email = user.Email,
            Role = user.Role,
            CreatedAt = user.CreatedAt
        };

    }
}
