namespace be.DTOs.Certificate;

public class CertificateResponse
{
    public Guid Id { get; set; }

    public string CertificateNumber { get; set; } = string.Empty;

    public Guid SimulationSessionId { get; set; }

    public string ParticipantName { get; set; } = string.Empty;

    public string LearningModuleName { get; set; } = string.Empty;

    public string ScenarioName { get; set; } = string.Empty;

    public double OverallScore { get; set; }

    public string Result { get; set; } = string.Empty;

    public DateTime IssuedAt { get; set; }
}
