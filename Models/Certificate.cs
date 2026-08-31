namespace be.Models;

public class Certificate
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public string CertificateNumber { get; set; } = string.Empty;

    public Guid UserId { get; set; }
    public User User { get; set; } = null!;

    public Guid SimulationSessionId { get; set; }
    public SimulationSession SimulationSession { get; set; } = null!;

    public DateTime IssuedAt { get; set; } = DateTime.UtcNow;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}
