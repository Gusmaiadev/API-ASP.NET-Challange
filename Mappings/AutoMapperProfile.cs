// AutoMapperProfile.cs
using DentalClinicAPI.DTOs;
using DentalClinicAPI.Models;
using AutoMapper;
namespace DentalClinicAPI.Mappings;
using static System.Runtime.InteropServices.JavaScript.JSType;

public class AutoMapperProfile : Profile
{
    public AutoMapperProfile()
    {
        CreateMap<Patient, PatientReadDTO>();
        CreateMap<PatientCreateDTO, Patient>();

        CreateMap<Dentist, DentistReadDTO>();
        CreateMap<DentistCreateDTO, Dentist>();

        CreateMap<Appointment, AppointmentReadDTO>();
        CreateMap<AppointmentCreateDTO, Appointment>();

        CreateMap<User, AuthResponseDTO>();
        CreateMap<RegisterDTO, User>();
    }
}
