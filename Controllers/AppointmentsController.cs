using Microsoft.AspNetCore.Mvc;
using DentalClinicAPI.DTOs;
using DentalClinicAPI.Models;
using DentalClinicAPI.Repositories;
using DentalClinicAPI.Services;
using AutoMapper;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;

namespace DentalClinicAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] // Protege todos os métodos deste controller
    public class AppointmentsController : ControllerBase
    {
        private readonly IRepository<Appointment> _appointmentRepo;
        private readonly IClinicService _clinicService;
        private readonly IMapper _mapper;

        public AppointmentsController(
            IRepository<Appointment> appointmentRepo,
            IClinicService clinicService,
            IMapper mapper)
        {
            _appointmentRepo = appointmentRepo;
            _clinicService = clinicService;
            _mapper = mapper;
        }

        /// <summary>
        /// Obtém todos os agendamentos
        /// </summary>
        /// <returns>Lista de agendamentos</returns>
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var appointments = await _appointmentRepo.GetAll();
            return Ok(_mapper.Map<IEnumerable<AppointmentReadDTO>>(appointments));
        }

        /// <summary>
        /// Obtém agendamento por ID
        /// </summary>
        /// <param name="id">ID do agendamento</param>
        /// <returns>Detalhes do agendamento</returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var appointment = await _appointmentRepo.GetById(id);
            if (appointment == null) return NotFound();

            return Ok(_mapper.Map<AppointmentReadDTO>(appointment));
        }

        /// <summary>
        /// Cria um novo agendamento
        /// </summary>
        /// <param name="dto">Dados do agendamento</param>
        /// <returns>Agendamento criado</returns>
        [HttpPost]
        public async Task<IActionResult> Create(AppointmentCreateDTO dto)
        {
            // Validações de negócio
            if (!await _clinicService.IsDentistAvailable(dto.DentistId, dto.Date))
                return Conflict("Dentista não disponível nesta data/hora");

            if (!await _clinicService.PatientExists(dto.PatientId))
                return NotFound("Paciente não encontrado");

            var appointment = _mapper.Map<Appointment>(dto);
            await _appointmentRepo.Create(appointment);

            return CreatedAtAction(
                nameof(GetById),
                new { id = appointment.Id },
                _mapper.Map<AppointmentReadDTO>(appointment)
            );
        }

        /// <summary>
        /// Atualiza um agendamento existente
        /// </summary>
        /// <param name="id">ID do agendamento</param>
        /// <param name="dto">Novos dados do agendamento</param>
        /// <returns>Status 204 No Content</returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, AppointmentCreateDTO dto)
        {
            var existingAppointment = await _appointmentRepo.GetById(id);
            if (existingAppointment == null) return NotFound();

            // Verifica se o dentista está disponível para o novo horário
            if (existingAppointment.Date != dto.Date || existingAppointment.DentistId != dto.DentistId)
            {
                if (!await _clinicService.IsDentistAvailable(dto.DentistId, dto.Date))
                    return Conflict("Novo horário/dentista indisponível");
            }

            _mapper.Map(dto, existingAppointment);
            await _appointmentRepo.Update(existingAppointment);

            return NoContent();
        }

        /// <summary>
        /// Exclui um agendamento
        /// </summary>
        /// <param name="id">ID do agendamento</param>
        /// <returns>Status 204 No Content</returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _appointmentRepo.Delete(id);
            return NoContent();
        }

        /// <summary>
        /// Obtém agendamentos por data
        /// </summary>
        /// <param name="date">Data no formato yyyy-MM-dd</param>
        /// <returns>Lista de agendamentos</returns>
        [HttpGet("by-date/{date}")]
        public async Task<IActionResult> GetByDate(string date)
        {
            if (!DateTime.TryParse(date, out var parsedDate))
                return BadRequest("Formato de data inválido");

            var appointments = await _clinicService.GetAppointmentsByDate(parsedDate);
            return Ok(_mapper.Map<IEnumerable<AppointmentReadDTO>>(appointments));
        }
    }
}