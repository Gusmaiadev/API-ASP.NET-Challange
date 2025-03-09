namespace DentalClinicAPI.DTOs
{
    public class DentistCreateDTO
    {
        public string Name { get; set; }
        public string CRM { get; set; }
        public string Specialty { get; set; }
    }

    public class DentistReadDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string CRM { get; set; }
        public string Specialty { get; set; }
    }
}