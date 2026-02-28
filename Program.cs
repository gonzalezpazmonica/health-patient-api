using HealthPatientApi.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services
builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddSingleton<PatientService>();
builder.Services.AddSingleton<MedicalRecordService>();

// VIOLATION: No authentication/authorization configured
// Missing: builder.Services.AddAuthentication()
// Missing: builder.Services.AddAuthorization()
// Missing: CORS policy for healthcare applications

// VIOLATION: No HTTPS enforcement configuration
// VIOLATION: No data protection services configured
// Missing: builder.Services.AddDataProtection()

// VIOLATION: No logging configuration for audit trails
// Missing: Structured logging with PHI access tracking

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

// VIOLATION: No security headers (HSTS, CSP, X-Content-Type)
// VIOLATION: No rate limiting middleware
// VIOLATION: No request logging/audit middleware

app.UseHttpsRedirection();

// VIOLATION: Missing app.UseAuthentication()
// VIOLATION: Missing app.UseAuthorization()

app.MapControllers();

app.Run();
