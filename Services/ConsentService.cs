using HealthPatientApi.Models;

namespace HealthPatientApi.Services;

/// <summary>
/// Consent management service for patient data processing authorization.
/// Implements GDPR Article 9 and HIPAA consent requirements.
/// </summary>
public interface IConsentService
{
    /// <summary>
    /// Gets all active consents for a patient.
    /// </summary>
    IEnumerable<PatientConsent> GetPatientConsents(int patientId);

    /// <summary>
    /// Gets specific consent by ID.
    /// </summary>
    PatientConsent? GetConsentById(int consentId);

    /// <summary>
    /// Creates a new patient consent record.
    /// </summary>
    PatientConsent CreateConsent(PatientConsent consent);

    /// <summary>
    /// Revokes a patient's consent.
    /// </summary>
    bool RevokeConsent(int consentId);

    /// <summary>
    /// Verifies if patient has active consent for a specific type of data processing.
    /// </summary>
    bool HasActiveConsent(int patientId, string consentType);

    /// <summary>
    /// Gets the lawful basis for processing a patient's data.
    /// </summary>
    string? GetDataProcessingBasis(int patientId, string consentType);
}

public class ConsentService : IConsentService
{
    /// <summary>
    /// In-memory consent storage.
    /// In production, this should be persisted to encrypted database.
    /// </summary>
    private static readonly List<PatientConsent> _consents = new()
    {
        // Sample consent for demo patient
        new PatientConsent
        {
            Id = 1,
            PatientId = 1,
            ConsentType = "Treatment",
            Description = "Consent for medical treatment and clinical care",
            GrantedDate = DateTime.UtcNow.AddDays(-30),
            DataProcessingBasis = "Consent",
            ConsentVersion = 1,
            ConsentText = "I consent to treatment and storage of my medical information for clinical care purposes.",
            ObtainedBy = "Dr. Smith",
            IpAddress = "192.168.1.1"
        }
    };

    private static int _nextId = 2;
    private readonly ILogger<ConsentService> _logger;

    public ConsentService(ILogger<ConsentService> logger)
    {
        _logger = logger;
    }

    public IEnumerable<PatientConsent> GetPatientConsents(int patientId)
    {
        return _consents
            .Where(c => c.PatientId == patientId && c.IsActive)
            .OrderByDescending(c => c.GrantedDate);
    }

    public PatientConsent? GetConsentById(int consentId)
    {
        return _consents.FirstOrDefault(c => c.Id == consentId);
    }

    public PatientConsent CreateConsent(PatientConsent consent)
    {
        if (consent == null)
            throw new ArgumentNullException(nameof(consent));

        if (consent.PatientId <= 0)
            throw new InvalidOperationException("Patient ID must be valid.");

        if (string.IsNullOrEmpty(consent.ConsentType))
            throw new InvalidOperationException("Consent type is required.");

        consent.Id = _nextId++;
        consent.GrantedDate = DateTime.UtcNow;
        consent.ObtainedBy ??= "Unknown";

        _consents.Add(consent);

        _logger.LogInformation(
            "Consent granted for Patient {PatientId}, Type: {ConsentType}, Basis: {Basis}",
            consent.PatientId,
            consent.ConsentType,
            consent.DataProcessingBasis
        );

        return consent;
    }

    public bool RevokeConsent(int consentId)
    {
        var consent = _consents.FirstOrDefault(c => c.Id == consentId);
        if (consent == null)
            return false;

        consent.RevokedDate = DateTime.UtcNow;

        _logger.LogWarning(
            "Consent revoked - PatientId: {PatientId}, ConsentType: {ConsentType}",
            consent.PatientId,
            consent.ConsentType
        );

        return true;
    }

    public bool HasActiveConsent(int patientId, string consentType)
    {
        var hasConsent = _consents.Any(c =>
            c.PatientId == patientId &&
            c.ConsentType == consentType &&
            c.IsActive
        );

        if (!hasConsent)
        {
            _logger.LogWarning(
                "No active consent found - PatientId: {PatientId}, ConsentType: {ConsentType}",
                patientId,
                consentType
            );
        }

        return hasConsent;
    }

    public string? GetDataProcessingBasis(int patientId, string consentType)
    {
        var consent = _consents.FirstOrDefault(c =>
            c.PatientId == patientId &&
            c.ConsentType == consentType &&
            c.IsActive
        );

        return consent?.DataProcessingBasis;
    }
}
