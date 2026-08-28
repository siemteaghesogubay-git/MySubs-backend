using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MySubs.Dtos;
using MySubs.Services.IServices;
using System.Security.Claims;

namespace MySubs.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class SubscriptionsController : ControllerBase
    {
        private readonly ISubscriptionService _subscriptionService;

        public SubscriptionsController(ISubscriptionService subscriptionService)
        {
            _subscriptionService = subscriptionService;
        }

        private string UserId => User.FindFirstValue(ClaimTypes.NameIdentifier)!;

        [HttpGet]
        public async Task<ActionResult<List<SubscriptionResponseDto>>> GetAll()
        {
            return Ok(await _subscriptionService.GetAllSubscriptionsAsync(UserId));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<SubscriptionResponseDto>> GetById(int id)
        {
            var result = await _subscriptionService.GetSubscriptionByIdAsync(id, UserId);
            return result is null ? NotFound() : Ok(result);
        }

        [HttpPost]
        public async Task<ActionResult<SubscriptionResponseDto>> Create(SubscriptionCreateDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var created = await _subscriptionService.CreateSubscriptionAsync(dto, UserId);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, SubscriptionUpdateDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var success = await _subscriptionService.UpdateSubscriptionAsync(id, dto, UserId);
            return success ? NoContent() : NotFound();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var success = await _subscriptionService.DeleteSubscriptionAsync(id, UserId);
            return success ? NoContent() : NotFound();
        }
    }
}