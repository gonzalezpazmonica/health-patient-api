namespace HealthPatientApi.Attributes;

/// <summary>
/// Custom attribute to mark fields that require encryption at rest.
/// Used for HIPAA §164.312(a)(2)(i) compliance.
/// Fields marked with this attribute must be encrypted using AES-256.
/// </summary>
[AttributeUsage(AttributeTargets.Property)]
public class EncryptedFieldAttribute : Attribute
{
    /// <summary>
    /// Gets or sets the encryption algorithm to use (default: AES-256).
    /// </summary>
    public string Algorithm { get; set; } = "AES-256";

    /// <summary>
    /// Gets or sets the field classification (PHI/PII/Sensitive).
    /// </summary>
    public string Classification { get; set; } = "PHI";

    /// <summary>
    /// Gets or sets the reason for encryption requirement.
    /// </summary>
    public string Reason { get; set; } = string.Empty;

    public EncryptedFieldAttribute(string reason = "")
    {
        Reason = reason;
    }
}
