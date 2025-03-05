using Microsoft.AspNetCore.Mvc;
using Amazon.DynamoDBv2.DataModel;
using Roamio.API.Models;
using System.Threading.Tasks;

namespace Roamio.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserPreferencesController : ControllerBase
    {
        private readonly IDynamoDBContext _context;

        public UserPreferencesController(IDynamoDBContext context)
        {
            _context = context;
        }

        [HttpGet("{id}/{userId}")]
        public async Task<IActionResult> GetUserPreferences(string id, string userId)
        {
            var preference = await _context.LoadAsync<UserPreferences>(id, userId);
            if (preference == null)
            {
                return NotFound();
            }
            return Ok(preference);
        }

        [HttpPost]
        public async Task<IActionResult> CreateUserPreference([FromBody] UserPreferences preferences)
        {
            if (preferences == null)
            {
                return BadRequest();
            }

            if (string.IsNullOrEmpty(preferences.Id))
            {
                preferences.Id = System.Guid.NewGuid().ToString();
            }
            await _context.SaveAsync(preferences);

            return CreatedAtAction(nameof(GetUserPreferences), new { id = preferences.Id, userId = preferences.UserId }, preferences);
        }

        [HttpPut("{id}/{userId}")]
        public async Task<IActionResult> UpdateUserPreferences(string id, string userId, [FromBody] UserPreferences updatedPreferences)
        {
            if (updatedPreferences == null || id != updatedPreferences.Id || userId != updatedPreferences.UserId)
            {
                return BadRequest();
            }

            var existingPreferences = await _context.LoadAsync<UserPreferences>(id, userId);
            if (existingPreferences == null)
            {
                NotFound();
            }
            await _context.SaveAsync(updatedPreferences);

            return NoContent();
        }

        [HttpDelete("{id}/{userId}")]
        public async Task<IActionResult> DeletePreferences(string id, string userId)
        {
            var existingPreferences = await _context.LoadAsync<UserPreferences>(id, userId);
            if (existingPreferences == null)
            {
                return NotFound();
            }
            await _context.DeleteAsync(existingPreferences);

            return NoContent();
        }
    }
}
