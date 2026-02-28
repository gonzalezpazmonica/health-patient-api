# Health Patient API

API de gestión de pacientes y registros médicos construida con .NET 10.

## Propósito

Este proyecto es un **caso de prueba** para validar el sistema de
**Regulatory Compliance Intelligence** de pm-workspace v0.33.0.

Contiene violaciones intencionadas de regulaciones sanitarias (HIPAA, GDPR Art.9,
HL7/FHIR) para verificar que los comandos `/compliance-scan`, `/compliance-fix`
y `/compliance-report` detectan y reportan correctamente las incidencias.

## Estructura

```
HealthPatientApi/
├── Models/           # Entidades con PHI sin cifrar
│   ├── Patient.cs
│   ├── MedicalRecord.cs
│   └── Prescription.cs
├── Controllers/      # APIs sin autenticación ni auditoría
│   ├── PatientsController.cs
│   └── MedicalRecordsController.cs
├── Services/         # Lógica de negocio sin protección de datos
│   ├── PatientService.cs
│   └── MedicalRecordService.cs
└── Program.cs        # Sin middleware de seguridad
```

## Violaciones intencionadas

- **Cifrado**: PHI almacenada en texto plano (SSN, diagnósticos, resultados)
- **Auditoría**: Sin trail de acceso ni modificación de datos médicos
- **Control de acceso**: Sin autenticación ni RBAC en endpoints
- **Consentimiento**: Sin verificación de consentimiento del paciente
- **Interoperabilidad**: Sin cumplimiento HL7/FHIR ni terminología estándar
- **Trazabilidad**: Sin soft-delete ni políticas de retención

## Tech Stack

- .NET 10 (Preview)
- ASP.NET Core Web API
- En memoria (sin DB real)
