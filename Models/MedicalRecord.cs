namespace HealthPatientApi.Models;

/// <summary>
/// Medical record — contains sensitive clinical data
/// VIOLATION: No audit trail fields (CreatedBy, ModifiedBy, AccessedBy)
/// VIOLATION: No consent tracking
/// </summary>
public class MedicalRecord
{
    public int Id { get; set; }
    public int PatientId { get; set; }
    public string DoctorName { get; set; } = string.Empty;
    public DateTime VisitDate { get; set; }

    // Clinical data without encryption
    public string ChiefComplaint { get; set; } = string.Empty;
    public string Symptoms { get; set; } = string.Empty;
    public string Diagnosis { get; set; } = string.Empty;
    public string Prescription { get; set; } = string.Empty;
    public string LabResults { get; set; } = string.Empty;

    // VIOLATION: No fields for audit trail
    // Missing: CreatedAt, CreatedBy, ModifiedAt, ModifiedBy, AccessLog

    // VIOLATION: No patient consent tracking
    // Missing: ConsentGiven, ConsentDate, ConsentType, DataProcessingBasis
}
