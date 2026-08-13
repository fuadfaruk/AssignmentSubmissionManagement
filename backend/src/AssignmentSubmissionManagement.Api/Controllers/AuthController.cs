using AssignmentSubmissionManagement.Core.DTOs.Auth;
using AssignmentSubmissionManagement.Core.Interfaces.Services;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace AssignmentSubmissionManagement.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly IValidator<LoginRequest> _validator;

        public AuthController(IAuthService authService, IValidator<LoginRequest> validator)
        {
            _authService = authService;
            _validator = validator;
        }

        [HttpPost("login")]
        [ProducesResponseType(typeof(LoginResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<LoginResponse>> Login([FromBody] LoginRequest request)
        {
            var validation = await _validator.ValidateAsync(request);
            if (!validation.IsValid)
                return ValidationProblem(new ValidationProblemDetails(
                    validation.ToDictionary()));

            var result = await _authService.LoginAsync(request);
            if (result is null)
                return Unauthorized(new ProblemDetails
                {
                    Title = "Invalid credentials",
                    Detail = "Email or password is incorrect.",
                    Status = StatusCodes.Status401Unauthorized
                });

            return Ok(result);
        }
    }
}
