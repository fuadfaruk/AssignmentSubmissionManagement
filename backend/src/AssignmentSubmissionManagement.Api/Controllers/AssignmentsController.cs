using AssignmentSubmissionManagement.Core.DTOs.Assignments;
using AssignmentSubmissionManagement.Core.DTOs.Common;
using AssignmentSubmissionManagement.Infrastructure.Services;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace AssignmentSubmissionManagement.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class AssignmentsController : ControllerBase
    {
        private readonly IAssignmentService _assignmentService;
        private readonly IValidator<CreateAssignmentRequest> _createValidator;
        private readonly IValidator<UpdateAssignmentRequest> _updateValidator;

        public AssignmentsController(
            IAssignmentService assignmentService,
            IValidator<CreateAssignmentRequest> createValidator,
            IValidator<UpdateAssignmentRequest> updateValidator)
        {
            _assignmentService = assignmentService;
            _createValidator = createValidator;
            _updateValidator = updateValidator;
        }

        [HttpGet]
        [ProducesResponseType(typeof(PagedResult<AssignmentResponse>), StatusCodes.Status200OK)]
        public async Task<ActionResult<PagedResult<AssignmentResponse>>> GetAll(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20,
            [FromQuery] Guid? classId = null,
            [FromQuery] DateTime? dueBefore = null,
            [FromQuery] DateTime? dueAfter = null)
        {
            var role = User.FindFirstValue(ClaimTypes.Role);
            var userId = GetCurrentUserId();

            PagedResult<AssignmentResponse> result;

            if (role == "Student")
            {
                result = await _assignmentService.GetForStudentPagedAsync(
                    userId, page, pageSize, classId, dueBefore, dueAfter);
            }
            else if (role == "Teacher")
            {
                result = await _assignmentService.GetPagedAsync(
                    page, pageSize, classId, dueBefore, dueAfter, teacherId: userId);
            }
            else
            {
                result = await _assignmentService.GetPagedAsync(
                    page, pageSize, classId, dueBefore, dueAfter);
            }

            return Ok(result);
        }

        [HttpGet("{id:guid}")]
        [ProducesResponseType(typeof(AssignmentResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<AssignmentResponse>> GetById(Guid id)
        {
            var assignment = await _assignmentService.GetByIdAsync(id);
            return assignment is null ? NotFound() : Ok(assignment);
        }

        [HttpPost]
        [Authorize(Roles = "Teacher")]
        [ProducesResponseType(typeof(AssignmentResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<AssignmentResponse>> Create([FromBody] CreateAssignmentRequest request)
        {
            var validation = await _createValidator.ValidateAsync(request);
            if (!validation.IsValid)
                return ValidationProblem(new ValidationProblemDetails(validation.ToDictionary()));

            try
            {
                var created = await _assignmentService.CreateAsync(request, GetCurrentUserId());
                return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Forbid(ex.Message);
            }
        }

        [HttpPut("{id:guid}")]
        [Authorize(Roles = "Teacher")]
        [ProducesResponseType(typeof(AssignmentResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<AssignmentResponse>> Update(Guid id, [FromBody] UpdateAssignmentRequest request)
        {
            var validation = await _updateValidator.ValidateAsync(request);
            if (!validation.IsValid)
                return ValidationProblem(new ValidationProblemDetails(validation.ToDictionary()));

            try
            {
                var updated = await _assignmentService.UpdateAsync(id, request, GetCurrentUserId());
                return Ok(updated);
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

        [HttpDelete("{id:guid}")]
        [Authorize(Roles = "Teacher")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                await _assignmentService.DeleteAsync(id, GetCurrentUserId());
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
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
