using System.Security.Claims;
using be.Services.ParticipantAssessment;
using be.DTOs.ParticipantAssessment;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace be.Controllers;

[ApiController]
[Route("api/participant-assessments")]
[Authorize(Roles = "PARTICIPANT")]
public class ParticipantAssessmentsController : ControllerBase
{
    private readonly IParticipantAssessmentService
        _participantAssessmentService;

    public ParticipantAssessmentsController(
        IParticipantAssessmentService participantAssessmentService)
    {
        _participantAssessmentService =
            participantAssessmentService;
    }

    [HttpPost("{assessmentId:guid}/start")]
    public async Task<IActionResult> Start(Guid assessmentId)
    {
        try
        {
            var userIdClaim =
                User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!Guid.TryParse(userIdClaim, out var userId))
            {
                return Unauthorized(new
                {
                    message = "Invalid user identity."
                });
            }

            var result =
                await _participantAssessmentService
                    .StartAssessmentAsync(
                        userId,
                        assessmentId);

            return Ok(result);
        }
        catch (ArgumentException ex)
        {
            return NotFound(new
            {
                message = ex.Message
            });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new
            {
                message = ex.Message
            });
        }
    }


    [HttpPut("attempts/{attemptId:guid}/questions/{questionId:guid}/answer")]
    public async Task<IActionResult> SaveAnswer(
        Guid attemptId,
        Guid questionId,
        SaveAssessmentAnswerRequest request)
    {
        try
        {
            var userIdClaim =
                User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!Guid.TryParse(userIdClaim, out var userId))
            {
                return Unauthorized(new
                {
                    message = "Invalid user identity."
                });
            }

            var result =
                await _participantAssessmentService.SaveAnswerAsync(
                    userId,
                    attemptId,
                    questionId,
                    request);

            return Ok(result);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new
            {
                message = ex.Message
            });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new
            {
                message = ex.Message
            });
        }
    }


    [HttpPost("attempts/{attemptId:guid}/submit")]
    public async Task<IActionResult> Submit(
        Guid attemptId)
    {
        try
        {
            var userIdClaim =
                User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!Guid.TryParse(userIdClaim, out var userId))
            {
                return Unauthorized(new
                {
                    message = "Invalid user identity."
                });
            }

            var result =
                await _participantAssessmentService
                    .SubmitAssessmentAsync(
                        userId,
                        attemptId);

            return Ok(result);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new
            {
                message = ex.Message
            });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new
            {
                message = ex.Message
            });
        }
    }
}