using DentalClinicAPI.Models;

namespace DentalClinicAPI.Services
{
    // Interface para verificações
    public interface IAvailabilityService
    {
        Task<bool> IsDentistAvailable(int dentistId, DateTime date);
        Task<bool> PatientExists(int patientId);
    }

    // Interface para operações de agendamento
    public interface IAppointmentService
    {
        Task<List<Appointment>> GetAppointmentsByDate(DateTime date);
        Task<bool> IsAppointmentTimeValid(DateTime appointmentTime);
    }

    // Interface combinada para o serviço da clínica
    public interface IClinicService : IAvailabilityService, IAppointmentService
    {
    }
}