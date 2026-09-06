using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MySubs.Data;
using MySubs.Dtos;
using MySubs.Models;

namespace MySubs.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] // kräver inloggning för samtliga endpoints; admin-only skyddas separat per metod
    public class UsersController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;

        public UsersController(UserManager<ApplicationUser> userManager, ApplicationDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        private string UserId => User.FindFirstValue(ClaimTypes.NameIdentifier)!;

        // ---------- Admin-only: hantera alla användare ----------

        [HttpGet]
        [Authorize(Roles = "ADMIN")]
        public async Task<ActionResult<List<UserResponseDto>>> GetAll()
        {
            var users = await _userManager.Users.ToListAsync();
            var result = new List<UserResponseDto>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                result.Add(new UserResponseDto
                {
                    Id = user.Id,
                    Email = user.Email!,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    PhoneNumber = user.PhoneNumber,
                    Roles = roles
                });
            }

            return Ok(result);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> Delete(string id)
        {
            // Skydd: en admin ska inte kunna radera sitt eget konto via admin-endpointen
            if (id == UserId)
                return BadRequest("Du kan inte radera ditt eget konto via den här endpointen. Använd DELETE /api/users/me istället.");

            var user = await _userManager.FindByIdAsync(id);
            if (user is null) return NotFound();

            var ownsGroup = await _context.FamilyGroups.AnyAsync(fg => fg.CreatedByUserId == id);
            if (ownsGroup)
                return BadRequest("Användaren äger en familjegrupp. Överför ägarskapet eller radera gruppen innan kontot kan raderas.");

            var result = await _userManager.DeleteAsync(user);
            if (!result.Succeeded) return BadRequest(result.Errors);

            return NoContent();
        }

        // ---------- Egen profil: alla inloggade användare ----------

        [HttpGet("/api/Users/me")]
        public async Task<ActionResult<UserResponseDto>> GetMyProfile()
        {
            var user = await _userManager.FindByIdAsync(UserId);
            if (user is null) return NotFound();

            var roles = await _userManager.GetRolesAsync(user);
            return Ok(new UserResponseDto
            {
                Id = user.Id,
                Email = user.Email!,
                FirstName = user.FirstName,
                LastName = user.LastName,
                PhoneNumber = user.PhoneNumber,
                Roles = roles
            });
        }

        [HttpPut("/api/Users/me")]
        public async Task<IActionResult> UpdateMyProfile(UpdateProfileDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var user = await _userManager.FindByIdAsync(UserId);
            if (user is null) return NotFound();

            user.FirstName = dto.FirstName;
            user.LastName = dto.LastName;
            user.PhoneNumber = dto.PhoneNumber;

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded) return BadRequest(result.Errors);

            return NoContent();
        }

        [HttpDelete("/api/Users/me")]
        public async Task<IActionResult> DeleteMyAccount()
        {
            var user = await _userManager.FindByIdAsync(UserId);
            if (user is null) return NotFound();

            var ownsGroup = await _context.FamilyGroups.AnyAsync(fg => fg.CreatedByUserId == UserId);
            if (ownsGroup)
                return BadRequest("Du äger en familjegrupp. Överför ägarskapet eller radera gruppen innan du raderar ditt konto.");

            // Kaskadraderar automatiskt Subscriptions, RefreshTokens och
            // FamilyGroupMember-poster tack vare DeleteBehavior.Cascade
            var result = await _userManager.DeleteAsync(user);
            if (!result.Succeeded) return BadRequest(result.Errors);

            return Ok(new { message = "Ditt konto och all tillhörande data har raderats permanent." });
        }
    }
}