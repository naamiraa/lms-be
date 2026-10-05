using be.Data;
using be.DTOs.AssessmentQuestion;
using Microsoft.EntityFrameworkCore;

namespace be.Services.AssessmentQuestion;

public class AssessmentQuestionService : IAssessmentQuestionService
{
    private readonly AppDbContext _context;

    public AssessmentQuestionService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<AssessmentQuestionResponse>>
        GetByAssessmentAsync(Guid assessmentId)
    {
        return await _context.AssessmentQuestions
            .Where(q =>
                q.AssessmentId == assessmentId &&
                q.IsActive)
            .OrderBy(q => q.Order)
            .Select(q => new AssessmentQuestionResponse
            {
                Id = q.Id,
                AssessmentId = q.AssessmentId,
                QuestionText = q.QuestionText,
                MediaUrl = q.MediaUrl,
                Order = q.Order,
                IsActive = q.IsActive,

                Options = q.Options
                    .Where(o => o.IsActive)
                    .OrderBy(o => o.Order)
                    .Select(o => new QuestionOptionResponse
                    {
                        Id = o.Id,
                        OptionText = o.OptionText,
                        IsCorrect = o.IsCorrect,
                        Order = o.Order
                    })
                    .ToList()
            })
            .ToListAsync();
    }

    public async Task<AssessmentQuestionResponse?> GetByIdAsync(Guid id)
    {
        return await _context.AssessmentQuestions
            .Where(q => q.Id == id)
            .Select(q => new AssessmentQuestionResponse
            {
                Id = q.Id,
                AssessmentId = q.AssessmentId,
                QuestionText = q.QuestionText,
                MediaUrl = q.MediaUrl,
                Order = q.Order,
                IsActive = q.IsActive,

                Options = q.Options
                    .Where(o => o.IsActive)
                    .OrderBy(o => o.Order)
                    .Select(o => new QuestionOptionResponse
                    {
                        Id = o.Id,
                        OptionText = o.OptionText,
                        IsCorrect = o.IsCorrect,
                        Order = o.Order
                    })
                    .ToList()
            })
            .FirstOrDefaultAsync();
    }

    public async Task<AssessmentQuestionResponse> CreateAsync(
        CreateAssessmentQuestionRequest request)
    {
        ValidateQuestion(
            request.QuestionText,
            request.Options);

        var assessment = await _context.Assessments
            .FirstOrDefaultAsync(a =>
                a.Id == request.AssessmentId &&
                a.IsActive);

        if (assessment == null)
        {
            throw new ArgumentException(
                "Assessment not found or inactive.");
        }

        var question = new Models.AssessmentQuestion
        {
            AssessmentId = request.AssessmentId,
            QuestionText = request.QuestionText,
            MediaUrl = request.MediaUrl,
            Order = request.Order
        };

        foreach (var optionRequest in request.Options)
        {
            question.Options.Add(new Models.QuestionOption
            {
                OptionText = optionRequest.OptionText,
                IsCorrect = optionRequest.IsCorrect,
                Order = optionRequest.Order
            });
        }

        _context.AssessmentQuestions.Add(question);

        await _context.SaveChangesAsync();

        return new AssessmentQuestionResponse
        {
            Id = question.Id,
            AssessmentId = question.AssessmentId,
            QuestionText = question.QuestionText,
            MediaUrl = question.MediaUrl,
            Order = question.Order,
            IsActive = question.IsActive,

            Options = question.Options
                .OrderBy(o => o.Order)
                .Select(o => new QuestionOptionResponse
                {
                    Id = o.Id,
                    OptionText = o.OptionText,
                    IsCorrect = o.IsCorrect,
                    Order = o.Order
                })
                .ToList()
        };
    }

    public async Task<bool> UpdateAsync(
        Guid id,
        UpdateAssessmentQuestionRequest request)
    {
        ValidateQuestion(
            request.QuestionText,
            request.Options);

        var question = await _context.AssessmentQuestions
            .Include(q => q.Options)
            .FirstOrDefaultAsync(q => q.Id == id);

        if (question == null)
        {
            return false;
        }

        question.QuestionText = request.QuestionText;
        question.MediaUrl = request.MediaUrl;
        question.Order = request.Order;
        question.UpdatedAt = DateTime.UtcNow;

        var existingOptions = question.Options
            .Where(o => o.IsActive)
            .OrderBy(o => o.Order)
            .ToList();

        for (var i = 0; i < request.Options.Count; i++)
        {
            var optionRequest = request.Options[i];

            if (i < existingOptions.Count)
            {
                var option = existingOptions[i];

                option.OptionText = optionRequest.OptionText;
                option.IsCorrect = optionRequest.IsCorrect;
                option.Order = optionRequest.Order;
                option.UpdatedAt = DateTime.UtcNow;
            }
            else
            {
                question.Options.Add(new Models.QuestionOption
                {
                    OptionText = optionRequest.OptionText,
                    IsCorrect = optionRequest.IsCorrect,
                    Order = optionRequest.Order
                });
            }
        }

        // Kalau jumlah option baru lebih sedikit,
        // nonaktifkan option lama yg tdk lg digunakan
        for (var i = request.Options.Count; i < existingOptions.Count; i++)
        {
            existingOptions[i].IsActive = false;
            existingOptions[i].UpdatedAt = DateTime.UtcNow;
        }

        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<bool> DeactivateAsync(Guid id)
    {
        var question = await _context.AssessmentQuestions
            .Include(q => q.Options)
            .FirstOrDefaultAsync(q => q.Id == id);

        if (question == null)
        {
            return false;
        }

        question.IsActive = false;
        question.UpdatedAt = DateTime.UtcNow;

        foreach (var option in question.Options)
        {
            option.IsActive = false;
            option.UpdatedAt = DateTime.UtcNow;
        }

        await _context.SaveChangesAsync();

        return true;
    }

    private static void ValidateQuestion(
        string questionText,
        List<QuestionOptionRequest> options)
    {
        if (string.IsNullOrWhiteSpace(questionText))
        {
            throw new ArgumentException(
                "Question text is required.");
        }

        if (options == null || options.Count < 2)
        {
            throw new ArgumentException(
                "A question must have at least two options.");
        }

        if (options.Any(o =>
            string.IsNullOrWhiteSpace(o.OptionText)))
        {
            throw new ArgumentException(
                "Option text is required.");
        }

        var correctCount =
            options.Count(o => o.IsCorrect);

        if (correctCount != 1)
        {
            throw new ArgumentException(
                "A question must have exactly one correct option.");
        }
    }
}