using Microsoft.AspNetCore.Mvc;
using DentalClinicAPI.Models;          // Adicione esta linha
using DentalClinicAPI.Repositories;    // Adicione esta linha
using DentalClinicAPI.Services;        // Adicione esta linha
using System.Threading.Tasks;

[ApiController]
[Route("api/[controller]")]
public class AppointmentsController : ControllerBase
{
    private readonly IRepository<Appointment> _appointmentRepo; // Corrigido para Appointment
    private readonly IClinicService _clinicService;

    public AppointmentsController(
        IRepository<Appointment> appointmentRepo, // Corrigido para Appointment
        IClinicService clinicService)
    {
        _appointmentRepo = appointmentRepo;
        _clinicService = clinicService;
    }

    // GET: api/appointments
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var appointments = await _appointmentRepo.GetAll();
        return Ok(appointments);
    }

    // GET: api/appointments/5
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var appointment = await _appointmentRepo.GetById(id);
        if (appointment == null) return NotFound();
        return Ok(appointment);
    }

    // POST: api/appointments
    [HttpPost]
    public async Task<IActionResult> Create(Appointment appointment) // Corrigido para Appointment
    {
        await _appointmentRepo.Create(appointment);
        return CreatedAtAction(nameof(GetById), new { id = appointment.Id }, appointment);
    }

    // PUT: api/appointments/5
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, Appointment appointment) // Corrigido para Appointment
    {
        if (id != appointment.Id) return BadRequest();
        await _appointmentRepo.Update(appointment);
        return NoContent();
    }

    // DELETE: api/appointments/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _appointmentRepo.Delete(id);
        return NoContent();
    }

    // GET: api/appointments/by-date/2024-01-01
    [HttpGet("by-date/{date}")]
    public async Task<IActionResult> GetByDate([FromRoute] DateTime date)
    {
        var appointments = await _clinicService.GetAppointmentsByDate(date);
        return Ok(appointments);
    }
}