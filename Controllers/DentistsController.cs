using Microsoft.AspNetCore.Mvc;
using DentalClinicAPI.Models;
using DentalClinicAPI.Repositories;
using System.Threading.Tasks;

[ApiController]
[Route("api/[controller]")]
public class DentistsController : ControllerBase
{
    private readonly IRepository<Dentist> _dentistRepo;

    public DentistsController(IRepository<Dentist> dentistRepo)
    {
        _dentistRepo = dentistRepo;
    }

    // GET: api/dentists
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var dentists = await _dentistRepo.GetAll();
        return Ok(dentists);
    }

    // GET: api/dentists/5
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var dentist = await _dentistRepo.GetById(id);
        if (dentist == null) return NotFound();
        return Ok(dentist);
    }

    // POST: api/dentists
    [HttpPost]
    public async Task<IActionResult> Create(Dentist dentist)
    {
        await _dentistRepo.Create(dentist);
        return CreatedAtAction(nameof(GetById), new { id = dentist.Id }, dentist);
    }

    // PUT: api/dentists/5
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, Dentist dentist)
    {
        if (id != dentist.Id) return BadRequest();
        await _dentistRepo.Update(dentist);
        return NoContent();
    }

    // DELETE: api/dentists/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _dentistRepo.Delete(id);
        return NoContent();
    }
}