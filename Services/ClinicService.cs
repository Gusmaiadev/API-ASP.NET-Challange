using DentalClinicAPI.Models;
using DentalClinicAPI.Repositories;

namespace DentalClinicAPI.Services
{
    public class ClinicService : IClinicService
    {
        private readonly IRepository<Appointment> _appointmentRepo;
        private readonly IRepository<Patient> _patientRepo;
        private readonly IRepository<Dentist> _dentistRepo;

        public ClinicService(
            IRepository<Appointment> appointmentRepo,
            IRepository<Patient> patientRepo,
            IRepository<Dentist> dentistRepo)
        {
            _appointmentRepo = appointmentRepo;
            _patientRepo = patientRepo;
            _dentistRepo = dentistRepo;
        }

        public async Task<bool> IsDentistAvailable(int dentistId, DateTime date)
        {
            var dentist = await _dentistRepo.GetById(dentistId);
            if (dentist == null) return false;

            var appointments = await _appointmentRepo.GetAll();
            return !appointments.Any(a =>
                a.DentistId == dentistId &&
                a.Date.Date == date.Date &&
                a.Date.Hour == date.Hour);
        }

        public async Task<bool> PatientExists(int patientId)
        {
            return await _patientRepo.GetById(patientId) != null;
        }

        public async Task<List<Appointment>> GetAppointmentsByDate(DateTime date)
        {
            var appointments = await _appointmentRepo.GetAll();
            return appointments
                .Where(a => a.Date.Date == date.Date)
                .ToList();
        }

        public Task<bool> IsAppointmentTimeValid(DateTime appointmentTime)
        {
            // Verificar se o horário é válido (dentro do horário de funcionamento da clínica)
            bool isWithinBusinessHours = appointmentTime.Hour >= 8 && appointmentTime.Hour < 18;
            bool isWeekday = appointmentTime.DayOfWeek != DayOfWeek.Saturday && appointmentTime.DayOfWeek != DayOfWeek.Sunday;

            return Task.FromResult(isWithinBusinessHours && isWeekday);
        }
    }
}