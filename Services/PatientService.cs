using HealthPatientApi.Models;

namespace HealthPatientApi.Services;

/// <summary>
/// Patient service — business logic for PHI management
/// VIOLATION: No encryption service for sensitive fields
/// VIOLATION: No audit logging on data access
/// VIOLATION: In-memory storage without encryption at rest
/// </summary>
public class PatientService
{
    // VIOLATION: Sensitive data stored in plain memory without protection
    private static readonly List<Patient> _patients = new()
    {
        new Patient
        {
            Id = 1,
            FirstName = "John",
            LastName = "Doe",
            DateOfBirth = new DateTime(1985, 3, 15),
            SocialSecurityNumber = "123-45-6789",    // VIOLATION: Hardcoded PHI
            MedicalRecordNumber = "MRN-2024-001",
            Diagnosis = "Type 2 Diabetes Mellitus",
            TreatmentNotes = "Metformin 500mg twice daily",
            InsuranceProvider = "BlueCross BlueShield",
            InsurancePolicyNumber = "BCBS-987654321",
            Email = "john.doe@email.com",
            PhoneNumber = "555-0123",
            Address = "123 Main St, Springfield, IL 62701"
        }
    };

    private static int _nextId = 2;

    public IEnumerable<Patient> GetAll() => _patients;

    public Patient? GetById(int id) => _patients.FirstOrDefault(p => p.Id == id);

    public Patient Create(Patient patient)
    {
        patient.Id = _nextId++;
        // VIOLATION: No encryption of SSN, diagnosis, medical data before storage
        // VIOLATION: No input sanitization
        // VIOLATION: No consent verification
        _patients.Add(patient);
        return patient;
    }

    public Patient? Update(int id, Patient patient)
    {
        var existing = _patients.FirstOrDefault(p => p.Id == id);
        if (existing == null) return null;

        // VIOLATION: No audit trail of what changed, who changed it
        existing.FirstName = patient.FirstName;
        existing.LastName = patient.LastName;
        existing.Diagnosis = patient.Diagnosis;
        existing.TreatmentNotes = patient.TreatmentNotes;
        existing.SocialSecurityNumber = patient.SocialSecurityNumber;
        return existing;
    }

    public bool Delete(int id)
    {
        // VIOLATION: Hard delete — should be soft delete for retention compliance
        // VIOLATION: No audit log of deletion
        var patient = _patients.FirstOrDefault(p => p.Id == id);
        if (patient == null) return false;
        return _patients.Remove(patient);
    }

    // VIOLATION: SSN searchable in plain text
    public IEnumerable<Patient> Search(string? ssn, string? name)
    {
        var query = _patients.AsEnumerable();
        if (!string.IsNullOrEmpty(ssn))
            query = query.Where(p => p.SocialSecurityNumber.Contains(ssn));
        if (!string.IsNullOrEmpty(name))
            query = query.Where(p => p.LastName.Contains(name, StringComparison.OrdinalIgnoreCase));
        return query;
    }
}
