namespace DentalClinicAPI.DTOs
{
    public class PatientCreateDTO
    {
        public string Name { get; set; }
        public string CPF { get; set; }
        public string Phone { get; set; }
        public string MedicalHistory { get; set; }
    }

    public class PatientReadDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string CPF { get; set; }
        public string Phone { get; set; }
        public string MedicalHistory { get; set; }
    }
}