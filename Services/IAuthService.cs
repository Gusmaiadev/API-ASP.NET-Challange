using DentalClinicAPI.DTOs;
using DentalClinicAPI.Models;

namespace DentalClinicAPI.Services
{
    public interface IAuthService
    {
        Task<AuthResponseDTO> Register(RegisterDTO registerDto);
        Task<AuthResponseDTO> Login(LoginDTO loginDto);
        Task<User> GetUserByUsername(string username);
        string CreateToken(User user);
    }
}