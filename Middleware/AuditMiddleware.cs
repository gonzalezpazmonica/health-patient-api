using HealthPatientApi.Models;
using HealthPatientApi.Services;
using System.Security.Claims;

namespace HealthPatientApi.Middleware;

/// <summary>
/// Middleware for auditing all API requests and responses.
/// Logs every PHI access with user identification, timestamp, IP address, and result.
/// Implements HIPAA §164.312(a)(2)(b) audit control requirements.
/// </summary>
public class AuditMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<AuditMiddleware> _logger;

    public AuditMiddleware(RequestDelegate next, ILogger<AuditMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context, IAuditService auditService)
    {
        var startTime = DateTime.UtcNow;
        var request = context.Request;

        // Extract user information from JWT claims or use "Anonymous"
        var userId = context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "Anonymous";
        var userRole = context.User?.FindFirst(ClaimTypes.Role)?.Value ?? "Unknown";

        // Get client IP address
        var ipAddress = context.Connection.RemoteIpAddress?.ToString() ?? "Unknown";

        // Store original response stream to capture status code
        var originalBodyStream = context.Response.Body;

        try
        {
            using (var responseBody = new MemoryStream())
            {
                context.Response.Body = responseBody;

                // Call next middleware in the pipeline
                await _next(context);

                var statusCode = context.Response.StatusCode;
                var isSuccess = statusCode >= 200 && statusCode < 300;

                // Parse entity ID from route if available
                int? entityId = null;
                if (context.GetRouteData().Values.TryGetValue("id", out var idValue) && int.TryParse(idValue?.ToString(), out var id))
                {
                    entityId = id;
                }

                // Determine entity type from endpoint
                var entityType = DetermineEntityType(request.Path);

                // Create audit log entry
                var auditLog = new AuditLog
                {
                    UserId = userId,
                    Action = DetermineAction(request.Method),
                    EntityType = entityType,
                    EntityId = entityId,
                    HttpMethod = request.Method,
                    Endpoint = request.Path.Value ?? string.Empty,
                    IpAddress = ipAddress,
                    StatusCode = statusCode,
                    Timestamp = startTime,
                    IsSuccess = isSuccess,
                    Details = $"Role: {userRole}"
                };

                // Log to audit service
                auditService.LogAudit(auditLog);

                // Log specific security events
                if (!isSuccess && statusCode == 401)
                {
                    _logger.LogWarning("SECURITY: Unauthorized access attempt from {IpAddress} to {Endpoint}", ipAddress, request.Path);
                }
                else if (!isSuccess && statusCode == 403)
                {
                    _logger.LogWarning("SECURITY: Forbidden access attempt by {UserId} from {IpAddress} to {Endpoint}", userId, ipAddress, request.Path);
                }

                // Copy the response body to the original stream
                await responseBody.CopyToAsync(originalBodyStream);
            }
        }
        finally
        {
            context.Response.Body = originalBodyStream;
        }
    }

    private static string DetermineAction(string httpMethod)
    {
        return httpMethod.ToUpperInvariant() switch
        {
            "GET" => "READ",
            "POST" => "CREATE",
            "PUT" => "UPDATE",
            "DELETE" => "DELETE",
            "PATCH" => "UPDATE",
            _ => "UNKNOWN"
        };
    }

    private static string DetermineEntityType(PathString path)
    {
        var pathValue = path.Value?.ToLowerInvariant() ?? string.Empty;

        if (pathValue.Contains("/patients"))
            return "Patient";
        if (pathValue.Contains("/medicalrecords"))
            return "MedicalRecord";
        if (pathValue.Contains("/prescriptions"))
            return "Prescription";
        if (pathValue.Contains("/consent"))
            return "PatientConsent";
        if (pathValue.Contains("/audit"))
            return "AuditLog";

        return "Unknown";
    }
}
