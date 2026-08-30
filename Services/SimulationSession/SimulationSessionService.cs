using be.Data;
using be.DTOs.SimulationSession;
using Microsoft.EntityFrameworkCore;

namespace be.Services.SimulationSession;

public class SimulationSessionService : ISimulationSessionService
{
    private readonly AppDbContext _context;

    public SimulationSessionService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<LaunchSimulationResponse> CreateSessionAsync(
        Guid userId,
        CreateSimulationSessionRequest request)
    {
        var attempt = await _context.AssessmentAttempts
            .FirstOrDefaultAsync(a =>
                a.Id == request.AssessmentAttemptId &&
                a.UserId == userId);

        if (attempt == null)
        {
            throw new ArgumentException(
                "Assessment attempt not found.");
        }

        if (attempt.Status != "COMPLETED")
        {
            throw new ArgumentException(
                "Assessment must be completed before starting simulation.");
        }

        var scenario = await _context.SimulationScenarios
            .Include(s => s.SimulationDomain)
            .FirstOrDefaultAsync(s =>
                s.Id == request.SimulationScenarioId &&
                s.IsActive &&
                s.SimulationDomain.IsActive);

        if (scenario == null)
        {
            throw new ArgumentException(
                "Simulation scenario not found or inactive.");
        }

        var existingSession = await _context.SimulationSessions
            .FirstOrDefaultAsync(s =>
                s.UserId == userId &&
                s.AssessmentAttemptId == request.AssessmentAttemptId &&
                s.SimulationScenarioId == request.SimulationScenarioId &&
                s.Status != "COMPLETED" &&
                s.Status != "FAILED");

        if (existingSession != null)
        {
            return new LaunchSimulationResponse
            {
                SessionId = existingSession.Id,
                Status = existingSession.Status,
                ScenarioName = scenario.Name,
                Message = "Existing simulation session returned."
            };
        }

        var session = new Models.SimulationSession
        {
            UserId = userId,
            AssessmentAttemptId = request.AssessmentAttemptId,
            SimulationScenarioId = request.SimulationScenarioId,
            Status = "READY"
        };

        _context.SimulationSessions.Add(session);

        await _context.SaveChangesAsync();

        return new LaunchSimulationResponse
        {
            SessionId = session.Id,
            Status = session.Status,
            ScenarioName = scenario.Name,
            Message = "Simulation session created successfully."
        };
    }

    public async Task<SimulationSessionResponse?> GetByIdAsync(
        Guid userId,
        Guid sessionId)
    {
        return await _context.SimulationSessions
            .Where(s =>
                s.Id == sessionId &&
                s.UserId == userId)
            .Select(s => new SimulationSessionResponse
            {
                Id = s.Id,
                AssessmentAttemptId = s.AssessmentAttemptId,
                SimulationScenarioId = s.SimulationScenarioId,
                SimulationScenarioName =
                    s.SimulationScenario.Name,
                SimulationDomainName =
                    s.SimulationScenario.SimulationDomain.Name,
                Status = s.Status,
                StartedAt = s.StartedAt,
                CompletedAt = s.CompletedAt,
                CreatedAt = s.CreatedAt
            })
            .FirstOrDefaultAsync();
    }


    public async Task<StartSimulationResponse> StartSessionAsync(
        Guid sessionId)
    {
        var session = await _context.SimulationSessions
            .FirstOrDefaultAsync(s => s.Id == sessionId);

        if (session == null)
        {
            throw new ArgumentException(
                "Simulation session not found.");
        }

        if (session.Status != "READY")
        {
            throw new ArgumentException(
                "Only READY simulation sessions can be started.");
        }

        session.Status = "IN_PROGRESS";
        session.StartedAt = DateTime.UtcNow;
        session.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return new StartSimulationResponse
        {
            SessionId = session.Id,
            Status = session.Status,
            StartedAt = session.StartedAt.Value,
            Message = "Simulation session started successfully."
        };
    }


    public async Task<CompleteSimulationResponse> CompleteSessionAsync(
        Guid sessionId,
        CompleteSimulationRequest request)
    {
        var session = await _context.SimulationSessions
            .Include(s => s.Result)
            .Include(s => s.Events)
            .FirstOrDefaultAsync(s => s.Id == sessionId);

        if (session == null)
        {
            throw new ArgumentException(
                "Simulation session not found.");
        }

        if (session.Status != "IN_PROGRESS")
        {
            throw new ArgumentException(
                "Only IN_PROGRESS simulation sessions can be completed.");
        }

        if (request.OverallSimulatorScore < 0 ||
            request.OverallSimulatorScore > 100)
        {
            throw new ArgumentException(
                "Overall simulator score must be between 0 and 100.");
        }

        var result = new Models.SimulationResult
        {
            SimulationSessionId = session.Id,
            OverallSimulatorScore = request.OverallSimulatorScore,
            SafetyScore = request.SafetyScore,
            VehicleControlScore = request.VehicleControlScore,
            EmergencyResponseScore = request.EmergencyResponseScore,
            CompletionTimeSeconds = request.CompletionTimeSeconds,
            TotalViolations = request.Events.Count,
            ReceivedAt = DateTime.UtcNow
        };

        _context.SimulationResults.Add(result);

        foreach (var eventRequest in request.Events)
        {
            var simulationEvent = new Models.SimulationEvent
            {
                SimulationSessionId = session.Id,
                EventType = eventRequest.EventType,
                Severity = eventRequest.Severity,
                Description = eventRequest.Description,
                OccurredAtSeconds = eventRequest.OccurredAtSeconds,
                PenaltyPoints = eventRequest.PenaltyPoints
            };

            _context.SimulationEvents.Add(simulationEvent);
        }

        session.Status = "COMPLETED";
        session.CompletedAt = DateTime.UtcNow;
        session.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return new CompleteSimulationResponse
        {
            SessionId = session.Id,
            Status = session.Status,
            OverallSimulatorScore = result.OverallSimulatorScore,
            TotalViolations = result.TotalViolations,
            CompletedAt = session.CompletedAt.Value,
            Message = "Simulation result received successfully."
        };
    }
}