using AutoMapper;
using VolunteerApp1.Data;
using VolunteerApp1.Models.Dtos;
using VolunteerApp1.Models.Entities;

namespace VolunteerApp1.Repository
{
    public interface IVolunteer
    {
        public List<VolunteerDto> GetAllVolunteers();
        public VolunteerDto GetById(int id);
        public VolunteerDto AddVolunteer(VolunteerDto volunteerDto);
        public VolunteerDto UpdateVolunteer(int id, VolunteerDto volunteerDto);
        public void DeleteVolunteer(int id);

    }
    public class VolunteerRepository : IVolunteer
    {
        private readonly IMapper mapper;
        private readonly VolunteerDbContext dbContext;

        public VolunteerRepository(IMapper mapper, VolunteerDbContext dbContext)
        {
            this.mapper = mapper;
            this.dbContext = dbContext;
        }

        public List<VolunteerDto> GetAllVolunteers()
        {
            var volunteers = dbContext.VolunteersRecords.ToList();

            var res = mapper.Map<List<VolunteerDto>>(volunteers);
            return res;

        }
        public VolunteerDto GetById(int id)
        {
            var volunteer = dbContext.VolunteersRecords.Find(id);
            var res = mapper.Map<VolunteerDto>(volunteer);
            return res;
        }
        public VolunteerDto AddVolunteer(VolunteerDto volunteerDto)
        {
            var volunteerEntity = mapper.Map<VolunteerEntity>(volunteerDto);
            dbContext.VolunteersRecords.Add(volunteerEntity);
            dbContext.SaveChanges();
            var res = mapper.Map<VolunteerDto>(volunteerEntity);
            dbContext.SaveChanges();
            return res;
        }

        public VolunteerDto UpdateVolunteer(int id, VolunteerDto volunteerDto)
        {
            var existingVolunteer = dbContext.VolunteersRecords.Find(id);
            if (existingVolunteer == null)
            {
                return null;
            }
            existingVolunteer = mapper.Map(volunteerDto, existingVolunteer);
            dbContext.SaveChanges();
            var res = mapper.Map<VolunteerDto>(existingVolunteer);
            return res;
        }
        public void DeleteVolunteer(int id)
        {
            var volunteer = dbContext.VolunteersRecords.Find(id);
            if (volunteer != null)
            {
                dbContext.VolunteersRecords.Remove(volunteer);
                dbContext.SaveChanges();
            }
        }
    }
}
