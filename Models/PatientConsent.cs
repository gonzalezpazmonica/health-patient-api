namespace HealthPatientApi.Models;

/// <summary>
/// Patient consent record for data processing.
/// Implements GDPR Article 9 and HIPAA consent requirements.
/// All health data processing requires explicit documented consent.
/// </summary>
public class PatientConsent
{
    /// <summary>
    /// Unique consent record identifier.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Patient ID associated with this consent.
    /// </summary>
    public int PatientId { get; set; }

    /// <summary>
    /// Type of consent: DataProcessing, Treatment, Research, Marketing, etc.
    /// </summary>
    public string ConsentType { get; set; } = string.Empty;

    /// <summary>
    /// Description of what the consent authorizes.
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Date consent was granted by patient.
    /// </summary>
    public DateTime GrantedDate { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Date consent was revoked (if applicable).
    /// Null indicates consent is still active.
    /// </summary>
    public DateTime? RevokedDate { get; set; }

    /// <summary>
    /// Lawful basis for processing: Consent, Contract, LegalObligation, VitalInterest, PublicTask, LegitimateInterest.
    /// Required by GDPR Article 6.
    /// </summary>
    public string DataProcessingBasis { get; set; } = "Consent";

    /// <summary>
    /// Version of the consent form signed by patient.
    /// For tracking updates to consent terms.
    /// </summary>
    public int ConsentVersion { get; set; } = 1;

    /// <summary>
    /// Consent form text that patient acknowledged.
    /// Stored for audit trail and dispute resolution.
    /// </summary>
    public string ConsentText { get; set; } = string.Empty;

    /// <summary>
    /// User ID or name of person who obtained consent.
    /// For accountability tracking.
    /// </summary>
    public string ObtainedBy { get; set; } = string.Empty;

    /// <summary>
    /// IP address from which consent was provided.
    /// For dispute resolution.
    /// </summary>
    public string IpAddress { get; set; } = string.Empty;

    /// <summary>
    /// Expiration date of consent (if applicable).
    /// Null means no expiration.
    /// </summary>
    public DateTime? ExpirationDate { get; set; }

    /// <summary>
    /// Indicates if consent is currently active.
    /// False if revoked or expired.
    /// </summary>
    public bool IsActive => RevokedDate == null && (ExpirationDate == null || ExpirationDate > DateTime.UtcNow);
}
