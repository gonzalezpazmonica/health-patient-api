using System.Security.Cryptography;
using System.Text;

namespace HealthPatientApi.Services;

/// <summary>
/// Encryption service for AES-256 encryption/decryption of PHI fields.
/// Implements HIPAA §164.312(a)(2)(i) technical safeguards.
/// </summary>
public interface IEncryptionService
{
    /// <summary>
    /// Encrypts plaintext using AES-256.
    /// </summary>
    string Encrypt(string plaintext);

    /// <summary>
    /// Decrypts ciphertext using AES-256.
    /// </summary>
    string Decrypt(string ciphertext);
}

public class EncryptionService : IEncryptionService
{
    private readonly byte[] _encryptionKey;
    private const int KeySize = 32; // 256 bits
    private const int IvSize = 16;  // 128 bits

    /// <summary>
    /// Initializes the encryption service with a key from configuration.
    /// Key must be 256 bits (32 bytes) for AES-256.
    /// In production, load from Azure Key Vault or AWS Secrets Manager.
    /// </summary>
    public EncryptionService(IConfiguration configuration)
    {
        var keyString = configuration["Encryption:Key"] ?? throw new InvalidOperationException(
            "Encryption key not configured. Set 'Encryption:Key' in appsettings.json or environment variables.");

        _encryptionKey = Convert.FromBase64String(keyString);

        if (_encryptionKey.Length != KeySize)
        {
            throw new InvalidOperationException(
                $"Encryption key must be {KeySize} bytes (256 bits). Provided key is {_encryptionKey.Length} bytes.");
        }
    }

    /// <summary>
    /// Encrypts plaintext using AES-256-GCM (authenticated encryption).
    /// Returns: IV + AuthTag + Ciphertext (Base64 encoded).
    /// </summary>
    public string Encrypt(string plaintext)
    {
        if (string.IsNullOrEmpty(plaintext))
            return plaintext;

        try
        {
            using (var aes = new AesGcm(_encryptionKey, 16))
            {
                var iv = new byte[IvSize];
                using (var rng = RandomNumberGenerator.Create())
                {
                    rng.GetBytes(iv);
                }

                var plaintextBytes = Encoding.UTF8.GetBytes(plaintext);
                var ciphertextBytes = new byte[plaintextBytes.Length];
                var tag = new byte[16]; // GCM tag is always 16 bytes

                aes.Encrypt(iv, plaintextBytes, Array.Empty<byte>(), ciphertextBytes, tag);

                // Return IV + Tag + Ciphertext as Base64 for storage
                var result = new byte[IvSize + tag.Length + ciphertextBytes.Length];
                Buffer.BlockCopy(iv, 0, result, 0, IvSize);
                Buffer.BlockCopy(tag, 0, result, IvSize, tag.Length);
                Buffer.BlockCopy(ciphertextBytes, 0, result, IvSize + tag.Length, ciphertextBytes.Length);

                return Convert.ToBase64String(result);
            }
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("Encryption failed.", ex);
        }
    }

    /// <summary>
    /// Decrypts Base64-encoded ciphertext using AES-256-GCM.
    /// Expects format: IV + AuthTag + Ciphertext.
    /// </summary>
    public string Decrypt(string ciphertext)
    {
        if (string.IsNullOrEmpty(ciphertext))
            return ciphertext;

        try
        {
            var encryptedBytes = Convert.FromBase64String(ciphertext);

            if (encryptedBytes.Length < IvSize + 16)
            {
                throw new InvalidOperationException("Invalid ciphertext format.");
            }

            var iv = new byte[IvSize];
            var tag = new byte[16];
            var ciphertextOnlyBytes = new byte[encryptedBytes.Length - IvSize - 16];

            Buffer.BlockCopy(encryptedBytes, 0, iv, 0, IvSize);
            Buffer.BlockCopy(encryptedBytes, IvSize, tag, 0, 16);
            Buffer.BlockCopy(encryptedBytes, IvSize + 16, ciphertextOnlyBytes, 0, ciphertextOnlyBytes.Length);

            using (var aes = new AesGcm(_encryptionKey, 16))
            {
                var plaintextBytes = new byte[ciphertextOnlyBytes.Length];
                aes.Decrypt(iv, ciphertextOnlyBytes, tag, plaintextBytes, Array.Empty<byte>());
                return Encoding.UTF8.GetString(plaintextBytes);
            }
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("Decryption failed.", ex);
        }
    }
}
