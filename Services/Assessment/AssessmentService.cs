using be.Data;
using be.DTOs.Assessment;
using Microsoft.EntityFrameworkCore;

namespace be.Services.Assessment;

public class AssessmentService : IAssessmentService
{
    private readonly AppDbContext _context;

    private static readonly string[] AllowedTypes =
    {
        "PRE_TEST",
        "POST_TEST"
    };

    public AssessmentService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<AssessmentResponse>> GetAllAsync()
    {
        return await _context.Assessments
            .Where(a => a.IsActive)
            .OrderBy(a => a.LearningModule.Name)
            .ThenBy(a => a.Title)
            .Select(a => new AssessmentResponse
            {
                Id = a.Id,
                LearningModuleId = a.LearningModuleId,
                LearningModuleName = a.LearningModule.Name,
                Title = a.Title,
                Type = a.Type,
                Description = a.Description,
                DurationMinutes = a.DurationMinutes,
                PassingScore = a.PassingScore,
                MaxAttempts = a.MaxAttempts,
                QuestionCount = a.Questions.Count(q => q.IsActive),
                IsActive = a.IsActive,
                CreatedAt = a.CreatedAt,
                UpdatedAt = a.UpdatedAt
            })
            .ToListAsync();
    }

    public async Task<List<AssessmentResponse>>
        GetByLearningModuleAsync(Guid learningModuleId)
    {
        return await _context.Assessments
            .Where(a =>
                a.LearningModuleId == learningModuleId &&
                a.IsActive)
            .OrderBy(a => a.Title)
            .Select(a => new AssessmentResponse
            {
                Id = a.Id,
                LearningModuleId = a.LearningModuleId,
                LearningModuleName = a.LearningModule.Name,
                Title = a.Title,
                Type = a.Type,
                Description = a.Description,
                DurationMinutes = a.DurationMinutes,
                PassingScore = a.PassingScore,
                MaxAttempts = a.MaxAttempts,
                QuestionCount = a.Questions.Count(q => q.IsActive),
                IsActive = a.IsActive,
                CreatedAt = a.CreatedAt,
                UpdatedAt = a.UpdatedAt
            })
            .ToListAsync();
    }

    public async Task<AssessmentResponse?> GetByIdAsync(Guid id)
    {
        return await _context.Assessments
            .Where(a => a.Id == id)
            .Select(a => new AssessmentResponse
            {
                Id = a.Id,
                LearningModuleId = a.LearningModuleId,
                LearningModuleName = a.LearningModule.Name,
                Title = a.Title,
                Type = a.Type,
                Description = a.Description,
                DurationMinutes = a.DurationMinutes,
                PassingScore = a.PassingScore,
                MaxAttempts = a.MaxAttempts,
                QuestionCount = a.Questions.Count(q => q.IsActive),
                IsActive = a.IsActive,
                CreatedAt = a.CreatedAt,
                UpdatedAt = a.UpdatedAt
            })
            .FirstOrDefaultAsync();
    }

    public async Task<AssessmentResponse> CreateAsync(
        CreateAssessmentRequest request)
    {
        ValidateRequest(
            request.Type,
            request.DurationMinutes,
            request.PassingScore,
            request.MaxAttempts);

        var module = await _context.LearningModules
            .FirstOrDefaultAsync(m =>
                m.Id == request.LearningModuleId &&
                m.IsActive);

        if (module == null)
        {
            throw new ArgumentException(
                "Learning module not found or inactive.");
        }

        var assessment = new Models.Assessment
        {
            LearningModuleId = request.LearningModuleId,
            Title = request.Title,
            Type = NormalizeType(request.Type),
            Description = request.Description,
            DurationMinutes = request.DurationMinutes,
            PassingScore = request.PassingScore,
            MaxAttempts = request.MaxAttempts
        };

        _context.Assessments.Add(assessment);

        await _context.SaveChangesAsync();

        return new AssessmentResponse
        {
            Id = assessment.Id,
            LearningModuleId = assessment.LearningModuleId,
            LearningModuleName = module.Name,
            Title = assessment.Title,
            Type = assessment.Type,
            Description = assessment.Description,
            DurationMinutes = assessment.DurationMinutes,
            PassingScore = assessment.PassingScore,
            MaxAttempts = assessment.MaxAttempts,
            QuestionCount = 0,
            IsActive = assessment.IsActive,
            CreatedAt = assessment.CreatedAt,
            UpdatedAt = assessment.UpdatedAt
        };
    }

    public async Task<bool> UpdateAsync(
        Guid id,
        UpdateAssessmentRequest request)
    {
        ValidateRequest(
            request.Type,
            request.DurationMinutes,
            request.PassingScore,
            request.MaxAttempts);

        var assessment = await _context.Assessments
            .FirstOrDefaultAsync(a => a.Id == id);

        if (assessment == null)
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

        assessment.LearningModuleId = request.LearningModuleId;
        assessment.Title = request.Title;
        assessment.Type = NormalizeType(request.Type);
        assessment.Description = request.Description;
        assessment.DurationMinutes = request.DurationMinutes;
        assessment.PassingScore = request.PassingScore;
        assessment.MaxAttempts = request.MaxAttempts;
        assessment.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<bool> DeactivateAsync(Guid id)
    {
        var assessment = await _context.Assessments
            .FirstOrDefaultAsync(a => a.Id == id);

        if (assessment == null)
        {
            return false;
        }

        assessment.IsActive = false;
        assessment.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return true;
    }

    private static string NormalizeType(string type)
    {
        return type.Trim().ToUpperInvariant();
    }

    private static void ValidateRequest(
        string type,
        int durationMinutes,
        double passingScore,
        int maxAttempts)
    {
        var normalizedType = NormalizeType(type);

        if (!AllowedTypes.Contains(normalizedType))
        {
            throw new ArgumentException(
                "Assessment type must be PRE_TEST or POST_TEST.");
        }

        if (durationMinutes <= 0)
        {
            throw new ArgumentException(
                "Duration must be greater than 0 minutes.");
        }

        if (passingScore < 0 || passingScore > 100)
        {
            throw new ArgumentException(
                "Passing score must be between 0 and 100.");
        }

        if (maxAttempts <= 0)
        {
            throw new ArgumentException(
                "Max attempts must be greater than 0.");
        }
    }
}