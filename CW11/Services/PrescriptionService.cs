using CW11.Data;
using CW11.DTOs;
using CW11.Models;

namespace CW11.Services;

public class PrescriptionService : IPrescriptionService
{
        private readonly DatabaseContext _context;

        public PrescriptionService(DatabaseContext context)
        {
            _context = context;
        }

        public async Task AddPrescriptionAsync(AddPrescriptionRequestDto request)
        {
            if (request.Medicaments.Count > 10)
                throw new ArgumentException("Prescription cannot have more than 10 medicaments.");

            if (request.DueDate < request.Date)
                throw new ArgumentException("DueDate cannot be earlier than Date.");

            var doctor = await _context.Doctors.FindAsync(request.IdDoctor);
            if (doctor == null)
                throw new ArgumentException("Doctor does not exist.");

            var patient = request.Patient.IdPatient.HasValue
                ? await _context.Patients.FindAsync(request.Patient.IdPatient.Value)
                : null;

            if (patient == null)
            {
                patient = new Patient
                {
                    FirstName = request.Patient.FirstName,
                    LastName = request.Patient.LastName,
                    BirthDate = request.Patient.Birthday
                };
                _context.Patients.Add(patient);
                await _context.SaveChangesAsync();
            }

            var prescription = new Prescription
            {
                Date = request.Date,
                DueDate = request.DueDate,
                IdDoctor = request.IdDoctor,
                IdPatient = patient.IdPatient,
                Prescription_Medicaments = new List<Prescription_Medicament>()
            };

            foreach (var medDto in request.Medicaments)
            {
                var medicament = await _context.Medicaments.FindAsync(medDto.IdMedicament);
                if (medicament == null)
                    throw new ArgumentException();

                prescription.Prescription_Medicaments.Add(new Prescription_Medicament()
                {
                    IdMedicament = medDto.IdMedicament,
                    Dose = medDto.Dose,
                    Details = medDto.Details
                });
            }

            _context.Prescriptions.Add(prescription);
            await _context.SaveChangesAsync();
        }

        public async Task<GetPatientDto?> GetPatientDataAsync(int idPatient)
        {
            var patient = await _context.Patients
                .Include(p => p.Prescriptions)
                    .ThenInclude(pr => pr.PrescriptionMedicaments)
                        .ThenInclude(pm => pm.Medicament)
                .Include(p => p.Prescriptions)
                    .ThenInclude(pr => pr.Doctor)
                .FirstOrDefaultAsync(p => p.IdPatient == idPatient);

            if (patient == null)
                return null;

            return new GetPatientDto
            {
                IdPatient = patient.IdPatient,
                FirstName = patient.FirstName,
                LastName = patient.LastName,
                Birthday = patient.Birthdate,
                Prescriptions = patient.Prescriptions
                    .OrderBy(p => p.DueDate)
                    .Select(p => new PrescriptionDto
                    {
                        IdPrescription = p.IdPrescription,
                        Date = p.Date,
                        DueDate = p.DueDate,
                        Doctor = new DoctorDto
                        {
                            IdDoctor = p.Doctor.IdDoctor,
                            FirstName = p.Doctor.FirstName
                        },
                        Medicaments = p.PrescriptionMedicaments.Select(pm => new GetMedicamentDto
                        {
                            IdMedicament = pm.Medicament.IdMedicament,
                            Name = pm.Medicament.Name,
                            Dose = pm.Dose,
                            Description = pm.Medicament.Description
                        }).ToList()
                    }).ToList() 
            };
    }
}