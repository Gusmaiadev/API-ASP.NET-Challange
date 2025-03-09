namespace DentalClinicAPI.Models;


public class Dentist
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string CRM { get; set; } = string.Empty;
    public string Specialty { get; set; } = string.Empty;

    [Newtonsoft.Json.JsonIgnore]
    public List<Appointment> Appointments { get; set; } = new();
}