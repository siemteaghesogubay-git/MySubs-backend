using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MySubs.Dtos;
using MySubs.Services.IServices;

namespace MySubs.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class FamilyGroupsController : ControllerBase
    {
        private readonly IFamilyGroupService _familyGroupService;

        public FamilyGroupsController(IFamilyGroupService familyGroupService)
        {
            _familyGroupService = familyGroupService;
        }

        private string UserId => User.FindFirstValue(ClaimTypes.NameIdentifier)!;

        [HttpGet]
        public async Task<ActionResult<List<FamilyGroupResponseDto>>> GetMyGroups()
        {
            return Ok(await _familyGroupService.GetMyGroupsAsync(UserId));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<FamilyGroupResponseDto>> GetById(int id)
        {
            var result = await _familyGroupService.GetByIdAsync(id, UserId);
            return result is null ? NotFound() : Ok(result);
        }

        [HttpPost]
        public async Task<ActionResult<FamilyGroupResponseDto>> Create(FamilyGroupCreateDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var created = await _familyGroupService.CreateAsync(dto, UserId);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        [HttpPost("{id}/members")]
        public async Task<ActionResult<FamilyGroupResponseDto>> AddMember(int id, AddMemberDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var result = await _familyGroupService.AddMemberAsync(id, dto, UserId);
            return result is null ? Forbid() : Ok(result);
        }

        [HttpDelete("{id}/members/{userId}")]
        public async Task<IActionResult> RemoveMember(int id, string userId)
        {
            var success = await _familyGroupService.RemoveMemberAsync(id, userId, UserId);
            return success ? NoContent() : NotFound();
        }
    }
}