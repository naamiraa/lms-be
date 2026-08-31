namespace be.Services.Certificate;

public interface ICertificatePdfService
{
    byte[] GenerateCertificatePdf(
        string certificateNumber,
        string participantName,
        string learningModuleName,
        string scenarioName,
        double overallScore,
        DateTime issuedAt);
}
