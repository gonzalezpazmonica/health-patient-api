using HealthPatientApi.Models;

namespace HealthPatientApi.Services;

/// <summary>
/// Audit service for logging all PHI access and modifications.
/// Implements HIPAA §164.312(a)(2)(b) audit control requirements.
/// All PHI operations must be logged with user identification, timestamp, and IP address.
/// </summary>
public interface IAuditService
{
    /// <summary>
    /// Logs an audit event to the audit trail.
    /// </summary>
    void LogAudit(AuditLog auditLog);

    /// <summary>
    /// Retrieves audit logs for a specific entity (e.g., Patient ID).
    /// </summary>
    IEnumerable<AuditLog> GetAuditsByEntityId(int entityId);

    /// <summary>
    /// Retrieves audit logs for a specific user.
    /// </summary>
    IEnumerable<AuditLog> GetAuditsByUserId(string userId);

    /// <summary>
    /// Retrieves audit logs within a date range.
    /// </summary>
    IEnumerable<AuditLog> GetAuditsByDateRange(DateTime startDate, DateTime endDate);

    /// <summary>
    /// Retrieves all failed audit attempts (authentication/authorization failures).
    /// </summary>
    IEnumerable<AuditLog> GetFailedAttempts();
}

public class AuditService : IAuditService
{
    /// <summary>
    /// In-memory audit log storage.
    /// In production, this should be persisted to a tamper-proof audit database.
    /// </summary>
    private static readonly List<AuditLog> _auditLogs = new();
    private static int _nextId = 1;
    private readonly ILogger<AuditService> _logger;

    public AuditService(ILogger<AuditService> logger)
    {
        _logger = logger;
    }

    public void LogAudit(AuditLog auditLog)
    {
        if (auditLog == null)
            return;

        auditLog.Id = _nextId++;
        auditLog.Timestamp = DateTime.UtcNow;
        _auditLogs.Add(auditLog);

        // Also log to structured logging for operational visibility
        var logMessage = $"AUDIT: [{auditLog.Action}] User={auditLog.UserId} Entity={auditLog.EntityType}/{auditLog.EntityId} " +
                        $"Endpoint={auditLog.Endpoint} Status={auditLog.StatusCode} IP={auditLog.IpAddress}";

        if (auditLog.IsSuccess)
            _logger.LogInformation(logMessage);
        else
            _logger.LogWarning(logMessage + " - FAILED");
    }

    public IEnumerable<AuditLog> GetAuditsByEntityId(int entityId)
    {
        return _auditLogs.Where(a => a.EntityId == entityId).OrderByDescending(a => a.Timestamp);
    }

    public IEnumerable<AuditLog> GetAuditsByUserId(string userId)
    {
        return _auditLogs.Where(a => a.UserId == userId).OrderByDescending(a => a.Timestamp);
    }

    public IEnumerable<AuditLog> GetAuditsByDateRange(DateTime startDate, DateTime endDate)
    {
        return _auditLogs
            .Where(a => a.Timestamp >= startDate && a.Timestamp <= endDate)
            .OrderByDescending(a => a.Timestamp);
    }

    public IEnumerable<AuditLog> GetFailedAttempts()
    {
        return _auditLogs.Where(a => !a.IsSuccess).OrderByDescending(a => a.Timestamp);
    }
}
