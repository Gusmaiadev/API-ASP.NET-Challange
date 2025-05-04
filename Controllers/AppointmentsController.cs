using Microsoft.AspNetCore.Mvc;
using DentalClinicAPI.DTOs;
using DentalClinicAPI.Models;
using DentalClinicAPI.Repositories;
using DentalClinicAPI.Services;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;

namespace DentalClinicAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
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
        public async Task<ActionResult<IEnumerable<AppointmentReadDTO>>> GetAll()
        {
            try
            {
                var appointments = await _appointmentRepo.GetAll();
                return Ok(_mapper.Map<IEnumerable<AppointmentReadDTO>>(appointments));
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro interno: {ex.Message}");
            }
        }

        /// <summary>
        /// Obtém agendamento por ID
        /// </summary>
        /// <param name="id">ID do agendamento</param>
        /// <returns>Detalhes do agendamento</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<AppointmentReadDTO>> GetById(int id)
        {
            try
            {
                var appointment = await _appointmentRepo.GetById(id);
                if (appointment == null) return NotFound("Agendamento não encontrado");

                return Ok(_mapper.Map<AppointmentReadDTO>(appointment));
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro interno: {ex.Message}");
            }
        }

        /// <summary>
        /// Cria um novo agendamento
        /// </summary>
        /// <param name="dto">Dados do agendamento</param>
        /// <returns>Agendamento criado</returns>
        [HttpPost]
        public async Task<ActionResult<AppointmentReadDTO>> Create(AppointmentCreateDTO dto)
        {
            try
            {
                // Validações de negócio
                if (!await _clinicService.IsAppointmentTimeValid(dto.Date))
                    return BadRequest("Horário fora do período de funcionamento da clínica");

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
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro interno: {ex.Message}");
            }
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
            try
            {
                var existingAppointment = await _appointmentRepo.GetById(id);
                if (existingAppointment == null) return NotFound("Agendamento não encontrado");

                // Validação de horário comercial
                if (!await _clinicService.IsAppointmentTimeValid(dto.Date))
                    return BadRequest("Horário fora do período de funcionamento da clínica");

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
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro interno: {ex.Message}");
            }
        }

        /// <summary>
        /// Exclui um agendamento
        /// </summary>
        /// <param name="id">ID do agendamento</param>
        /// <returns>Status 204 No Content</returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var appointment = await _appointmentRepo.GetById(id);
                if (appointment == null) return NotFound("Agendamento não encontrado");

                await _appointmentRepo.Delete(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro interno: {ex.Message}");
            }
        }

        /// <summary>
        /// Obtém agendamentos por data
        /// </summary>
        /// <param name="date">Data no formato yyyy-MM-dd</param>
        /// <returns>Lista de agendamentos</returns>
        [HttpGet("by-date/{date}")]
        public async Task<ActionResult<IEnumerable<AppointmentReadDTO>>> GetByDate(string date)
        {
            try
            {
                if (!DateTime.TryParse(date, out var parsedDate))
                    return BadRequest("Formato de data inválido");

                var appointments = await _clinicService.GetAppointmentsByDate(parsedDate);
                return Ok(_mapper.Map<IEnumerable<AppointmentReadDTO>>(appointments));
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro interno: {ex.Message}");
            }
        }
    }
}