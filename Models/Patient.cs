namespace DentalClinicAPI.Models;
using Newtonsoft.Json;
public class Patient
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string CPF { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string MedicalHistory { get; set; } = string.Empty;

    // Adicione [JsonIgnore] para evitar referências circulares no Swagger
    [Newtonsoft.Json.JsonIgnore]
    public List<Appointment> Appointments { get; set; } = new();
}