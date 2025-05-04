using Microsoft.AspNetCore.Mvc;
using DentalClinicAPI.Models;
using DentalClinicAPI.Services;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace DentalClinicAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] // Qualquer usuário autenticado pode acessar os endpoints
    public class MLRecommendationController : ControllerBase
    {
        private readonly IMLService _mlService;

        public MLRecommendationController(IMLService mlService)
        {
            _mlService = mlService;
        }

        /// <summary>
        /// Treina o modelo de machine learning com dados históricos
        /// </summary>
        /// <returns>Confirmação de treinamento</returns>
        [HttpPost("train")]
        // Removida a restrição: [Authorize(Roles = "Admin")]
        public async Task<IActionResult> TrainModel()
        {
            try
            {
                await _mlService.TrainModel();
                return Ok(new { message = "Modelo treinado com sucesso!" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro ao treinar modelo: {ex.Message}");
            }
        }

        /// <summary>
        /// Recomenda melhores horários para consulta com base em histórico
        /// </summary>
        /// <param name="patientId">ID do paciente</param>
        /// <param name="specialty">Especialidade odontológica</param>
        /// <returns>Lista de horários recomendados</returns>
        [HttpGet("recommend")]
        public async Task<ActionResult<List<TimeSlotRecommendation>>> GetRecommendedTimeSlots(
            [FromQuery] int patientId, [FromQuery] string specialty)
        {
            try
            {
                var recommendations = await _mlService.GetRecommendedTimeSlots(patientId, specialty);
                return Ok(recommendations);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro ao gerar recomendações: {ex.Message}");
            }
        }

        /// <summary>
        /// Verifica a probabilidade de comparecimento do paciente
        /// </summary>
        /// <param name="data">Dados da consulta para previsão</param>
        /// <returns>Probabilidade de comparecimento</returns>
        [HttpPost("predict-attendance")]
        public async Task<ActionResult<object>> PredictAttendance([FromBody] AppointmentData data)
        {
            try
            {
                var probability = await _mlService.PredictAttendanceProbability(data);
                return Ok(new
                {
                    probability,
                    riskLevel = probability < 0.6 ? "Alto" : (probability < 0.8 ? "Médio" : "Baixo"),
                    recommendation = probability < 0.7 ? "Recomendado contato de confirmação" : "Baixo risco de ausência"
                });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro ao prever comparecimento: {ex.Message}");
            }
        }
    }
}