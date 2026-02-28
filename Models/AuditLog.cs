namespace HealthPatientApi.Models;

/// <summary>
/// Audit log entry — tracks all PHI access and modifications.
/// Implements HIPAA §164.312(a)(2)(b) audit controls requirement.
/// </summary>
public class AuditLog
{
    /// <summary>
    /// Unique audit log identifier.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// User who performed the action (UserId from JWT claim or Anonymous).
    /// </summary>
    public string UserId { get; set; } = string.Empty;

    /// <summary>
    /// Action performed: READ, CREATE, UPDATE, DELETE, SEARCH, EXPORT, LOGIN, LOGOUT.
    /// </summary>
    public string Action { get; set; } = string.Empty;

    /// <summary>
    /// Entity type accessed: Patient, MedicalRecord, Prescription, etc.
    /// </summary>
    public string EntityType { get; set; } = string.Empty;

    /// <summary>
    /// Entity ID accessed (e.g., PatientId).
    /// </summary>
    public int? EntityId { get; set; }

    /// <summary>
    /// HTTP method used: GET, POST, PUT, DELETE, etc.
    /// </summary>
    public string HttpMethod { get; set; } = string.Empty;

    /// <summary>
    /// Endpoint path accessed: /api/Patients/{id}, /api/MedicalRecords, etc.
    /// </summary>
    public string Endpoint { get; set; } = string.Empty;

    /// <summary>
    /// Client IP address of the requester.
    /// </summary>
    public string IpAddress { get; set; } = string.Empty;

    /// <summary>
    /// HTTP status code returned: 200, 401, 403, 404, 500, etc.
    /// </summary>
    public int StatusCode { get; set; }

    /// <summary>
    /// Timestamp of the audit event (UTC).
    /// </summary>
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Previous value before modification (JSON serialized).
    /// Captured during UPDATE operations for data integrity tracking.
    /// </summary>
    public string? OldValues { get; set; }

    /// <summary>
    /// New value after modification (JSON serialized).
    /// Captured during CREATE/UPDATE operations.
    /// </summary>
    public string? NewValues { get; set; }

    /// <summary>
    /// Additional details or error messages (e.g., "Unauthorized access attempt").
    /// </summary>
    public string? Details { get; set; }

    /// <summary>
    /// Indicates if the request succeeded (true) or failed (false).
    /// </summary>
    public bool IsSuccess { get; set; } = true;
}
