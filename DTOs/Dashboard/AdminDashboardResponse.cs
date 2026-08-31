namespace be.DTOs.Dashboard;

public class AdminDashboardResponse
{
    public int TotalParticipants { get; set; }

    public int TotalInstructors { get; set; }

    public int PendingUserApprovals { get; set; }

    public int CompletedSimulations { get; set; }

    public int CertificatesIssued { get; set; }

    public double PassRate { get; set; }

    public double AverageOverallScore { get; set; }

    public int PassedSimulations { get; set; }

    public int FailedSimulations { get; set; }

    public List<ScenarioPerformanceResponse> ScenarioPerformance { get; set; }
        = new();
}

public class ScenarioPerformanceResponse
{
    public Guid ScenarioId { get; set; }

    public string ScenarioName { get; set; } = string.Empty;

    public int CompletedSimulations { get; set; }

    public double AverageOverallScore { get; set; }

    public double PassRate { get; set; }
}