using Microsoft.AspNetCore.Mvc;
using HealthPatientApi.Models;
using HealthPatientApi.Services;

namespace HealthPatientApi.Controllers;

/// <summary>
/// Medical Records API
/// VIOLATION: No authentication required
/// VIOLATION: No audit logging for record access
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class MedicalRecordsController : ControllerBase
{
    private readonly MedicalRecordService _recordService;

    public MedicalRecordsController(MedicalRecordService recordService)
    {
        _recordService = recordService;
    }

    [HttpGet("patient/{patientId}")]
    public ActionResult<IEnumerable<MedicalRecord>> GetByPatient(int patientId)
    {
        // VIOLATION: No check if requesting user has access to this patient's records
        return Ok(_recordService.GetByPatientId(patientId));
    }

    [HttpPost]
    public ActionResult<MedicalRecord> Create([FromBody] MedicalRecord record)
    {
        // VIOLATION: No consent verification
        // VIOLATION: No data validation against HL7/FHIR schemas
        var created = _recordService.Create(record);
        return CreatedAtAction(nameof(GetByPatient),
            new { patientId = created.PatientId }, created);
    }

    // VIOLATION: Full clinical data returned in response without encryption
    [HttpGet("{id}")]
    public ActionResult<MedicalRecord> GetById(int id)
    {
        var record = _recordService.GetById(id);
        if (record == null) return NotFound();
        return Ok(record);
    }
}
