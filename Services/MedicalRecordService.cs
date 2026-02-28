using HealthPatientApi.Models;

namespace HealthPatientApi.Services;

/// <summary>
/// Medical record service
/// VIOLATION: No encryption for clinical data
/// VIOLATION: No access control checks
/// VIOLATION: No audit logging
/// </summary>
public class MedicalRecordService
{
    private static readonly List<MedicalRecord> _records = new()
    {
        new MedicalRecord
        {
            Id = 1,
            PatientId = 1,
            DoctorName = "Dr. Smith",
            VisitDate = DateTime.Now.AddDays(-30),
            ChiefComplaint = "Routine checkup",
            Symptoms = "Increased thirst, frequent urination",
            Diagnosis = "Type 2 Diabetes — A1C 7.2%",
            Prescription = "Metformin 500mg BID",
            LabResults = "Glucose: 180mg/dL, A1C: 7.2%, Creatinine: 1.1"
        }
    };

    private static int _nextId = 2;

    public IEnumerable<MedicalRecord> GetByPatientId(int patientId) =>
        _records.Where(r => r.PatientId == patientId);

    public MedicalRecord? GetById(int id) =>
        _records.FirstOrDefault(r => r.Id == id);

    public MedicalRecord Create(MedicalRecord record)
    {
        record.Id = _nextId++;
        // VIOLATION: No data validation against medical coding standards
        // VIOLATION: No consent check before storing clinical data
        _records.Add(record);
        return record;
    }
}
