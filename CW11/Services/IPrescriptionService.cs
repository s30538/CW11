using CW11.DTOs;

namespace CW11.Services;

public interface IPrescriptionService
{
    Task AddPrescriptionAsync(AddPrescriptionRequestDto request);
    Task<GetPatientDto> GetPatientDataAsync(int idPatient);
    
}