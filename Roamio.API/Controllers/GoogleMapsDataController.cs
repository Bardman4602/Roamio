using Microsoft.AspNetCore.Mvc;
using Amazon.DynamoDBv2.DataModel;
using Roamio.API.Models;
using System.Threading.Tasks;


namespace Roamio.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class GoogleMapsDataController : ControllerBase
    {
        private readonly IDynamoDBContext _context;

        public GoogleMapsDataController(IDynamoDBContext context)
        {
            _context = context;
        }

        [HttpGet("{userId}/{location}")]
        public async Task<IActionResult> GetGoogleMapsData(string userId, string location)
        {
            var data = await _context.LoadAsync<GoogleMapsData>(userId, location);
            if (data == null)
            {
                return NotFound();
            }

            return Ok(data);
        }

        [HttpPost]
        public async Task<IActionResult> CreateGoogleMapsData([FromBody] GoogleMapsData data)
        {
            if (data == null)
            {
                return BadRequest();
            }
            await _context.SaveAsync(data);

            return CreatedAtAction(nameof(GetGoogleMapsData), new { userId = data.UserId, location = data.Location }, data);
        }

        [HttpPut("{userId}/{location}")]
        public async Task<IActionResult> UpdateGoogleMapsData(string userId, string location, [FromBody] GoogleMapsData updatedData)
        {
            if (updatedData == null || userId != updatedData.UserId || location != updatedData.Location)
            {
                return BadRequest();
            }

            var existingData = await _context.LoadAsync<GoogleMapsData>(userId, location);
            if (existingData == null)
            {
                return NotFound();
            }
            await _context.SaveAsync(updatedData);
            return NoContent();
        }

        [HttpDelete("{userId}/{location}")]
        public async Task<IActionResult> DeleteGoogleMapsData(string userId, string location)
        {
            var existingData = await _context.LoadAsync<GoogleMapsData>(userId, location);
            if (existingData == null)
            {
                return NotFound();
            }
            await _context.DeleteAsync(existingData);
            return NoContent();
        }
    }
}
