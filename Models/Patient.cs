namespace HealthPatientApi.Models;

/// <summary>
/// Patient entity — stores Protected Health Information (PHI)
/// VIOLATION: PHI fields stored as plain text without encryption
/// VIOLATION: SSN stored directly — should be masked/hashed
/// VIOLATION: No data classification attributes
/// </summary>
public class Patient
{
    public int Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public DateTime DateOfBirth { get; set; }

    // HIPAA VIOLATION: SSN in plain text, no encryption
    public string SocialSecurityNumber { get; set; } = string.Empty;

    // HIPAA VIOLATION: Medical data without encryption at rest
    public string MedicalRecordNumber { get; set; } = string.Empty;
    public string Diagnosis { get; set; } = string.Empty;
    public string TreatmentNotes { get; set; } = string.Empty;

    // HIPAA VIOLATION: Insurance data in plain text
    public string InsuranceProvider { get; set; } = string.Empty;
    public string InsurancePolicyNumber { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
}
