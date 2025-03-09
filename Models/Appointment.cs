namespace DentalClinicAPI.Models;
using Newtonsoft.Json;

public class Appointment
{
    public int Id { get; set; }
    public DateTime Date { get; set; }
    public string Procedure { get; set; } = string.Empty;
    public int PatientId { get; set; }
    public int DentistId { get; set; }

    [Newtonsoft.Json.JsonIgnore]
    public Patient Patient { get; set; } = null!;

    [Newtonsoft.Json.JsonIgnore]
    public Dentist Dentist { get; set; } = null!;
}