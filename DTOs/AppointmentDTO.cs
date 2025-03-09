namespace DentalClinicAPI.DTOs
{
    public class AppointmentCreateDTO
    {
        public DateTime Date { get; set; }
        public string Procedure { get; set; }
        public int PatientId { get; set; }
        public int DentistId { get; set; }
    }

    public class AppointmentReadDTO
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public string Procedure { get; set; }
        public int PatientId { get; set; }
        public int DentistId { get; set; }
    }
}