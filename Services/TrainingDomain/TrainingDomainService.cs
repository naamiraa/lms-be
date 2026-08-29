using be.Data;
using be.DTOs.TrainingDomain;
using be.Models;
using Microsoft.EntityFrameworkCore;

namespace be.Services.TrainingDomain;

public class TrainingDomainService : ITrainingDomainService
{
    private readonly AppDbContext _context;

    public TrainingDomainService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<TrainingDomainResponse>> GetAllAsync()
    {
        return await _context.TrainingDomains
            .Where(d => d.IsActive)
            .OrderBy(d => d.Name)
            .Select(d => new TrainingDomainResponse
            {
                Id = d.Id,
                Name = d.Name,
                Description = d.Description,
                IsActive = d.IsActive,
                CreatedAt = d.CreatedAt,
                UpdatedAt = d.UpdatedAt
            })
            .ToListAsync();
    }

    public async Task<TrainingDomainResponse?> GetByIdAsync(Guid id)
    {
        return await _context.TrainingDomains
            .Where(d => d.Id == id)
            .Select(d => new TrainingDomainResponse
            {
                Id = d.Id,
                Name = d.Name,
                Description = d.Description,
                IsActive = d.IsActive,
                CreatedAt = d.CreatedAt,
                UpdatedAt = d.UpdatedAt
            })
            .FirstOrDefaultAsync();
    }

    public async Task<TrainingDomainResponse> CreateAsync(
        CreateTrainingDomainRequest request)
    {
        var domain = new Models.TrainingDomain
        {
            Name = request.Name,
            Description = request.Description
        };

        _context.TrainingDomains.Add(domain);

        await _context.SaveChangesAsync();

        return new TrainingDomainResponse
        {
            Id = domain.Id,
            Name = domain.Name,
            Description = domain.Description,
            IsActive = domain.IsActive,
            CreatedAt = domain.CreatedAt,
            UpdatedAt = domain.UpdatedAt
        };
    }

    public async Task<bool> UpdateAsync(
        Guid id,
        UpdateTrainingDomainRequest request)
    {
        var domain = await _context.TrainingDomains
            .FirstOrDefaultAsync(d => d.Id == id);

        if (domain == null)
        {
            return false;
        }

        domain.Name = request.Name;
        domain.Description = request.Description;
        domain.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<bool> DeactivateAsync(Guid id)
    {
        var domain = await _context.TrainingDomains
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