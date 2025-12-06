namespace VolunteerApp1.Models.Dtos
{
    public class LoginDto
    {
        public string Email { get; set; }
        public string Password { get; set; }

    }

    public class LoginResponseDto
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Token { get; set; }

        public string Role { get; set; }
    }
}
