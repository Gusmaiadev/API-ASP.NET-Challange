using DentalClinicAPI.Models;        // Adicione esta linha
using System.Collections.Generic;
using System.Threading.Tasks;
namespace DentalClinicAPI.Services;
public interface IClinicService
{
    Task<List<Appointment>> GetAppointmentsByDate(DateTime date);
    Task<Patient> GetPatientWithRecords(int id);
}