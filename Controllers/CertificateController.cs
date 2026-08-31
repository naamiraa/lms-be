using System.Security.Claims;
using be.Services.Certificate;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace be.Controllers;

[ApiController]
[Route("api/certificates")]
[Authorize(Roles = "PARTICIPANT")]
public class CertificateController : ControllerBase
{
    private readonly ICertificateService _service;
    private readonly ICertificatePdfService _pdfService;

    public CertificateController(
        ICertificateService service,
        ICertificatePdfService pdfService)
    {
        _service = service;
        _pdfService = pdfService;
    }

    [HttpPost("session/{sessionId:guid}")]
    public async Task<IActionResult> Issue(Guid sessionId)
    {
        try
        {
            var userId = GetCurrentUserId();

            var result = await _service.IssueAsync(
                userId,
                sessionId);

            return Ok(result);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new
            {
                message = ex.Message
            });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new
            {
                message = ex.Message
            });
        }
    }

    [HttpGet]
    public async Task<IActionResult> GetMine()
    {
        var result = await _service
            .GetMyCertificatesAsync(
                GetCurrentUserId());

        return Ok(result);
    }

    [HttpGet("session/{sessionId:guid}")]
    public async Task<IActionResult> GetBySession(
        Guid sessionId)
    {
        var result =
            await _service.GetBySessionAsync(
                GetCurrentUserId(),
                sessionId);

        if (result == null)
        {
            return NotFound(new
            {
                message = "Certificate not found."
            });
        }

        return Ok(result);
    }

    private Guid GetCurrentUserId()
    {
        var value =
            User.FindFirstValue(
                ClaimTypes.NameIdentifier);

        if (!Guid.TryParse(value, out var userId))
        {
            throw new UnauthorizedAccessException(
                "Invalid user identifier.");
        }

        return userId;
    }


    [HttpGet("{certificateId:guid}/pdf")]
    public async Task<IActionResult> DownloadPdf(
        Guid certificateId)
    {
        var certificate =
            await _service.GetByIdAsync(
                GetCurrentUserId(),
                certificateId);

        if (certificate == null)
        {
            return NotFound(new
            {
                message = "Certificate not found."
            });
        }

        var pdf =
            _pdfService.GenerateCertificatePdf(
                certificate.CertificateNumber,
                certificate.ParticipantName,
                certificate.LearningModuleName,
                certificate.ScenarioName,
                certificate.OverallScore,
                certificate.IssuedAt);

        var fileName =
            $"{certificate.CertificateNumber}.pdf";

        return File(
            pdf,
            "application/pdf",
            fileName);
    }

}
