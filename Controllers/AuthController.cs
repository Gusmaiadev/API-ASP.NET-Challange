using Microsoft.AspNetCore.Mvc;
using DentalClinicAPI.DTOs;
using DentalClinicAPI.Services;
using System.Threading.Tasks;
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
        [AllowAnonymous] // Permite acesso sem autenticação
        public async Task<IActionResult> Register(RegisterDTO registerDto)
        {
            try
            {
                var response = await _authService.Register(registerDto);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Autentica um usuário
        /// </summary>
        /// <param name="loginDto">Credenciais do usuário</param>
        /// <returns>Token de autenticação</returns>
        [HttpPost("login")]
        [AllowAnonymous] // Permite acesso sem autenticação
        public async Task<IActionResult> Login(LoginDTO loginDto)
        {
            try
            {
                var response = await _authService.Login(loginDto);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Obtém informações do usuário atual
        /// </summary>
        /// <returns>Dados do usuário autenticado</returns>
        [HttpGet("me")]
        [Authorize] // Requer autenticação
        public async Task<IActionResult> GetCurrentUser()
        {
            var username = User.Identity.Name;
            var user = await _authService.GetUserByUsername(username);

            if (user == null)
                return NotFound("Usuário não encontrado");

            return Ok(new
            {
                Username = user.Username,
                Email = user.Email,
                Role = user.Role
            });
        }
    }
}