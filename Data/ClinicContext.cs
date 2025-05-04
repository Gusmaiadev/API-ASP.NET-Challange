using Microsoft.EntityFrameworkCore;
using DentalClinicAPI.Models;
using DentalClinicAPI.Conversors;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace DentalClinicAPI.Data
{
    public class ClinicContext : DbContext
    {
        // Construtor
        public ClinicContext(DbContextOptions<ClinicContext> options)
            : base(options)
        {
        }

        // DbSets para as entidades
        public DbSet<Patient> Patients { get; set; }
        public DbSet<Dentist> Dentists { get; set; }
        public DbSet<Appointment> Appointments { get; set; }
        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configurações específicas para Oracle
            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                // Configura todos os campos booleanos para serem armazenados como números
                foreach (var property in entityType.GetProperties())
                {
                    if (property.ClrType == typeof(bool))
                    {
                        property.SetValueConverter(new BoolToIntConverter());
                    }
                    else if (property.ClrType == typeof(bool?))
                    {
                        property.SetValueConverter(new NullableBoolToIntConverter());
                    }
                }
            }

            // Configure a tabela Users
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Username).IsRequired().HasMaxLength(50);
                entity.Property(e => e.Email).IsRequired().HasMaxLength(100);
                entity.Property(e => e.PasswordHash).IsRequired();
                entity.Property(e => e.PasswordSalt).IsRequired();
                entity.Property(e => e.Role).IsRequired().HasMaxLength(20);

                // Índice único para username para garantir unicidade
                entity.HasIndex(e => e.Username).IsUnique();
            });
        }
    }

    // Classes de conversão movidas para o mesmo arquivo para simplificar (pode ser movida para um arquivo separado)
    public class BoolToIntConverter : ValueConverter<bool, int>
    {
        public BoolToIntConverter()
            : base(
                v => v ? 1 : 0,
                v => v == 1)
        {
        }
    }

    public class NullableBoolToIntConverter : ValueConverter<bool?, int?>
    {
        public NullableBoolToIntConverter()
            : base(
                v => v.HasValue ? (v.Value ? 1 : 0) : null,
                v => v.HasValue ? (v == 1) : null)
        {
        }
    }
}