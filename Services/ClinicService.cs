using DentalClinicAPI.Models;        // Adicione esta linha
using DentalClinicAPI.Repositories;  // Adicione esta linha
using System.Threading.Tasks;
namespace DentalClinicAPI.Services;

public class ClinicService : IClinicService
{
    private readonly IRepository<Appointment> _appointmentRepo;
    private readonly IRepository<Patient> _patientRepo;

    public ClinicService(IRepository<Appointment> appointmentRepo, IRepository<Patient> patientRepo)
    {
        _appointmentRepo = appointmentRepo;
        _patientRepo = patientRepo;
    }

    public async Task<List<Appointment>> GetAppointmentsByDate(DateTime date)
    {
        return (await _appointmentRepo.GetAll())
            .Where(a => a.Date.Date == date.Date)
            .ToList();
    }

    public async Task<Patient> GetPatientWithRecords(int id)
    {
        return await _patientRepo.GetById(id);
    }
}