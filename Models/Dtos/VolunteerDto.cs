using System.ComponentModel.DataAnnotations;

namespace VolunteerApp1.Models.Dtos
{
    public class VolunteerDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Gender { get; set; }
        public DateOnly DateofJoining { get; set; }

        public string Area { get; set; }

        public int AvailableWeekends { get; set; }

        public string Role { get; set; }

        [MaxLength(500)]
        public string ProfileImageUrl { get; set; }
    }
}
