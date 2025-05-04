using Microsoft.AspNetCore.Mvc;
using DentalClinicAPI.DTOs;
using DentalClinicAPI.Services;
using Microsoft.AspNetCore.Authorization;

namespace DentalClinicAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        /// <summary>
        /// Registra um novo usuário
        /// </summary>
        /// <param name="registerDto">Dados do usuário</param>
        /// <returns>Token de autenticação</returns>
        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<ActionResult<AuthResponseDTO>> Register(RegisterDTO registerDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var response = await _authService.Register(registerDto);
                return Ok(response);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro interno: {ex.Message}");
            }
        }

        /// <summary>
        /// Autentica um usuário
        /// </summary>
        /// <param name="loginDto">Credenciais do usuário</param>
        /// <returns>Token de autenticação</returns>
        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<ActionResult<AuthResponseDTO>> Login(LoginDTO loginDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var response = await _authService.Login(loginDto);
                return Ok(response);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro interno: {ex.Message}");
            }
        }

        /// <summary>
        /// Obtém informações do usuário atual
        /// </summary>
        /// <returns>Dados do usuário autenticado</returns>
        [HttpGet("me")]
        [Authorize]
        public async Task<ActionResult<object>> GetCurrentUser()
        {
            try
            {
                var username = User.Identity?.Name;
                if (string.IsNullOrEmpty(username))
                {
                    return Unauthorized("Usuário não autenticado");
                }

                var user = await _authService.GetUserByUsername(username);
                if (user == null)
                {
                    return NotFound("Usuário não encontrado");
                }

                return Ok(new
                {
                    Username = user.Username,
                    Email = user.Email,
                    Role = user.Role
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro interno: {ex.Message}");
            }
        }
    }
}