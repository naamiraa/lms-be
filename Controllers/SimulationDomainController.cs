using be.DTOs.SimulationDomain;
using be.Services.SimulationDomain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace be.Controllers;

[ApiController]
[Route("api/simulation-domains")]
public class SimulationDomainsController : ControllerBase
{
    private readonly ISimulationDomainService _simulationDomainService;

    public SimulationDomainsController(
        ISimulationDomainService simulationDomainService)
    {
        _simulationDomainService = simulationDomainService;
    }

    [HttpGet]
    [Authorize]
    public async Task<IActionResult> GetAll()
    {
        return Ok(
            await _simulationDomainService.GetAllAsync());
    }

    [HttpGet("{id:guid}")]
    [Authorize]
    public async Task<IActionResult> GetById(Guid id)
    {
        var domain =
            await _simulationDomainService.GetByIdAsync(id);

        if (domain == null)
        {
            return NotFound(new
            {
                message = "Simulation domain not found."
            });
        }

        return Ok(domain);
    }

    [HttpPost]
    [Authorize(Roles = "ADMIN")]
    public async Task<IActionResult> Create(
        CreateSimulationDomainRequest request)
    {
        try
        {
            var domain =
                await _simulationDomainService.CreateAsync(request);

            return CreatedAtAction(
                nameof(GetById),
                new { id = domain.Id },
                domain);
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
        UpdateSimulationDomainRequest request)
    {
        try
        {
            var success =
                await _simulationDomainService.UpdateAsync(id, request);

            if (!success)
            {
                return NotFound(new
                {
                    message = "Simulation domain not found."
                });
            }

            return Ok(new
            {
                message = "Simulation domain updated successfully."
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
            await _simulationDomainService.DeactivateAsync(id);

        if (!success)
        {
            return NotFound(new
            {
                message = "Simulation domain not found."
            });
        }

        return Ok(new
        {
            message = "Simulation domain deactivated successfully."
        });
    }
}