using be.Data;
using be.DTOs.SimulationScenario;
using Microsoft.EntityFrameworkCore;

namespace be.Services.SimulationScenario;

public class SimulationScenarioService : ISimulationScenarioService
{
    private readonly AppDbContext _context;

    public SimulationScenarioService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<SimulationScenarioResponse>> GetAllAsync()
    {
        return await _context.SimulationScenarios
            .Where(s => s.IsActive)
            .OrderBy(s => s.Name)
            .Select(s => new SimulationScenarioResponse
            {
                Id = s.Id,
                SimulationDomainId = s.SimulationDomainId,
                SimulationDomainName = s.SimulationDomain.Name,
                Name = s.Name,
                Description = s.Description,
                Difficulty = s.Difficulty,
                EstimatedMinutes = s.EstimatedMinutes,
                IsActive = s.IsActive,
                CreatedAt = s.CreatedAt,
                UpdatedAt = s.UpdatedAt
            })
            .ToListAsync();
    }

    public async Task<List<SimulationScenarioResponse>> GetByDomainIdAsync(
        Guid domainId)
    {
        return await _context.SimulationScenarios
            .Where(s =>
                s.SimulationDomainId == domainId &&
                s.IsActive)
            .OrderBy(s => s.Name)
            .Select(s => new SimulationScenarioResponse
            {
                Id = s.Id,
                SimulationDomainId = s.SimulationDomainId,
                SimulationDomainName = s.SimulationDomain.Name,
                Name = s.Name,
                Description = s.Description,
                Difficulty = s.Difficulty,
                EstimatedMinutes = s.EstimatedMinutes,
                IsActive = s.IsActive,
                CreatedAt = s.CreatedAt,
                UpdatedAt = s.UpdatedAt
            })
            .ToListAsync();
    }

    public async Task<SimulationScenarioResponse?> GetByIdAsync(Guid id)
    {
        return await _context.SimulationScenarios
            .Where(s => s.Id == id)
            .Select(s => new SimulationScenarioResponse
            {
                Id = s.Id,
                SimulationDomainId = s.SimulationDomainId,
                SimulationDomainName = s.SimulationDomain.Name,
                Name = s.Name,
                Description = s.Description,
                Difficulty = s.Difficulty,
                EstimatedMinutes = s.EstimatedMinutes,
                IsActive = s.IsActive,
                CreatedAt = s.CreatedAt,
                UpdatedAt = s.UpdatedAt
            })
            .FirstOrDefaultAsync();
    }

    public async Task<SimulationScenarioResponse> CreateAsync(
        CreateSimulationScenarioRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Name))
        {
            throw new ArgumentException(
                "Simulation scenario name is required.");
        }

        if (request.EstimatedMinutes <= 0)
        {
            throw new ArgumentException(
                "Estimated minutes must be greater than zero.");
        }

        var domain = await _context.SimulationDomains
            .FirstOrDefaultAsync(d =>
                d.Id == request.SimulationDomainId &&
                d.IsActive);

        if (domain == null)
        {
            throw new ArgumentException(
                "Simulation domain not found or inactive.");
        }

        var duplicate = await _context.SimulationScenarios
            .AnyAsync(s =>
                s.SimulationDomainId == request.SimulationDomainId &&
                s.Name.ToLower() == request.Name.Trim().ToLower() &&
                s.IsActive);

        if (duplicate)
        {
            throw new ArgumentException(
                "Simulation scenario already exists in this domain.");
        }

        var scenario = new Models.SimulationScenario
        {
            SimulationDomainId = request.SimulationDomainId,
            Name = request.Name.Trim(),
            Description = request.Description,
            Difficulty = request.Difficulty,
            EstimatedMinutes = request.EstimatedMinutes
        };

        _context.SimulationScenarios.Add(scenario);

        await _context.SaveChangesAsync();

        return new SimulationScenarioResponse
        {
            Id = scenario.Id,
            SimulationDomainId = scenario.SimulationDomainId,
            SimulationDomainName = domain.Name,
            Name = scenario.Name,
            Description = scenario.Description,
            Difficulty = scenario.Difficulty,
            EstimatedMinutes = scenario.EstimatedMinutes,
            IsActive = scenario.IsActive,
            CreatedAt = scenario.CreatedAt,
            UpdatedAt = scenario.UpdatedAt
        };
    }

    public async Task<bool> UpdateAsync(
        Guid id,
        UpdateSimulationScenarioRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Name))
        {
            throw new ArgumentException(
                "Simulation scenario name is required.");
        }

        if (request.EstimatedMinutes <= 0)
        {
            throw new ArgumentException(
                "Estimated minutes must be greater than zero.");
        }

        var scenario = await _context.SimulationScenarios
            .FirstOrDefaultAsync(s => s.Id == id);

        if (scenario == null)
        {
            return false;
        }

        var domain = await _context.SimulationDomains
            .FirstOrDefaultAsync(d =>
                d.Id == request.SimulationDomainId &&
                d.IsActive);

        if (domain == null)
        {
            throw new ArgumentException(
                "Simulation domain not found or inactive.");
        }

        var duplicate = await _context.SimulationScenarios
            .AnyAsync(s =>
                s.Id != id &&
                s.SimulationDomainId == request.SimulationDomainId &&
                s.Name.ToLower() == request.Name.Trim().ToLower() &&
                s.IsActive);

        if (duplicate)
        {
            throw new ArgumentException(
                "Simulation scenario already exists in this domain.");
        }

        scenario.SimulationDomainId = request.SimulationDomainId;
        scenario.Name = request.Name.Trim();
        scenario.Description = request.Description;
        scenario.Difficulty = request.Difficulty;
        scenario.EstimatedMinutes = request.EstimatedMinutes;
        scenario.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<bool> DeactivateAsync(Guid id)
    {
        var scenario = await _context.SimulationScenarios
            .FirstOrDefaultAsync(s => s.Id == id);

        if (scenario == null)
        {
            return false;
        }

        scenario.IsActive = false;
        scenario.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return true;
    }

}