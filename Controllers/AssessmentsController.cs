using be.DTOs.Assessment;
using be.Services.Assessment;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace be.Controllers;

[ApiController]
[Route("api/assessments")]
public class AssessmentsController : ControllerBase
{
    private readonly IAssessmentService _assessmentService;

    public AssessmentsController(
        IAssessmentService assessmentService)
    {
        _assessmentService = assessmentService;
    }

    [HttpGet]
    [Authorize]
    public async Task<IActionResult> GetAll()
    {
        var assessments =
            await _assessmentService.GetAllAsync();

        return Ok(assessments);
    }

    [HttpGet("{id:guid}")]
    [Authorize]
    public async Task<IActionResult> GetById(Guid id)
    {
        var assessment =
            await _assessmentService.GetByIdAsync(id);

        if (assessment == null)
        {
            return NotFound(new
            {
                message = "Assessment not found."
            });
        }

        return Ok(assessment);
    }

    [HttpGet("learning-module/{learningModuleId:guid}")]
    [Authorize]
    public async Task<IActionResult> GetByLearningModule(
        Guid learningModuleId)
    {
        var assessments =
            await _assessmentService
                .GetByLearningModuleAsync(learningModuleId);

        return Ok(assessments);
    }

    [HttpPost]
    [Authorize(Roles = "ADMIN")]
    public async Task<IActionResult> Create(
        CreateAssessmentRequest request)
    {
        try
        {
            var assessment =
                await _assessmentService.CreateAsync(request);

            return CreatedAtAction(
                nameof(GetById),
                new { id = assessment.Id },
                assessment
            );
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new
            {
                message = ex.Message
            });
        }
    }

    [HttpPut("{id:guid}")]
    [Authorize(Roles = "ADMIN")]
    public async Task<IActionResult> Update(
        Guid id,
        UpdateAssessmentRequest request)
    {
        try
        {
            var success =
                await _assessmentService.UpdateAsync(id, request);

            if (!success)
            {
                return NotFound(new
                {
                    message = "Assessment not found."
                });
            }

            return Ok(new
            {
                message = "Assessment updated successfully."
            });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new
            {
                message = ex.Message
            });
        }
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "ADMIN")]
    public async Task<IActionResult> Deactivate(Guid id)
    {
        var success =
            await _assessmentService.DeactivateAsync(id);

        if (!success)
        {
            return NotFound(new
            {
                message = "Assessment not found."
            });
        }

        return Ok(new
        {
            message = "Assessment deactivated successfully."
        });
    }
}