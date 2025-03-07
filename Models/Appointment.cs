namespace DentalClinicAPI.Models;

public class Appointment
{
    public int Id { get; set; }
    public DateTime Date { get; set; }
    public string Procedure { get; set; } = string.Empty;
    public int PatientId { get; set; }
    public Patient Patient { get; set; } = new Patient();
    public int DentistId { get; set; }
    public Dentist Dentist { get; set; } = new Dentist();
}