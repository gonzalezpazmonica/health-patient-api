namespace HealthPatientApi.Models;

/// <summary>
/// Application user model for authentication and role-based access control.
/// Implements HIPAA §164.312(a)(1) access control requirements.
/// </summary>
public class AppUser
{
    /// <summary>
    /// Unique user identifier.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Username for login.
    /// </summary>
    public string Username { get; set; } = string.Empty;

    /// <summary>
    /// User's email address.
    /// </summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Hashed password (not stored in plaintext).
    /// </summary>
    public string PasswordHash { get; set; } = string.Empty;

    /// <summary>
    /// User's full name.
    /// </summary>
    public string FullName { get; set; } = string.Empty;

    /// <summary>
    /// User roles: Admin, Clinician, Researcher, Patient.
    /// Determines access level to PHI and system functions.
    /// </summary>
    public List<string> Roles { get; set; } = new();

    /// <summary>
    /// Indicates if user account is active.
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Timestamp when user account was created.
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Timestamp of last login.
    /// </summary>
    public DateTime? LastLoginAt { get; set; }

    /// <summary>
    /// Timestamp when password was last changed.
    /// For password rotation compliance.
    /// </summary>
    public DateTime? LastPasswordChangeAt { get; set; }

    /// <summary>
    /// If user is a Patient, stores the associated PatientId.
    /// </summary>
    public int? PatientId { get; set; }
}
