namespace HealthPatientApi.Models;

/// <summary>
/// Prescription data — drug and dosage information
/// VIOLATION: No HL7/FHIR standard compliance
/// VIOLATION: Drug codes not using standard terminology (RxNorm, SNOMED CT)
/// </summary>
public class Prescription
{
    public int Id { get; set; }
    public int PatientId { get; set; }
    public int MedicalRecordId { get; set; }

    // VIOLATION: Free text instead of coded terminology (RxNorm)
    public string DrugName { get; set; } = string.Empty;
    public string Dosage { get; set; } = string.Empty;
    public string Frequency { get; set; } = string.Empty;

    public DateTime PrescribedDate { get; set; }
    public DateTime? ExpirationDate { get; set; }
    public string PrescribedBy { get; set; } = string.Empty;

    // VIOLATION: No pharmacy verification tracking
    // VIOLATION: No adverse reaction cross-reference
}
