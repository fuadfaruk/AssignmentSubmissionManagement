using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using AssignmentSubmissionManagement.Core.DTOs.Auth;
using AssignmentSubmissionManagement.Core.Entities;
using AssignmentSubmissionManagement.Core.Enums;
using AssignmentSubmissionManagement.Core.Interfaces.Repositories;
using AssignmentSubmissionManagement.Infrastructure.Services;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Moq;

namespace AssignmentSubmissionManagement.Tests;

public class RoleAuthorizationTests
{
    private readonly Mock<IUserRepository> _userRepoMock = new();
    private readonly string _jwtSecret = "SuperSecretKeyForAssignmentSubmissionManagement123!";

    private AuthService CreateAuthService()
    {
        var inMemorySettings = new Dictionary<string, string?>
        {
            {"Jwt:Key", _jwtSecret},
            {"Jwt:Issuer", "AssignmentApi"},
            {"Jwt:Audience", "AssignmentClient"},
            {"Jwt:ExpiryMinutes", "60"}
        };

        IConfiguration configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(inMemorySettings)
            .Build();

        return new AuthService(_userRepoMock.Object, configuration);
    }

    [Theory]
    [InlineData(UserRole.Admin, "Admin")]
    [InlineData(UserRole.Teacher, "Teacher")]
    [InlineData(UserRole.Student, "Student")]
    public async Task LoginAsync_ReturnsTokenWithCorrectRoleClaim(UserRole role, string expectedRoleString)
    {
        // Arrange
        var authService = CreateAuthService();
        var rawPassword = "Password123!";
        var passwordHash = authService.HashPassword(rawPassword);

        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = $"{expectedRoleString.ToLower()}@school.edu",
            FirstName = "Test",
            LastName = "User",
            PasswordHash = passwordHash,
            Role = role
        };

        _userRepoMock.Setup(r => r.GetByEmailAsync(user.Email)).ReturnsAsync(user);

        var loginRequest = new LoginRequest
        {
            Email = user.Email,
            Password = rawPassword
        };

        // Act
        var response = await authService.LoginAsync(loginRequest);

        // Assert
        response.Should().NotBeNull();
        response!.Token.Should().NotBeNullOrEmpty();
        response.Role.Should().Be(role);

        var handler = new JwtSecurityTokenHandler();
        var jwtToken = handler.ReadJwtToken(response.Token);

        var roleClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role || c.Type == "role");
        roleClaim.Should().NotBeNull();
        roleClaim!.Value.Should().Be(expectedRoleString);
    }
}
