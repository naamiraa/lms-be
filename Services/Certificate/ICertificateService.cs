using be.DTOs.Certificate;

namespace be.Services.Certificate;

public interface ICertificateService
{
    Task<CertificateResponse> IssueAsync(
        Guid userId,
        Guid simulationSessionId);

    Task<CertificateResponse?> GetBySessionAsync(
        Guid userId,
        Guid simulationSessionId);

    Task<List<CertificateResponse>> GetMyCertificatesAsync(
        Guid userId);

    Task<CertificateResponse?> GetByIdAsync(
        Guid userId,
        Guid certificateId);
}
