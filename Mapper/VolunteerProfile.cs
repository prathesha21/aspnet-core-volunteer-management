using AutoMapper;
using VolunteerApp1.Models.Dtos;
using VolunteerApp1.Models.Entities;

namespace VolunteerApp1.Mapper
{
    public class VolunteerProfile : Profile
    {
        public VolunteerProfile()
        {
            CreateMap<VolunteerDto, VolunteerEntity>().ReverseMap();
        }
    }
}
