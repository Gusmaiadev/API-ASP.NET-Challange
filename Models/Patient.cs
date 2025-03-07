namespace DentalClinicAPI.Models;

public class Patient

{
    public Patient()
    {
        Appointments = new List<Appointment>();
    }

    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string CPF { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string MedicalHistory { get; set; } = string.Empty;
    public List<Appointment> Appointments { get; set; }
}