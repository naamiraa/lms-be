using be.Data;
using be.DTOs.TrainingHistory;
using be.Services.Evaluation;
using Microsoft.EntityFrameworkCore;

namespace be.Services.TrainingHistory;

public class TrainingHistoryService : ITrainingHistoryService
{
    private readonly AppDbContext _context;
    private readonly IEvaluationCalculator _evaluationCalculator;

    public TrainingHistoryService(
        AppDbContext context,
        IEvaluationCalculator evaluationCalculator)
    {
        _context = context;
        _evaluationCalculator = evaluationCalculator;
    }

    public async Task<List<TrainingHistoryItemResponse>> GetMyHistoryAsync(
        Guid userId)
    {
        var sessions = await _context.SimulationSessions
            .Include(s => s.SimulationScenario)
                .ThenInclude(sc => sc.SimulationDomain)
            .Include(s => s.AssessmentAttempt)
                .ThenInclude(a => a.Assessment)
                    .ThenInclude(a => a.LearningModule)
                        .ThenInclude(m => m.TrainingDomain)
            .Include(s => s.Result)
            .Include(s => s.InstructorFeedback)
            .Where(s =>
                s.UserId == userId &&
                s.Status == "COMPLETED" &&
                s.Result != null &&
                s.AssessmentAttempt.Score != null)
            .OrderByDescending(s => s.CompletedAt)
            .ToListAsync();

        return sessions.Select(s =>
        {
            var theoryScore = s.AssessmentAttempt.Score!.Value;
            var simulatorScore = s.Result!.OverallSimulatorScore;

            var overallScore =
            _evaluationCalculator.CalculateOverallScore(
                theoryScore,
                simulatorScore);

            return new TrainingHistoryItemResponse
            {
                AssessmentAttemptId = s.AssessmentAttemptId,
                SimulationSessionId = s.Id,

                TrainingDomainName =
                    s.AssessmentAttempt
                        .Assessment
                        .LearningModule
                        .TrainingDomain
                        .Name,

                LearningModuleName =
                    s.AssessmentAttempt
                        .Assessment
                        .LearningModule
                        .Name,

                AssessmentTitle =
                    s.AssessmentAttempt.Assessment.Title,

                ScenarioName =
                    s.SimulationScenario.Name,

                TheoryScore = theoryScore,
                SimulatorScore = simulatorScore,
                OverallScore = overallScore,

                Result = _evaluationCalculator.DetermineResult(overallScore),

                CompletedAt =
                    s.CompletedAt ?? s.UpdatedAt,

                HasInstructorFeedback =
                    s.InstructorFeedback != null
            };
        }).ToList();
    }


    public async Task<TrainingHistoryDetailResponse?> GetDetailAsync(
        Guid userId,
        Guid sessionId)
    {
        var session = await _context.SimulationSessions
            .Include(s => s.SimulationScenario)
                .ThenInclude(sc => sc.SimulationDomain)
            .Include(s => s.AssessmentAttempt)
                .ThenInclude(a => a.Assessment)
                    .ThenInclude(a => a.LearningModule)
                        .ThenInclude(m => m.TrainingDomain)
            .Include(s => s.Result)
            .Include(s => s.InstructorFeedback)
            .FirstOrDefaultAsync(s =>
                s.Id == sessionId &&
                s.UserId == userId &&
                s.Status == "COMPLETED");

        if (session == null ||
            session.Result == null ||
            session.AssessmentAttempt.Score == null)
        {
            return null;
        }

        var theoryScore = session.AssessmentAttempt.Score.Value;
        var simulatorScore = session.Result.OverallSimulatorScore;

        var overallScore =
        _evaluationCalculator.CalculateOverallScore(
            theoryScore,
            simulatorScore);

        return new TrainingHistoryDetailResponse
        {
            AssessmentAttemptId =
                session.AssessmentAttemptId,

            SimulationSessionId =
                session.Id,

            TrainingDomainName =
                session.AssessmentAttempt
                    .Assessment
                    .LearningModule
                    .TrainingDomain
                    .Name,

            LearningModuleName =
                session.AssessmentAttempt
                    .Assessment
                    .LearningModule
                    .Name,

            AssessmentTitle =
                session.AssessmentAttempt.Assessment.Title,

            ScenarioName =
                session.SimulationScenario.Name,

            TheoryScore =
                theoryScore,

            SimulatorScore =
                simulatorScore,

            OverallScore =
                overallScore,

            Result =
                _evaluationCalculator.DetermineResult(overallScore),

            CompletedAt =
                session.CompletedAt ?? session.UpdatedAt,

            TotalViolations =
                session.Result.TotalViolations,

            SafetyScore =
                session.Result.SafetyScore,

            VehicleControlScore =
                session.Result.VehicleControlScore,

            EmergencyResponseScore =
                session.Result.EmergencyResponseScore,

            CompletionTimeSeconds =
                session.Result.CompletionTimeSeconds,

            HasInstructorFeedback =
                session.InstructorFeedback != null
        };
    }

}
