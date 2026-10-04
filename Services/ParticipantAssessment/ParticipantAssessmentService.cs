using be.Data;
using be.DTOs.ParticipantAssessment;
using Microsoft.EntityFrameworkCore;

namespace be.Services.ParticipantAssessment;

public class ParticipantAssessmentService
    : IParticipantAssessmentService
{
    private readonly AppDbContext _context;

    public ParticipantAssessmentService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<StartAssessmentResponse> StartAssessmentAsync(
        Guid userId,
        Guid assessmentId)
    {
        var assessment = await _context.Assessments
            .Include(a => a.LearningModule)
            .Include(a => a.Questions.Where(q => q.IsActive))
                .ThenInclude(q => q.Options.Where(o => o.IsActive))
            .FirstOrDefaultAsync(a =>
                a.Id == assessmentId &&
                a.IsActive);

        if (assessment == null)
        {
            throw new ArgumentException(
                "Assessment not found or inactive.");
        }

        if (!assessment.LearningModule.IsActive)
        {
            throw new InvalidOperationException(
                "Learning module is inactive.");
        }

        if (!assessment.Questions.Any())
        {
            throw new InvalidOperationException(
                "Assessment has no active questions.");
        }

        /*
         * Kalau participant sebelumnya sudah menekan Start
         * tetapi belum submit, jangan membuat attempt baru.
         */
        var existingAttempt = await _context.AssessmentAttempts
            .Include(a => a.Answers)
            .FirstOrDefaultAsync(a =>
                a.AssessmentId == assessmentId &&
                a.UserId == userId &&
                a.Status == "IN_PROGRESS");

        if (existingAttempt != null)
        {
            var expiresAt = existingAttempt.StartedAt
                .AddMinutes(assessment.DurationMinutes);

            if (DateTime.UtcNow <= expiresAt)
            {
                return BuildResponse(
                    assessment,
                    existingAttempt);
            }

            existingAttempt.Status = "EXPIRED";
            existingAttempt.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
        }

        var previousAttempts = await _context.AssessmentAttempts
            .CountAsync(a =>
                a.AssessmentId == assessmentId &&
                a.UserId == userId);

        if (previousAttempts >= assessment.MaxAttempts)
        {
            throw new InvalidOperationException(
                "Maximum assessment attempts reached.");
        }

        var attempt = new Models.AssessmentAttempt
        {
            AssessmentId = assessment.Id,
            UserId = userId,
            AttemptNumber = previousAttempts + 1,
            Status = "IN_PROGRESS",
            StartedAt = DateTime.UtcNow
        };

        _context.AssessmentAttempts.Add(attempt);

        await _context.SaveChangesAsync();

        return BuildResponse(
            assessment,
            attempt);
    }


    public async Task<StartAssessmentResponse> GetAttemptAsync(
        Guid userId,
        Guid attemptId)
    {
        var attempt = await _context.AssessmentAttempts
            .Include(a => a.Assessment)
                .ThenInclude(a => a.LearningModule)
            .Include(a => a.Assessment)
                .ThenInclude(a => a.Questions.Where(q => q.IsActive))
                    .ThenInclude(q => q.Options.Where(o => o.IsActive))
            .Include(a => a.Answers)
            .FirstOrDefaultAsync(a =>
                a.Id == attemptId &&
                a.UserId == userId);

        if (attempt == null)
        {
            throw new ArgumentException(
                "Assessment attempt not found.");
        }

        if (attempt.Status == "IN_PROGRESS")
        {
            var expiresAt = attempt.StartedAt
                .AddMinutes(
                    attempt.Assessment.DurationMinutes);

            if (DateTime.UtcNow > expiresAt)
            {
                attempt.Status = "EXPIRED";
                attempt.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();
            }
        }

        return BuildResponse(
            attempt.Assessment,
            attempt);
    }


    public async Task<SaveAssessmentAnswerResponse> SaveAnswerAsync(
        Guid userId,
        Guid attemptId,
        Guid questionId,
        SaveAssessmentAnswerRequest request)
    {
        var attempt = await _context.AssessmentAttempts
            .Include(a => a.Assessment)
            .FirstOrDefaultAsync(a =>
                a.Id == attemptId &&
                a.UserId == userId);

        if (attempt == null)
        {
            throw new ArgumentException(
                "Assessment attempt not found.");
        }

        if (attempt.Status != "IN_PROGRESS")
        {
            throw new InvalidOperationException(
                "Assessment attempt is no longer in progress.");
        }

        var expiresAt = attempt.StartedAt
            .AddMinutes(attempt.Assessment.DurationMinutes);

        if (DateTime.UtcNow > expiresAt)
        {
            throw new InvalidOperationException(
                "Assessment time has expired.");
        }

        var question = await _context.AssessmentQuestions
            .FirstOrDefaultAsync(q =>
                q.Id == questionId &&
                q.AssessmentId == attempt.AssessmentId &&
                q.IsActive);

        if (question == null)
        {
            throw new ArgumentException(
                "Question does not belong to this assessment.");
        }

        var option = await _context.QuestionOptions
            .FirstOrDefaultAsync(o =>
                o.Id == request.SelectedOptionId &&
                o.AssessmentQuestionId == questionId &&
                o.IsActive);

        if (option == null)
        {
            throw new ArgumentException(
                "Selected option does not belong to this question.");
        }

        var answer = await _context.AssessmentAnswers
            .FirstOrDefaultAsync(a =>
                a.AssessmentAttemptId == attemptId &&
                a.AssessmentQuestionId == questionId);

        if (answer == null)
        {
            answer = new Models.AssessmentAnswer
            {
                AssessmentAttemptId = attemptId,
                AssessmentQuestionId = questionId,
                SelectedOptionId = request.SelectedOptionId
            };

            _context.AssessmentAnswers.Add(answer);
        }
        else
        {
            answer.SelectedOptionId = request.SelectedOptionId;
            answer.UpdatedAt = DateTime.UtcNow;
        }

        await _context.SaveChangesAsync();

        return new SaveAssessmentAnswerResponse
        {
            QuestionId = questionId,
            SelectedOptionId = answer.SelectedOptionId,
            SavedAt = answer.UpdatedAt
        };
    }


    public async Task<SubmitAssessmentResponse> SubmitAssessmentAsync(
        Guid userId,
        Guid attemptId)
    {
        var attempt = await _context.AssessmentAttempts
            .Include(a => a.Assessment)
                .ThenInclude(a => a.Questions)
                    .ThenInclude(q => q.Options)
            .Include(a => a.Answers)
                .ThenInclude(a => a.SelectedOption)
            .FirstOrDefaultAsync(a =>
                a.Id == attemptId &&
                a.UserId == userId);

        if (attempt == null)
        {
            throw new ArgumentException(
                "Assessment attempt not found.");
        }

        if (attempt.Status != "IN_PROGRESS")
        {
            throw new InvalidOperationException(
                "Assessment attempt is no longer in progress.");
        }

        var expiresAt = attempt.StartedAt
            .AddMinutes(attempt.Assessment.DurationMinutes);

        if (DateTime.UtcNow > expiresAt)
        {
            throw new InvalidOperationException(
                "Assessment time has expired.");
        }

        var activeQuestions = attempt.Assessment.Questions
            .Where(q => q.IsActive)
            .ToList();

        if (activeQuestions.Count == 0)
        {
            throw new InvalidOperationException(
                "Assessment has no active questions.");
        }

        var answeredQuestionIds = attempt.Answers
            .Select(a => a.AssessmentQuestionId)
            .ToHashSet();

        var unansweredCount = activeQuestions.Count(q =>
            !answeredQuestionIds.Contains(q.Id));

        if (unansweredCount > 0)
        {
            throw new InvalidOperationException(
                $"Please answer all questions before submitting. " +
                $"{unansweredCount} question(s) remaining.");
        }

        var correctAnswers = attempt.Answers.Count(a =>
            activeQuestions.Any(q =>
                q.Id == a.AssessmentQuestionId) &&
            a.SelectedOption.IsCorrect);

        var score = Math.Round(
            (double)correctAnswers / activeQuestions.Count * 100,
            2);

        attempt.Score = score;
        attempt.Status = "COMPLETED";
        attempt.SubmittedAt = DateTime.UtcNow;
        attempt.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return new SubmitAssessmentResponse
        {
            AttemptId = attempt.Id,
            Status = attempt.Status,
            AssessmentCompleted = true,
            SimulationUnlocked = true,
            Message =
                "Assessment submitted successfully. Continue to simulation."
        };
    }

    private static StartAssessmentResponse BuildResponse(
        Models.Assessment assessment,
        Models.AssessmentAttempt attempt)
    {
        return new StartAssessmentResponse
        {
            AttemptId = attempt.Id,
            AssessmentId = assessment.Id,
            AssessmentTitle = assessment.Title,
            AttemptNumber = attempt.AttemptNumber,
            MaxAttempts = assessment.MaxAttempts,
            DurationMinutes = assessment.DurationMinutes,
            StartedAt = attempt.StartedAt,
            ExpiresAt = attempt.StartedAt
                .AddMinutes(assessment.DurationMinutes),
            Status = attempt.Status,

            Questions = assessment.Questions
                .OrderBy(q => q.Order)
                .Select(q => new ParticipantQuestionResponse
                {
                    Id = q.Id,
                    QuestionText = q.QuestionText,
                    MediaUrl = q.MediaUrl,
                    Order = q.Order,

                    SelectedOptionId = attempt.Answers
                        .Where(a =>
                            a.AssessmentQuestionId == q.Id)
                        .Select(a =>
                            (Guid?)a.SelectedOptionId)
                        .FirstOrDefault(),

                    Options = q.Options
                        .OrderBy(o => o.Order)
                        .Select(o =>
                            new ParticipantOptionResponse
                            {
                                Id = o.Id,
                                OptionText = o.OptionText,
                                Order = o.Order
                            })
                        .ToList()
                })
                .ToList()
        };
    }
}