using Microsoft.EntityFrameworkCore;
using DentalClinicAPI.Models;

namespace DentalClinicAPI.Data
{
    public class ClinicContext : DbContext
    {
        // Adicione este construtor
        public ClinicContext(DbContextOptions<ClinicContext> options)
            : base(options)
        {
        }

        public DbSet<Patient> Patients { get; set; }
        public DbSet<Dentist> Dentists { get; set; }
        public DbSet<Appointment> Appointments { get; set; }
    }
}