using be.Data;
using be.DTOs.LearningContent;
using Microsoft.EntityFrameworkCore;

namespace be.Services.LearningContent;

public class LearningContentService : ILearningContentService
{
    private readonly AppDbContext _context;

    private static readonly string[] AllowedContentTypes =
    {
        "TEXT",
        "VIDEO",
        "PDF",
        "LINK"
    };

    public LearningContentService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<LearningContentResponse>> GetAllAsync()
    {
        return await _context.LearningContents
            .Where(c => c.IsActive)
            .OrderBy(c => c.LearningModuleId)
            .ThenBy(c => c.Order)
            .Select(c => new LearningContentResponse
            {
                Id = c.Id,
                LearningModuleId = c.LearningModuleId,
                LearningModuleName = c.LearningModule.Name,
                Title = c.Title,
                ContentType = c.ContentType,
                ContentUrl = c.ContentUrl,
                Description = c.Description,
                Order = c.Order,
                EstimatedMinutes = c.EstimatedMinutes,
                IsActive = c.IsActive,
                CreatedAt = c.CreatedAt,
                UpdatedAt = c.UpdatedAt
            })
            .ToListAsync();
    }

    public async Task<List<LearningContentResponse>>
        GetByLearningModuleAsync(Guid learningModuleId)
    {
        return await _context.LearningContents
            .Where(c =>
                c.LearningModuleId == learningModuleId &&
                c.IsActive)
            .OrderBy(c => c.Order)
            .Select(c => new LearningContentResponse
            {
                Id = c.Id,
                LearningModuleId = c.LearningModuleId,
                LearningModuleName = c.LearningModule.Name,
                Title = c.Title,
                ContentType = c.ContentType,
                ContentUrl = c.ContentUrl,
                Description = c.Description,
                Order = c.Order,
                EstimatedMinutes = c.EstimatedMinutes,
                IsActive = c.IsActive,
                CreatedAt = c.CreatedAt,
                UpdatedAt = c.UpdatedAt
            })
            .ToListAsync();
    }

    public async Task<LearningContentResponse?> GetByIdAsync(Guid id)
    {
        return await _context.LearningContents
            .Where(c => c.Id == id)
            .Select(c => new LearningContentResponse
            {
                Id = c.Id,
                LearningModuleId = c.LearningModuleId,
                LearningModuleName = c.LearningModule.Name,
                Title = c.Title,
                ContentType = c.ContentType,
                ContentUrl = c.ContentUrl,
                Description = c.Description,
                Order = c.Order,
                EstimatedMinutes = c.EstimatedMinutes,
                IsActive = c.IsActive,
                CreatedAt = c.CreatedAt,
                UpdatedAt = c.UpdatedAt
            })
            .FirstOrDefaultAsync();
    }

    public async Task<LearningContentResponse> CreateAsync(
        CreateLearningContentRequest request)
    {
        var module = await _context.LearningModules
            .FirstOrDefaultAsync(m =>
                m.Id == request.LearningModuleId &&
                m.IsActive);

        if (module == null)
        {
            throw new ArgumentException(
                "Learning module not found or inactive.");
        }

        var contentType = NormalizeContentType(request.ContentType);

        var content = new Models.LearningContent
        {
            LearningModuleId = request.LearningModuleId,
            Title = request.Title,
            ContentType = contentType,
            ContentUrl = request.ContentUrl,
            Description = request.Description,
            Order = request.Order,
            EstimatedMinutes = request.EstimatedMinutes
        };

        _context.LearningContents.Add(content);

        await _context.SaveChangesAsync();

        return new LearningContentResponse
        {
            Id = content.Id,
            LearningModuleId = content.LearningModuleId,
            LearningModuleName = module.Name,
            Title = content.Title,
            ContentType = content.ContentType,
            ContentUrl = content.ContentUrl,
            Description = content.Description,
            Order = content.Order,
            EstimatedMinutes = content.EstimatedMinutes,
            IsActive = content.IsActive,
            CreatedAt = content.CreatedAt,
            UpdatedAt = content.UpdatedAt
        };
    }

    public async Task<bool> UpdateAsync(
        Guid id,
        UpdateLearningContentRequest request)
    {
        var content = await _context.LearningContents
            .FirstOrDefaultAsync(c => c.Id == id);

        if (content == null)
        {
            return false;
        }

        var moduleExists = await _context.LearningModules
            .AnyAsync(m =>
                m.Id == request.LearningModuleId &&
                m.IsActive);

        if (!moduleExists)
        {
            throw new ArgumentException(
                "Learning module not found or inactive.");
        }

        var contentType = NormalizeContentType(request.ContentType);

        content.LearningModuleId = request.LearningModuleId;
        content.Title = request.Title;
        content.ContentType = contentType;
        content.ContentUrl = request.ContentUrl;
        content.Description = request.Description;
        content.Order = request.Order;
        content.EstimatedMinutes = request.EstimatedMinutes;
        content.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<bool> DeactivateAsync(Guid id)
    {
        var content = await _context.LearningContents
            .FirstOrDefaultAsync(c => c.Id == id);

        if (content == null)
        {
            return false;
        }

        content.IsActive = false;
        content.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return true;
    }

    private static string NormalizeContentType(string contentType)
    {
        var normalized = contentType.Trim().ToUpperInvariant();

        if (!AllowedContentTypes.Contains(normalized))
        {
            throw new ArgumentException(
                "Content type must be TEXT, VIDEO, PDF, or LINK.");
        }

        return normalized;
    }
}