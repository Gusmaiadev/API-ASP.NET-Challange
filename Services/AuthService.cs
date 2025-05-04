using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using DentalClinicAPI.Data;
using DentalClinicAPI.DTOs;
using DentalClinicAPI.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace DentalClinicAPI.Services
{
    public class AuthService : IAuthService
    {
        private readonly ClinicContext _context;
        private readonly IConfiguration _configuration;

        public AuthService(ClinicContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        public async Task<AuthResponseDTO> Register(RegisterDTO registerDto)
        {
            if (await UserExists(registerDto.Username))
            {
                throw new Exception("Username already exists");
            }

            CreatePasswordHash(registerDto.Password, out string passwordHash, out string passwordSalt);

            var user = new User
            {
                Username = registerDto.Username,
                Email = registerDto.Email,
                PasswordHash = passwordHash,
                PasswordSalt = passwordSalt,
                Role = "User" // Default role
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            // Create token
            string token = CreateToken(user);

            return new AuthResponseDTO
            {
                Username = user.Username,
                Token = token,
                Role = user.Role,
                Expiration = DateTime.Now.AddDays(1)
            };
        }

        public async Task<AuthResponseDTO> Login(LoginDTO loginDto)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Username.ToLower() == loginDto.Username.ToLower());

            if (user == null)
            {
                throw new Exception("User not found");
            }

            // Verifique a senha
            if (!VerifyPasswordHash(loginDto.Password, user.PasswordHash, user.PasswordSalt))
            {
                throw new Exception("Wrong password");
            }

            // Crie o token
            string token = CreateToken(user);

            return new AuthResponseDTO
            {
                Username = user.Username,
                Token = token,
                Role = user.Role,
                Expiration = DateTime.Now.AddDays(1)
            };
        }

        public async Task<User> GetUserByUsername(string username)
        {
            return await _context.Users
                .FirstOrDefaultAsync(u => u.Username.ToLower() == username.ToLower());
        }

        public string CreateToken(User user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Role, user.Role)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
                _configuration.GetSection("AppSettings:Token").Value ?? throw new Exception("Token key not found in configuration")));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.Now.AddDays(1),
                signingCredentials: creds
            );

            var jwt = new JwtSecurityTokenHandler().WriteToken(token);

            return jwt;
        }

        private async Task<bool> UserExists(string username)
        {
            // Use .Count() > 0 em vez de .Any() para evitar problemas com Oracle
            return await _context.Users.CountAsync(
                u => u.Username.ToLower() == username.ToLower()) > 0;
        }

        private void CreatePasswordHash(string password, out string passwordHash, out string passwordSalt)
        {
            using var hmac = new HMACSHA512();
            // Convertendo byte[] para string Base64
            passwordSalt = Convert.ToBase64String(hmac.Key);
            var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
            passwordHash = Convert.ToBase64String(hash);
        }

        private bool VerifyPasswordHash(string password, string storedHash, string storedSalt)
        {
            try
            {
                // Convertendo string Base64 para byte[]
                var saltBytes = Convert.FromBase64String(storedSalt);
                using var hmac = new HMACSHA512(saltBytes);
                var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
                var computedHashString = Convert.ToBase64String(computedHash);

                return computedHashString == storedHash;
            }
            catch (Exception)
            {
                // Em caso de erro na conversão ou no processo, retorne falso para segurança
                return false;
            }
        }
    }
}