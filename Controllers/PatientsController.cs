using Microsoft.AspNetCore.Mvc;
using DentalClinicAPI.Models;        // Adicione esta linha
using DentalClinicAPI.Repositories;  // Adicione esta linha
using System.Threading.Tasks;

[ApiController]
[Route("api/[controller]")]
public class PatientsController : ControllerBase
{
    private readonly IRepository<Patient> _patientRepo;

    public PatientsController(IRepository<Patient> patientRepo)
    {
        _patientRepo = patientRepo;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll() => Ok(await _patientRepo.GetAll());

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id) => Ok(await _patientRepo.GetById(id));

    [HttpPost]
    public async Task<IActionResult> Create(Patient patient)
    {
        await _patientRepo.Create(patient);
        return CreatedAtAction(nameof(GetById), new { id = patient.Id }, patient);
    }

    // Implemente PUT e DELETE similarmente
}