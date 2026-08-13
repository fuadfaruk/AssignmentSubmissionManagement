using AssignmentSubmissionManagement.Core.DTOs.Common;
using AssignmentSubmissionManagement.Core.DTOs.Submissions;
using AssignmentSubmissionManagement.Core.Interfaces.Services;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace AssignmentSubmissionManagement.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class SubmissionsController : ControllerBase
    {
        private readonly ISubmissionService _submissionService;
        private readonly IValidator<GradeSubmissionRequest> _gradeValidator;

        public SubmissionsController(
            ISubmissionService submissionService,
            IValidator<GradeSubmissionRequest> gradeValidator)
        {
            _submissionService = submissionService;
            _gradeValidator = gradeValidator;
        }

        [HttpGet("{id:guid}")]
        [ProducesResponseType(typeof(SubmissionResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<SubmissionResponse>> GetById(Guid id)
        {
            var submission = await _submissionService.GetByIdAsync(id);
            if (submission is null)
                return NotFound();

            var role = User.FindFirstValue(ClaimTypes.Role);
            var userId = GetCurrentUserId();

            if (role == "Student" && submission.StudentId != userId)
                return Forbid();

            return Ok(submission);
        }

        [HttpGet("by-assignment/{assignmentId:guid}")]
        [HttpGet("assignment/{assignmentId:guid}")]
        [Authorize(Roles = "Teacher")]
        [ProducesResponseType(typeof(PagedResult<SubmissionResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<PagedResult<SubmissionResponse>>> GetByAssignment(
            Guid assignmentId,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20)
        {
            try
            {
                var result = await _submissionService.GetByAssignmentIdAsync(
                    assignmentId, page, pageSize, GetCurrentUserId());
                return Ok(result);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
        }

        [HttpGet("my")]
        [HttpGet("my-submissions")]
        [Authorize(Roles = "Student")]
        [ProducesResponseType(typeof(PagedResult<SubmissionResponse>), StatusCodes.Status200OK)]
        public async Task<ActionResult<PagedResult<SubmissionResponse>>> GetMine(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20)
        {
            var result = await _submissionService.GetByStudentIdAsync(GetCurrentUserId(), page, pageSize);
            return Ok(result);
        }

        [HttpPost]
        [Authorize(Roles = "Student")]
        [Consumes("multipart/form-data")]
        [ProducesResponseType(typeof(SubmissionResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<SubmissionResponse>> Submit(
            [FromForm] CreateSubmissionRequest request,
            IFormFile? file)
        {
            try
            {
                var created = await _submissionService.SubmitAsync(request, file, GetCurrentUserId());
                return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new ProblemDetails { Detail = ex.Message });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Forbid(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new ProblemDetails { Detail = ex.Message });
            }
        }

        [HttpPut("{id:guid}/grade")]
        [Authorize(Roles = "Teacher")]
        [ProducesResponseType(typeof(SubmissionResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<SubmissionResponse>> Grade(Guid id, [FromBody] GradeSubmissionRequest request)
        {
            var validation = await _gradeValidator.ValidateAsync(request);
            if (!validation.IsValid)
                return ValidationProblem(new ValidationProblemDetails(validation.ToDictionary()));

            try
            {
                var graded = await _submissionService.GradeAsync(id, request, GetCurrentUserId());
                return Ok(graded);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new ProblemDetails { Detail = ex.Message });
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new ProblemDetails { Detail = ex.Message });
            }
        }

        private Guid GetCurrentUserId() =>
            Guid.Parse(User.FindFirstValue("userId")
                ?? User.FindFirstValue(ClaimTypes.NameIdentifier)
                ?? throw new InvalidOperationException("User ID claim missing."));
    }

}
