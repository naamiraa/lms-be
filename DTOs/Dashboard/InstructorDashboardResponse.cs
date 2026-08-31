namespace be.DTOs.Dashboard;

public class InstructorDashboardResponse
{
    public int CompletedSimulations { get; set; }

    public int PendingFeedback { get; set; }

    public int EvaluatedSessions { get; set; }

    public double AverageSimulatorScore { get; set; }

    public List<InstructorRecentSessionResponse> RecentSessions { get; set; }
        = new();
}

public class InstructorRecentSessionResponse
{
    public Guid SimulationSessionId { get; set; }

    public string ParticipantName { get; set; } = string.Empty;

    public string ScenarioName { get; set; } = string.Empty;

    public double SimulatorScore { get; set; }

    public DateTime? CompletedAt { get; set; }

    public bool HasInstructorFeedback { get; set; }

    public double OverallScore { get; set; }

    public string Result { get; set; } = string.Empty;
}
