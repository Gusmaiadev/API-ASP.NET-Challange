using DentalClinicAPI.Models;

namespace DentalClinicAPI.Services
{
    public interface IMLService
    {
        // Treinar modelo com dados históricos
        Task TrainModel();

        // Obter recomendações de horários para um paciente e especialidade
        Task<List<TimeSlotRecommendation>> GetRecommendedTimeSlots(int patientId, string specialty);

        // Prever probabilidade de comparecimento para um agendamento
        Task<float> PredictAttendanceProbability(AppointmentData appointmentData);
    }
}