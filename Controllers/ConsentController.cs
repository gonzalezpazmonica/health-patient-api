using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using HealthPatientApi.Models;
using HealthPatientApi.Services;

namespace HealthPatientApi.Controllers;

/// <summary>
/// Patient consent management endpoints.
/// Implements GDPR Article 9 and HIPAA patient consent requirements.
/// All health data processing requires explicit documented consent.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class ConsentController : ControllerBase
{
    private readonly IConsentService _consentService;
    private readonly ILogger<ConsentController> _logger;

    public ConsentController(IConsentService consentService, ILogger<ConsentController> logger)
    {
        _consentService = consentService;
        _logger = logger;
    }

    /// <summary>
    /// Get all active consents for a patient.
    /// GDPR Article 9: Patient has right to know what consents are active.
    /// </summary>
    [HttpGet("patient/{patientId}")]
    [Authorize(Roles = "Clinician,Admin,Patient")]
    public ActionResult<IEnumerable<PatientConsent>> GetPatientConsents(int patientId)
    {
        var consents = _consentService.GetPatientConsents(patientId);
        return Ok(consents);
    }

    /// <summary>
    /// Get specific consent record by ID.
    /// </summary>
    [HttpGet("{id}")]
    [Authorize(Roles = "Clinician,Admin,Patient")]
    public ActionResult<PatientConsent> GetConsent(int id)
    {
        var consent = _consentService.GetConsentById(id);
        if (consent == null)
            return NotFound();

        return Ok(consent);
    }

    /// <summary>
    /// Create new patient consent.
    /// GDPR Article 7: Consent request must be freely given, specific, informed.
    /// Patient must explicitly opt-in to data processing.
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "Clinician,Admin,Patient")]
    public ActionResult<PatientConsent> CreateConsent([FromBody] PatientConsent consent)
    {
        if (consent == null)
            return BadRequest("Consent data required.");

        if (consent.PatientId <= 0)
            return BadRequest("Valid PatientId required.");

        if (string.IsNullOrEmpty(consent.ConsentType))
            return BadRequest("Consent type is required.");

        if (string.IsNullOrEmpty(consent.DataProcessingBasis))
            consent.DataProcessingBasis = "Consent";

        try
        {
            var created = _consentService.CreateConsent(consent);
            _logger.LogInformation(
                "Consent created - PatientId: {PatientId}, Type: {Type}",
                created.PatientId,
                created.ConsentType
            );
            return CreatedAtAction(nameof(GetConsent), new { id = created.Id }, created);
        }
        catch (Exception ex)
        {
            _logger.LogError("Error creating consent: {Message}", ex.Message);
            return StatusCode(500, "Error creating consent.");
        }
    }

    /// <summary>
    /// Revoke patient consent.
    /// GDPR Article 7: Patient has right to withdraw consent at any time.
    /// Revocation must be logged for audit trail.
    /// </summary>
    [HttpPost("{id}/revoke")]
    [Authorize(Roles = "Patient,Admin")]
    public ActionResult RevokeConsent(int id)
    {
        var success = _consentService.RevokeConsent(id);
        if (!success)
            return NotFound();

        _logger.LogInformation("Consent revoked - ConsentId: {ConsentId}", id);
        return NoContent();
    }

    /// <summary>
    /// Verify if patient has active consent for specific processing.
    /// Must be called before any PHI data access to ensure GDPR compliance.
    /// </summary>
    [HttpGet("verify/{patientId}/{consentType}")]
    [Authorize(Roles = "Clinician,Admin")]
    public ActionResult<bool> VerifyConsent(int patientId, string consentType)
    {
        var hasConsent = _consentService.HasActiveConsent(patientId, consentType);
        return Ok(new { hasConsent });
    }

    /// <summary>
    /// Get lawful basis for data processing.
    /// GDPR Article 6: Processing must have documented lawful basis.
    /// </summary>
    [HttpGet("basis/{patientId}/{consentType}")]
    [Authorize(Roles = "Clinician,Admin")]
    public ActionResult<string?> GetProcessingBasis(int patientId, string consentType)
    {
        var basis = _consentService.GetDataProcessingBasis(patientId, consentType);
        if (basis == null)
            return NotFound("No active consent found for this processing type.");

        return Ok(new { processingBasis = basis });
    }
}
