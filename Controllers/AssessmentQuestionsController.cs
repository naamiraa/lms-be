using be.DTOs.AssessmentQuestion;
using be.Services.AssessmentQuestion;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace be.Controllers;

[ApiController]
[Route("api/assessment-questions")]
[Authorize(Roles = "ADMIN")]
public class AssessmentQuestionsController : ControllerBase
{
    private readonly IAssessmentQuestionService _questionService;

    public AssessmentQuestionsController(
        IAssessmentQuestionService questionService)
    {
        _questionService = questionService;
    }

    [HttpGet("assessment/{assessmentId:guid}")]
    public async Task<IActionResult> GetByAssessment(
        Guid assessmentId)
    {
        var questions =
            await _questionService
                .GetByAssessmentAsync(assessmentId);

        return Ok(questions);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var question =
            await _questionService.GetByIdAsync(id);

        if (question == null)
        {
            return NotFound(new
            {
                message = "Assessment question not found."
            });
        }

        return Ok(question);
    }

    [HttpPost]
    public async Task<IActionResult> Create(
        CreateAssessmentQuestionRequest request)
    {
        try
        {
            var question =
                await _questionService.CreateAsync(request);

            return CreatedAtAction(
                nameof(GetById),
                new { id = question.Id },
                question
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
    public async Task<IActionResult> Update(
        Guid id,
        UpdateAssessmentQuestionRequest request)
    {
        try
        {
            var success =
                await _questionService.UpdateAsync(id, request);

            if (!success)
            {
                return NotFound(new
                {
                    message = "Assessment question not found."
                });
            }

            return Ok(new
            {
                message = "Assessment question updated successfully."
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
    public async Task<IActionResult> Deactivate(Guid id)
    {
        var success =
            await _questionService.DeactivateAsync(id);

        if (!success)
        {
            return NotFound(new
            {
                message = "Assessment question not found."
            });
        }

        return Ok(new
        {
            message = "Assessment question deactivated successfully."
        });
    }
}