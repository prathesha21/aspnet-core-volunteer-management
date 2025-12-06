using Microsoft.EntityFrameworkCore;

namespace VolunteerApp1.Data
{
    public class VolunteerDbContext: DbContext
    {
        public VolunteerDbContext(DbContextOptions<VolunteerDbContext> options) : base(options)
        {
        }
        public DbSet<Models.Entities.VolunteerEntity> VolunteersRecords { get; set; }
    }
}
