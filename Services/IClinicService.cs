using DentalClinicAPI.Models;        // Adicione esta linha
using System.Collections.Generic;
using System.Threading.Tasks;
namespace DentalClinicAPI.Services;
public interface IClinicService
{
    Task<bool> IsDentistAvailable(int dentistId, DateTime date);
    Task<bool> PatientExists(int patientId);
    Task<List<Appointment>> GetAppointmentsByDate(DateTime date);
}