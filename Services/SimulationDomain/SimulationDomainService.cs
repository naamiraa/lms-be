using be.Data;
using be.DTOs.SimulationDomain;
using Microsoft.EntityFrameworkCore;

namespace be.Services.SimulationDomain;

public class SimulationDomainService : ISimulationDomainService
{
    private readonly AppDbContext _context;

    public SimulationDomainService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<SimulationDomainResponse>> GetAllAsync()
    {
        return await _context.SimulationDomains
            .Where(d => d.IsActive)
            .OrderBy(d => d.Name)
            .Select(d => new SimulationDomainResponse
            {
                Id = d.Id,
                Name = d.Name,
                Description = d.Description,
                ScenarioCount = d.Scenarios.Count(s => s.IsActive),
                IsActive = d.IsActive,
                CreatedAt = d.CreatedAt,
                UpdatedAt = d.UpdatedAt
            })
            .ToListAsync();
    }

    public async Task<SimulationDomainResponse?> GetByIdAsync(Guid id)
    {
        return await _context.SimulationDomains
            .Where(d => d.Id == id)
            .Select(d => new SimulationDomainResponse
            {
                Id = d.Id,
                Name = d.Name,
                Description = d.Description,
                ScenarioCount = d.Scenarios.Count(s => s.IsActive),
                IsActive = d.IsActive,
                CreatedAt = d.CreatedAt,
                UpdatedAt = d.UpdatedAt
            })
            .FirstOrDefaultAsync();
    }

    public async Task<SimulationDomainResponse> CreateAsync(
        CreateSimulationDomainRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Name))
        {
            throw new ArgumentException(
                "Simulation domain name is required.");
        }

        var duplicate = await _context.SimulationDomains
            .AnyAsync(d =>
                d.Name.ToLower() == request.Name.Trim().ToLower() &&
                d.IsActive);

        if (duplicate)
        {
            throw new ArgumentException(
                "Simulation domain already exists.");
        }

        var domain = new Models.SimulationDomain
        {
            Name = request.Name.Trim(),
            Description = request.Description
        };

        _context.SimulationDomains.Add(domain);

        await _context.SaveChangesAsync();

        return new SimulationDomainResponse
        {
            Id = domain.Id,
            Name = domain.Name,
            Description = domain.Description,
            ScenarioCount = 0,
            IsActive = domain.IsActive,
            CreatedAt = domain.CreatedAt,
            UpdatedAt = domain.UpdatedAt
        };
    }

    public async Task<bool> UpdateAsync(
        Guid id,
        UpdateSimulationDomainRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Name))
        {
            throw new ArgumentException(
                "Simulation domain name is required.");
        }

        var domain = await _context.SimulationDomains
            .FirstOrDefaultAsync(d => d.Id == id);

        if (domain == null)
        {
            return false;
        }

        var duplicate = await _context.SimulationDomains
            .AnyAsync(d =>
                d.Id != id &&
                d.Name.ToLower() == request.Name.Trim().ToLower() &&
                d.IsActive);

        if (duplicate)
        {
            throw new ArgumentException(
                "Simulation domain already exists.");
        }

        domain.Name = request.Name.Trim();
        domain.Description = request.Description;
        domain.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<bool> DeactivateAsync(Guid id)
    {
        var domain = await _context.SimulationDomains
            .FirstOrDefaultAsync(d => d.Id == id);

        if (domain == null)
        {
            return false;
        }

        domain.IsActive = false;
        domain.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return true;
    }
}