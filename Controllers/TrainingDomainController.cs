using be.DTOs.TrainingDomain;
using be.Services.TrainingDomain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace be.Controllers;

[ApiController]
[Route("api/training-domains")]
public class TrainingDomainsController : ControllerBase
{
    private readonly ITrainingDomainService _trainingDomainService;

    public TrainingDomainsController(
        ITrainingDomainService trainingDomainService)
    {
        _trainingDomainService = trainingDomainService;
    }

    [HttpGet]
    [Authorize]
    public async Task<IActionResult> GetAll()
    {
        var domains = await _trainingDomainService.GetAllAsync();

        return Ok(domains);
    }

    [HttpGet("{id:guid}")]
    [Authorize]
    public async Task<IActionResult> GetById(Guid id)
    {
        var domain = await _trainingDomainService.GetByIdAsync(id);

        if (domain == null)
        {
            return NotFound(new
            {
                message = "Training domain not found."
            });
        }

        return Ok(domain);
    }

    [HttpPost]
    [Authorize(Roles = "ADMIN")]
    public async Task<IActionResult> Create(
        CreateTrainingDomainRequest request)
    {
        var domain = await _trainingDomainService.CreateAsync(request);

        return CreatedAtAction(
            nameof(GetById),
            new { id = domain.Id },
            domain
        );
    }

    [HttpPut("{id:guid}")]
    [Authorize(Roles = "ADMIN")]
    public async Task<IActionResult> Update(
        Guid id,
        UpdateTrainingDomainRequest request)
    {
        var success = await _trainingDomainService.UpdateAsync(
            id,
            request
        );

        if (!success)
        {
            return NotFound(new
            {
                message = "Training domain not found."
            });
        }

        return Ok(new
        {
            message = "Training domain updated successfully."
        });
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "ADMIN")]
    public async Task<IActionResult> Deactivate(Guid id)
    {
        var success = await _trainingDomainService.DeactivateAsync(id);

        if (!success)
        {
            return NotFound(new
            {
                message = "Training domain not found."
            });
        }

        return Ok(new
        {
            message = "Training domain deactivated successfully."
        });
    }
}