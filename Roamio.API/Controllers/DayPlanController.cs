using Microsoft.AspNetCore.Mvc;
using Amazon.DynamoDBv2.DataModel;
using Roamio.API.Models;
using System.Threading.Tasks;

namespace Roamio.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DayPlanController : ControllerBase
    {
        private readonly IDynamoDBContext _context;

        public DayPlanController(IDynamoDBContext context)
        {
            _context = context;
        }
                
        [HttpGet("{tripId}/{date}")]
        public async Task<IActionResult> GetDayPlan(string tripId, string date)
        {
            var dayPlan = await _context.LoadAsync<DayPlan>(tripId, date);
            if (dayPlan == null)
            {
                return NotFound();
            }

            return Ok(dayPlan);
        }

        
        [HttpPost]
        public async Task<IActionResult> CreateDayPlan([FromBody] DayPlan dayPlan)
        {
            if (dayPlan == null)
            {
                return BadRequest();
            }
            if (string.IsNullOrEmpty(dayPlan.Id))
            {
                dayPlan.Id = System.Guid.NewGuid().ToString();
            }
            await _context.SaveAsync(dayPlan);

            return CreatedAtAction(nameof(GetDayPlan), new { tripId = dayPlan.TripId, date = dayPlan.Date }, dayPlan);
        }
                
        [HttpPut("{tripId}/{date}")]
        public async Task<IActionResult> UpdateDayPlan(string tripId, string date, [FromBody] DayPlan updatedDayPlan)
        {
            if (updatedDayPlan == null || tripId != updatedDayPlan.TripId || date != updatedDayPlan.Date)
            {
                return BadRequest();
            }

            var existingPlan = await _context.LoadAsync<DayPlan>(tripId, date);
            if (existingPlan == null)
            {
                return NotFound();
            }
            await _context.SaveAsync(updatedDayPlan);

            return NoContent();
        }
                
        [HttpDelete("{tripId}/{date}")]
        public async Task<IActionResult> DeleteDayPlan(string tripId, string date)
        {
            var existingPlan = await _context.LoadAsync<DayPlan>(tripId, date);
            if (existingPlan == null)
            {
                return NotFound();
            }
            await _context.DeleteAsync(existingPlan);

            return NoContent();
        }
    }
}
