using System.Linq; // Adicione para operações LINQ
using DentalClinicAPI.Models;
using DentalClinicAPI.Repositories;
using System.Threading.Tasks;

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

        // Verifica disponibilidade do dentista
        public async Task<bool> IsDentistAvailable(int dentistId, DateTime date)
        {
            var dentist = await _dentistRepo.GetById(dentistId);
            if (dentist == null) return false;

            var appointments = await _appointmentRepo.GetAll();
            return !appointments.Any(a =>
                a.DentistId == dentistId &&
                a.Date.Date == date.Date);
        }

        // Verifica existência do paciente
        public async Task<bool> PatientExists(int patientId)
        {
            return await _patientRepo.GetById(patientId) != null;
        }

        // Obtém agendamentos por data
        public async Task<List<Appointment>> GetAppointmentsByDate(DateTime date)
        {
            var appointments = await _appointmentRepo.GetAll();
            return appointments
                .Where(a => a.Date.Date == date.Date)
                .ToList();
        }

        // Obtém paciente com histórico
        public async Task<Patient> GetPatientWithRecords(int id)
        {
            return await _patientRepo.GetById(id);
        }
    }
}