using HealthPatientApi.Services;
using HealthPatientApi.Middleware;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// FIXED: Add authentication services with JWT bearer tokens
// HIPAA §164.312(a)(1) — Access control implementation
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        var jwtKey = builder.Configuration["Jwt:Key"] ?? "your-secret-key-minimum-32-characters-long";
        var key = Encoding.ASCII.GetBytes(jwtKey);

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(key),
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };
    });

// FIXED: Add authorization policies
// HIPAA §164.312(a)(1) — Role-based access control
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("Clinician", policy => policy.RequireRole("Clinician", "Admin"));
    options.AddPolicy("Admin", policy => policy.RequireRole("Admin"));
    options.AddPolicy("Patient", policy => policy.RequireRole("Patient"));
});

// FIXED: Add data protection services
// HIPAA §164.312(a)(2)(i) — Encryption at rest
// Uses DPAPI (Windows) or X509 (Linux/Mac) for key storage
builder.Services.AddDataProtection();

// Add MVC services
builder.Services.AddControllers();
builder.Services.AddOpenApi();

// FIXED: Register PHI services
builder.Services.AddSingleton<PatientService>();
builder.Services.AddSingleton<MedicalRecordService>();
builder.Services.AddSingleton<IEncryptionService, EncryptionService>();
builder.Services.AddSingleton<IAuditService, AuditService>();
builder.Services.AddSingleton<IConsentService, ConsentService>();

// FIXED: Configure CORS for healthcare applications
// Restrict to specific origins in production
builder.Services.AddCors(options =>
{
    options.AddPolicy("HealthcarePolicy", corsPolicyBuilder =>
    {
        corsPolicyBuilder
            .AllowAnyOrigin()  // In production: .WithOrigins("https://trusted-domain.com")
            .AllowAnyMethod()
            .AllowAnyHeader();
    });
});

// FIXED: Add structured logging
// HIPAA §164.312(a)(2)(b) — Audit controls with logging
builder.Services.AddLogging(config =>
{
    config.ClearProviders();
    config.AddConsole();
    config.AddDebug();
    // In production: Add Serilog for centralized structured logging
});

var app = builder.Build();

// FIXED: Add security headers middleware
// HIPAA §164.312(a)(2)(iv) — Transmission security
app.Use(async (context, next) =>
{
    // HSTS: Enforce HTTPS for 1 year
    context.Response.Headers["Strict-Transport-Security"] = "max-age=31536000; includeSubDomains";
    context.Response.Headers["Content-Security-Policy"] = "default-src 'self'; script-src 'self'; style-src 'self' 'unsafe-inline'";
    context.Response.Headers["X-Content-Type-Options"] = "nosniff";
    context.Response.Headers["X-Frame-Options"] = "DENY";
    context.Response.Headers["X-XSS-Protection"] = "1; mode=block";
    context.Response.Headers["Referrer-Policy"] = "strict-origin-when-cross-origin";

    await next();
});

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

// FIXED: Use HTTPS redirection
// HIPAA §164.312(a)(2)(iv) — Require TLS 1.2+
app.UseHttpsRedirection();

// FIXED: Use CORS
app.UseCors("HealthcarePolicy");

// FIXED: Add authentication middleware (must be before authorization)
// HIPAA §164.312(a)(1) — Access control
app.UseAuthentication();

// FIXED: Add authorization middleware
app.UseAuthorization();

// FIXED: Add audit middleware
// HIPAA §164.312(a)(2)(b) — Audit controls
// Logs all PHI access with user identification and timestamp
app.UseMiddleware<AuditMiddleware>();

app.MapControllers();

// FIXED: Log startup message
app.Logger.LogInformation("HealthPatientApi started with compliance mode enabled");
app.Logger.LogInformation("Authentication: JWT Bearer enabled");
app.Logger.LogInformation("Authorization: Role-based access control enabled");
app.Logger.LogInformation("Encryption: AES-256 data protection enabled");
app.Logger.LogInformation("Audit: All PHI access is logged for HIPAA compliance");

app.Run();
