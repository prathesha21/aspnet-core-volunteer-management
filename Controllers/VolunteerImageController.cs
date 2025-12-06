using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using System.Threading.Tasks;
using System;
using VolunteerApp1.Data;
using Microsoft.AspNetCore.Authorization;


namespace VolunteerApp1.Controllers
{

    [Route("api/volunteers")]
    [ApiController]
    //[Authorize] // 
    public class VolunteerImageController : ControllerBase
    {

        private readonly VolunteerDbContext _context;
        private readonly IWebHostEnvironment _env;

        public VolunteerImageController(VolunteerDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        [HttpPost("{volunteerId}/upload-image")]
        public async Task<IActionResult> UploadProfileImage(int volunteerId, IFormFile file)
        {
            // 1. Basic Validation checks
            if (file == null || file.Length == 0)
                return BadRequest(new { Message = "No file uploaded or file is empty." });

            // 2. Fetch the existing Volunteer record
            var volunteer = await _context.VolunteersRecords
                                           .FirstOrDefaultAsync(v => v.Id == volunteerId);

            if (volunteer == null)
                return NotFound(new { Message = $"Volunteer with ID {volunteerId} not found." });

            try
            {
                // 3. Define the upload directory path (wwwroot/images/profiles)
                var uploadsFolder = Path.Combine(_env.WebRootPath, "images", "profiles");
                Directory.CreateDirectory(uploadsFolder); // Create folder if it doesn't exist

                // 4. Generate a unique file name using GUID
                var fileExtension = Path.GetExtension(file.FileName).ToLowerInvariant();
                var uniqueFileName = $"{volunteerId}_{Guid.NewGuid()}{fileExtension}";
                var fullPath = Path.Combine(uploadsFolder, uniqueFileName);

                // 5. Save the File to the Server's hard drive
                using (var stream = new FileStream(fullPath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                // 6. Clean up the old file (Delete the previously uploaded image, if one existed)
                if (!string.IsNullOrEmpty(volunteer.ProfileImageUrl))
                {
                    // Convert relative URL path to physical path for deletion
                    var oldPath = Path.Combine(_env.WebRootPath, volunteer.ProfileImageUrl.TrimStart('/'));
                    if (System.IO.File.Exists(oldPath))
                    {
                        System.IO.File.Delete(oldPath);
                    }
                }

                // 7. Update the Database with the new relative URL/Path
                volunteer.ProfileImageUrl = $"/images/profiles/{uniqueFileName}";
                await _context.SaveChangesAsync();

                return Ok(new
                {
                    Message = "Image uploaded and database updated successfully.",
                    Url = volunteer.ProfileImageUrl
                });
            }
            catch (Exception ex)
            {
                // Handle any unexpected errors during the file operation
                return StatusCode(500, new { Message = $"Internal server error: {ex.Message}" });
            }
        }
        [HttpGet("{volunteerId}/profile-image")]
        public async Task<IActionResult> GetProfileImageUrl(int volunteerId)
        {
            var volunteer = await _context.VolunteersRecords
                                          .FirstOrDefaultAsync(v => v.Id == volunteerId);

            if (volunteer == null)
                return NotFound(new { Message = $"Volunteer with ID {volunteerId} not found." });

            if (string.IsNullOrEmpty(volunteer.ProfileImageUrl))
                return NotFound(new { Message = "No profile image uploaded for this volunteer." });

            return Ok(new
            {
                VolunteerId = volunteer.Id,
                ProfileImageUrl = volunteer.ProfileImageUrl
            });
        }

    }
}