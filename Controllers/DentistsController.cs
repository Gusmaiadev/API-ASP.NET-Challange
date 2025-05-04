using Microsoft.AspNetCore.Mvc;
using DentalClinicAPI.DTOs;
using DentalClinicAPI.Models;
using DentalClinicAPI.Repositories;
using AutoMapper;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;

namespace DentalClinicAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] // Protege todos os métodos, requerendo apenas autenticação
    public class DentistsController : ControllerBase
    {
        private readonly IRepository<Dentist> _dentistRepo;
        private readonly IMapper _mapper;

        public DentistsController(IRepository<Dentist> dentistRepo, IMapper mapper)
        {
            _dentistRepo = dentistRepo;
            _mapper = mapper;
        }

        /// <summary>
        /// Obtém todos os dentistas
        /// </summary>
        /// <returns>Lista de dentistas</returns>
        [HttpGet]
        [AllowAnonymous] // Permite acesso sem autenticação
        public async Task<IActionResult> GetAll()
        {
            var dentists = await _dentistRepo.GetAll();
            return Ok(_mapper.Map<IEnumerable<DentistReadDTO>>(dentists));
        }

        /// <summary>
        /// Obtém dentista por ID
        /// </summary>
        /// <param name="id">ID do dentista</param>
        /// <returns>Dados do dentista</returns>
        [HttpGet("{id}")]
        [AllowAnonymous] // Permite acesso sem autenticação
        public async Task<IActionResult> GetById(int id)
        {
            var dentist = await _dentistRepo.GetById(id);
            if (dentist == null) return NotFound();

            return Ok(_mapper.Map<DentistReadDTO>(dentist));
        }

        /// <summary>
        /// Cria um novo dentista
        /// </summary>
        /// <param name="dto">Dados do dentista (excluindo ID)</param>
        /// <returns>Dentista criado</returns>
        [HttpPost]
        [Authorize] // Qualquer usuário autenticado pode criar
        public async Task<IActionResult> Create(DentistCreateDTO dto)
        {
            var dentist = _mapper.Map<Dentist>(dto);
            await _dentistRepo.Create(dentist);

            return CreatedAtAction(
                nameof(GetById),
                new { id = dentist.Id },
                _mapper.Map<DentistReadDTO>(dentist)
            );
        }

        /// <summary>
        /// Atualiza um dentista existente
        /// </summary>
        /// <param name="id">ID do dentista</param>
        /// <param name="dto">Novos dados do dentista</param>
        /// <returns>Status 204 No Content</returns>
        [HttpPut("{id}")]
        [Authorize] // Qualquer usuário autenticado pode atualizar
        public async Task<IActionResult> Update(int id, DentistCreateDTO dto)
        {
            var existingDentist = await _dentistRepo.GetById(id);
            if (existingDentist == null) return NotFound();

            // Atualiza apenas os campos permitidos
            _mapper.Map(dto, existingDentist);
            await _dentistRepo.Update(existingDentist);

            return NoContent();
        }

        /// <summary>
        /// Exclui um dentista
        /// </summary>
        /// <param name="id">ID do dentista</param>
        /// <returns>Status 204 No Content</returns>
        [HttpDelete("{id}")]
        [Authorize] // Qualquer usuário autenticado pode excluir
        public async Task<IActionResult> Delete(int id)
        {
            await _dentistRepo.Delete(id);
            return NoContent();
        }
    }
}