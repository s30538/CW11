using CW11.DTOs;
using CW11.Services;
using Microsoft.AspNetCore.Mvc;

namespace CW11.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PrescriptionController : ControllerBase
    {
        private readonly IPrescriptionService _prescriptionService;

        public PrescriptionController(IPrescriptionService prescriptionService)
        {
            _prescriptionService = prescriptionService;
        }

        [HttpPost]
        public async Task<IActionResult> AddPrescription([FromBody] AddPrescriptionRequestDto request)
        {
            try
            {
                await _prescriptionService.AddPrescriptionAsync(request);
                return Ok("Prescription created successfully.");
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("patient/{idPatient}")]
        public async Task<IActionResult> GetPatient(int idPatient)
        {
            var result = await _prescriptionService.GetPatientDataAsync(idPatient);
            if (result == null)
                return NotFound("Patient not found.");

            return Ok(result);
        }
    }
}
