using Microsoft.AspNetCore.Mvc;
using Amazon.DynamoDBv2.DataModel;
using Roamio.API.Models;
using System.Threading.Tasks;

namespace Roamio.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TripController : ControllerBase
    {
        private readonly IDynamoDBContext _context;

        public TripController(IDynamoDBContext context)
        {
            _context = context;
        }

        [HttpGet("{id}/{userId}")]
        public async Task<IActionResult> GetTrip(string id, string userId)
        {
            var trip = await _context.LoadAsync<Trip>(id, userId);
            if (trip == null)
            {
                return NotFound();
            }
            return Ok(trip);
        }

        [HttpPost]
        public async Task<IActionResult> CreateTrip([FromBody] Trip trip)
        {
            Console.WriteLine("Received request to create trip...");
            if (trip == null)
            {
                Console.WriteLine("Trip is null.");
                return BadRequest();
            }

            if (string.IsNullOrEmpty(trip.Id))
            {
                trip.Id = System.Guid.NewGuid().ToString();
            }
            await _context.SaveAsync(trip);

            return CreatedAtAction(nameof(GetTrip), new { id = trip.Id, userId = trip.UserId }, trip);
            Console.WriteLine("Trip created successfully.");
        }

        [HttpPut("{id}/{userId}")]
        public async Task<IActionResult> UpdateTrip(string id, string userId, [FromBody] Trip updatedTrip)
        {
            if (updatedTrip == null || id != updatedTrip.Id || userId != updatedTrip.UserId)
            {
                return BadRequest();
            }

            var existingTrip = await _context.LoadAsync<Trip>(id, userId);
            if (existingTrip == null)
            {
                return NotFound();
            }
            await _context.SaveAsync(updatedTrip);

            return NoContent();
        }

        [HttpDelete("{id}/{userId}")]
        public async Task<IActionResult> DeleteTrip(string id, string userId)
        {
            var existingTrip = await _context.LoadAsync<Trip>(id, userId);
            if (existingTrip == null)
            {
                return NotFound();
            }
            await _context.DeleteAsync(existingTrip);

            return NoContent();
        }
    }
}
