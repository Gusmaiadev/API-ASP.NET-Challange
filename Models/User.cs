// Models/User.cs
namespace DentalClinicAPI.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public string PasswordSalt { get; set; } = string.Empty; // Armazenamos como string Base64
        public string Role { get; set; } = "User"; // Role para autorização: "Admin", "User", etc.
    }
}