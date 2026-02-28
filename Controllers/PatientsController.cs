using Microsoft.AspNetCore.Mvc;
using HealthPatientApi.Models;
using HealthPatientApi.Services;

namespace HealthPatientApi.Controllers;

/// <summary>
/// Patient API — manages PHI data
/// VIOLATION: No authentication/authorization middleware
/// VIOLATION: No role-based access control (RBAC)
/// VIOLATION: Returns full PHI in responses without masking
/// VIOLATION: No access logging/audit trail
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class PatientsController : ControllerBase
{
    private readonly PatientService _patientService;

    public PatientsController(PatientService patientService)
    {
        _patientService = patientService;
    }

    // VIOLATION: No [Authorize] attribute — unauthenticated access to PHI
    // VIOLATION: Returns full SSN and medical data
    [HttpGet]
    public ActionResult<IEnumerable<Patient>> GetAll()
    {
        return Ok(_patientService.GetAll());
    }

    // VIOLATION: No access logging when viewing individual patient PHI
    [HttpGet("{id}")]
    public ActionResult<Patient> GetById(int id)
    {
        var patient = _patientService.GetById(id);
        if (patient == null) return NotFound();
        return Ok(patient);
    }

    // VIOLATION: No input validation on PHI fields
    // VIOLATION: No consent verification before storing data
    [HttpPost]
    public ActionResult<Patient> Create([FromBody] Patient patient)
    {
        var created = _patientService.Create(patient);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    // VIOLATION: No audit trail for PHI modifications
    [HttpPut("{id}")]
    public ActionResult<Patient> Update(int id, [FromBody] Patient patient)
    {
        var updated = _patientService.Update(id, patient);
        if (updated == null) return NotFound();
        return Ok(updated);
    }

    // VIOLATION: Hard delete instead of soft delete — violates retention policies
    // VIOLATION: No verification of "right to erasure" request legitimacy
    [HttpDelete("{id}")]
    public ActionResult Delete(int id)
    {
        var deleted = _patientService.Delete(id);
        if (!deleted) return NotFound();
        return NoContent();
    }

    // VIOLATION: Bulk export of PHI without authorization check
    // VIOLATION: No rate limiting on data export
    [HttpGet("export")]
    public ActionResult<IEnumerable<Patient>> ExportAll()
    {
        return Ok(_patientService.GetAll());
    }

    // VIOLATION: Search by SSN over plain HTTP possible
    [HttpGet("search")]
    public ActionResult<IEnumerable<Patient>> Search([FromQuery] string? ssn, [FromQuery] string? name)
    {
        return Ok(_patientService.Search(ssn, name));
    }
}
