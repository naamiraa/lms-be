using be.DTOs.TrainingHistory;

namespace be.DTOs.Dashboard;

public class ParticipantDashboardResponse
{
    public int CompletedLearningContents { get; set; }

    public int CompletedAssessments { get; set; }

    public int CompletedSimulations { get; set; }

    public int CertificatesEarned { get; set; }

    public TrainingHistoryItemResponse? LatestTraining { get; set; }

    public List<TrainingHistoryItemResponse> RecentTrainings { get; set; }
        = new();

    public int TotalLearningContents { get; set; }

    public double LearningProgressPercentage { get; set; }
}
