using HealthPatientApi.Attributes;

namespace HealthPatientApi.Models;

/// <summary>
/// Patient entity — stores Protected Health Information (PHI)
/// FIXED: PHI fields marked with [EncryptedField] for AES-256 encryption
/// FIXED: Added data classification attributes per HIPAA §164.312(a)(2)(i)
/// </summary>
public class Patient
{
    public int Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public DateTime DateOfBirth { get; set; }

    // FIXED: SSN marked for encryption at rest (AES-256)
    // HIPAA §164.312(a)(2)(i) — Encryption and Decryption
    [EncryptedField("Social Security Number is PHI and must be encrypted at rest")]
    public string SocialSecurityNumber { get; set; } = string.Empty;

    // FIXED: Medical data marked for encryption
    public string MedicalRecordNumber { get; set; } = string.Empty;

    [EncryptedField("Diagnosis is PHI and must be encrypted at rest")]
    public string Diagnosis { get; set; } = string.Empty;

    [EncryptedField("Treatment notes are PHI and must be encrypted at rest")]
    public string TreatmentNotes { get; set; } = string.Empty;

    // FIXED: Insurance data marked for encryption
    public string InsuranceProvider { get; set; } = string.Empty;

    [EncryptedField("Insurance policy number is PHI and must be encrypted at rest")]
    public string InsurancePolicyNumber { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
}
