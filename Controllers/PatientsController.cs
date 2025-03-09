﻿using Microsoft.AspNetCore.Mvc;
using DentalClinicAPI.DTOs;
using DentalClinicAPI.Models;
using DentalClinicAPI.Repositories;
using AutoMapper;
using System.Threading.Tasks;

namespace DentalClinicAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PatientsController : ControllerBase
    {
        private readonly IRepository<Patient> _patientRepo;
        private readonly IMapper _mapper;

        public PatientsController(IRepository<Patient> patientRepo, IMapper mapper)
        {
            _patientRepo = patientRepo;
            _mapper = mapper;
        }

        /// <summary>
        /// Obtém todos os pacientes
        /// </summary>
        /// <returns>Lista de pacientes</returns>
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var patients = await _patientRepo.GetAll();
            return Ok(_mapper.Map<IEnumerable<PatientReadDTO>>(patients));
        }

        /// <summary>
        /// Obtém paciente por ID
        /// </summary>
        /// <param name="id">ID do paciente</param>
        /// <returns>Dados do paciente</returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var patient = await _patientRepo.GetById(id);
            if (patient == null) return NotFound();

            return Ok(_mapper.Map<PatientReadDTO>(patient));
        }

        /// <summary>
        /// Cria um novo paciente
        /// </summary>
        /// <param name="dto">Dados do paciente (excluindo ID)</param>
        /// <returns>Paciente criado</returns>
        [HttpPost]
        public async Task<IActionResult> Create(PatientCreateDTO dto)
        {
            var patient = _mapper.Map<Patient>(dto);
            await _patientRepo.Create(patient);

            return CreatedAtAction(
                nameof(GetById),
                new { id = patient.Id },
                _mapper.Map<PatientReadDTO>(patient)
            );
        }

        /// <summary>
        /// Atualiza um paciente existente
        /// </summary>
        /// <param name="id">ID do paciente</param>
        /// <param name="dto">Novos dados do paciente</param>
        /// <returns>Status 204 No Content</returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, PatientCreateDTO dto)
        {
            var existingPatient = await _patientRepo.GetById(id);
            if (existingPatient == null) return NotFound();

            _mapper.Map(dto, existingPatient);
            await _patientRepo.Update(existingPatient);

            return NoContent();
        }

        /// <summary>
        /// Exclui um paciente
        /// </summary>
        /// <param name="id">ID do paciente</param>
        /// <returns>Status 204 No Content</returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _patientRepo.Delete(id);
            return NoContent();
        }
    }
}