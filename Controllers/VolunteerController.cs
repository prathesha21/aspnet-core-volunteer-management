using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using VolunteerApp1.Data;
using VolunteerApp1.Models.Dtos;
using VolunteerApp1.Repository;

namespace VolunteerApp1.Controllers
{
    //[Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class VolunteerController : ControllerBase
    {
        private readonly IVolunteer volunteer;
        private readonly IConfiguration configuration;
        private readonly VolunteerDbContext db;

        public VolunteerController(IVolunteer volunteer, IConfiguration configuration, VolunteerDbContext db)
        {
            this.volunteer = volunteer;
            this.configuration = configuration;
            this.db = db;
        }
        [Authorize(Roles ="Admin")]
        [HttpGet]
        public IActionResult GetAllVolunteers()
        {
            var volunteers = volunteer.GetAllVolunteers();
            return Ok(volunteers);
        }
        [HttpGet("{id:int}")]
        public IActionResult GetById(int id)
        {
            var vol = volunteer.GetById(id);
            if (vol == null)
            {
                return NotFound();
            }
            return Ok(vol);
        }
        [HttpPost]
        public IActionResult AddVolunteer(VolunteerDto volunteerDto)
        {
            var newVolunteer = volunteer.AddVolunteer(volunteerDto);

            return Ok(newVolunteer);

        }
        [HttpPut("{id:int}")]
        public IActionResult UpdateVolunteer(int id, VolunteerDto volunteerDto)
        {
            var updatedVolunteer = volunteer.UpdateVolunteer(id, volunteerDto);
            if (updatedVolunteer == null)
            {
                return NotFound();
            }
            return Ok(updatedVolunteer);
        }
        [HttpDelete("{id:int}")]
        public IActionResult DeleteVolunteer(int id)
        {
            volunteer.DeleteVolunteer(id);
            return NoContent();
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public IActionResult PostLogin(LoginDto uuser)
        {
            var result = new LoginResponseDto();

            var user = db.VolunteersRecords.FirstOrDefault(p => p.Email == uuser.Email && p.Name == uuser.Password);
            if (user != null)
            {
                var token = GenerateJwtToken(user.Name, user.Role);
                result.Id = user.Id;
                result.Username = user.Name;
                result.Token = token;
                result.Role = user.Role;
                return Ok(result);
            }
            else
            {
                return Unauthorized("Invalid username and password");

            }

        }
        private string GenerateJwtToken(string username, string role)
        {
            var jwtSettings = configuration.GetSection("Jwt");
            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwtSettings["Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var claims = new[]
            {
    new Claim(JwtRegisteredClaimNames.Sub, username),
    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
    new Claim(ClaimTypes.Role, role)
};

            var token = new JwtSecurityToken(
                issuer: jwtSettings["Issuer"],
                audience: jwtSettings["Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(Convert.ToDouble(jwtSettings["ExpiryInMinutes"])),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

    }

}
