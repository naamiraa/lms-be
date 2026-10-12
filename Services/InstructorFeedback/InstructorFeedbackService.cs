using be.Data;
using be.DTOs.InstructorFeedback;
using Microsoft.EntityFrameworkCore;

namespace be.Services.InstructorFeedback;

public class InstructorFeedbackService : IInstructorFeedbackService
{
    private readonly AppDbContext _context;

    public InstructorFeedbackService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<InstructorFeedbackResponse> CreateAsync(
        Guid instructorId,
        CreateInstructorFeedbackRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.OverallComments))
        {
            throw new ArgumentException(
                "Overall comments are required.");
        }

        var session = await _context.SimulationSessions
            .Include(s => s.Events)
            .FirstOrDefaultAsync(s =>
                s.Id == request.SimulationSessionId);

        if (session == null)
        {
            throw new ArgumentException(
                "Simulation session not found.");
        }

        if (session.Status != "COMPLETED")
        {
            throw new ArgumentException(
                "Feedback can only be submitted for a completed simulation.");
        }

        var existingFeedback = await _context.InstructorFeedbacks
            .AnyAsync(f =>
                f.SimulationSessionId == request.SimulationSessionId);

        if (existingFeedback)
        {
            throw new ArgumentException(
                "Instructor feedback already exists for this simulation session.");
        }

        foreach (var explanation in request.ViolationExplanations)
        {
            var eventExists = session.Events.Any(e =>
                e.Id == explanation.SimulationEventId);

            if (!eventExists)
            {
                throw new ArgumentException(
                    "One or more simulation events do not belong to this session.");
            }
        }

        var moduleIds = request.RecommendedModules
            .Select(r => r.LearningModuleId)
            .Distinct()
            .ToList();

        if (moduleIds.Count > 0)
        {
            var validModuleCount = await _context.LearningModules
                .CountAsync(m =>
                    moduleIds.Contains(m.Id) &&
                    m.IsActive);

            if (validModuleCount != moduleIds.Count)
            {
                throw new ArgumentException(
                    "One or more recommended learning modules are invalid or inactive.");
            }
        }

        var feedback = new Models.InstructorFeedback
        {
            SimulationSessionId = session.Id,
            InstructorId = instructorId,
            OverallComments = request.OverallComments.Trim()
        };

        foreach (var item in request.Strengths
                     .Where(s => !string.IsNullOrWhiteSpace(s))
                     .Select((value, index) => new { value, index }))
        {
            feedback.Strengths.Add(
                new Models.FeedbackStrength
                {
                    Description = item.value.Trim(),
                    Order = item.index + 1
                });
        }

        foreach (var item in request.ImprovementAreas
                     .Where(s => !string.IsNullOrWhiteSpace(s))
                     .Select((value, index) => new { value, index }))
        {
            feedback.ImprovementAreas.Add(
                new Models.FeedbackImprovementArea
                {
                    Description = item.value.Trim(),
                    Order = item.index + 1
                });
        }

        foreach (var recommendation in request.SafetyRecommendations)
        {
            if (string.IsNullOrWhiteSpace(recommendation.Description))
            {
                continue;
            }

            feedback.SafetyRecommendations.Add(
                new Models.SafetyRecommendation
                {
                    Description = recommendation.Description.Trim(),
                    Priority = recommendation.Priority
                });
        }

        foreach (var explanation in request.ViolationExplanations)
        {
            if (string.IsNullOrWhiteSpace(explanation.Explanation))
            {
                continue;
            }

            feedback.ViolationExplanations.Add(
                new Models.ViolationExplanation
                {
                    SimulationEventId = explanation.SimulationEventId,
                    Explanation = explanation.Explanation.Trim()
                });
        }

        foreach (var module in request.RecommendedModules)
        {
            feedback.RecommendedModules.Add(
                new Models.FeedbackRecommendedModule
                {
                    LearningModuleId = module.LearningModuleId,
                    RecommendationType =
                        module.RecommendationType?.Trim().ToUpperInvariant(),
                    Order = module.Order
                });
        }

        _context.InstructorFeedbacks.Add(feedback);

        await _context.SaveChangesAsync();

        return await GetFeedbackResponseAsync(feedback.Id)
            ?? throw new InvalidOperationException(
                "Failed to retrieve created instructor feedback.");
    }

    public async Task<InstructorFeedbackResponse?> GetBySessionIdAsync(
        Guid userId,
        Guid sessionId,
        bool isInstructor)
    {
        var session = await _context.SimulationSessions
            .FirstOrDefaultAsync(s => s.Id == sessionId);

        if (session == null)
        {
            return null;
        }

        // Participant may only see their own feedback
        if (!isInstructor && session.UserId != userId)
        {
            return null;
        }

        var feedbackId = await _context.InstructorFeedbacks
            .Where(f => f.SimulationSessionId == sessionId)
            .Select(f => (Guid?)f.Id)
            .FirstOrDefaultAsync();

        if (feedbackId == null)
        {
            return null;
        }

        return await GetFeedbackResponseAsync(feedbackId.Value);
    }

    private async Task<InstructorFeedbackResponse?>
        GetFeedbackResponseAsync(Guid feedbackId)
    {
        var feedback = await _context.InstructorFeedbacks
            .Include(f => f.Instructor)
            .Include(f => f.SimulationSession)
                .ThenInclude(s => s.SimulationScenario)
            .Include(f => f.Strengths)
            .Include(f => f.ImprovementAreas)
            .Include(f => f.SafetyRecommendations)
            .Include(f => f.ViolationExplanations)
                .ThenInclude(v => v.SimulationEvent)
            .Include(f => f.RecommendedModules)
                .ThenInclude(r => r.LearningModule)
            .FirstOrDefaultAsync(f => f.Id == feedbackId);

        if (feedback == null)
        {
            return null;
        }

        return new InstructorFeedbackResponse
        {
            Id = feedback.Id,
            SimulationSessionId = feedback.SimulationSessionId,
            InstructorId = feedback.InstructorId,
            InstructorName = feedback.Instructor.Name,
            ScenarioName =
                feedback.SimulationSession.SimulationScenario.Name,
            OverallComments = feedback.OverallComments,
            EvaluatedAt = feedback.EvaluatedAt,

            Strengths = feedback.Strengths
                .OrderBy(s => s.Order)
                .Select(s => s.Description)
                .ToList(),

            ImprovementAreas = feedback.ImprovementAreas
                .OrderBy(a => a.Order)
                .Select(a => a.Description)
                .ToList(),

            SafetyRecommendations = feedback.SafetyRecommendations
                .OrderBy(r => r.Priority)
                .Select(r => new SafetyRecommendationResponse
                {
                    Description = r.Description,
                    Priority = r.Priority
                })
                .ToList(),

            ViolationExplanations = feedback.ViolationExplanations
                .Select(v => new ViolationExplanationResponse
                {
                    SimulationEventId = v.SimulationEventId,
                    EventType = v.SimulationEvent.EventType,
                    Severity = v.SimulationEvent.Severity,
                    Explanation = v.Explanation
                })
                .ToList(),

            RecommendedModules = feedback.RecommendedModules
                .OrderBy(r => r.Order)
                .Select(r => new RecommendedModuleResponse
                {
                    LearningModuleId = r.LearningModuleId,
                    LearningModuleName = r.LearningModule.Name,
                    RecommendationType = r.RecommendationType,
                    Order = r.Order
                })
                .ToList()
        };
    }
}