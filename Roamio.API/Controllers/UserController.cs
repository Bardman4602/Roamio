using Microsoft.AspNetCore.Mvc;
using Amazon.DynamoDBv2.DataModel;
using Roamio.API.Models;
using System.Threading.Tasks;

namespace Roamio.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IDynamoDBContext _context;

        public UserController(IDynamoDBContext context)
        {
            _context = context;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserById(string id)
        {
            var user = await _context.LoadAsync<User>(id);
            if (user == null)
            {
                return NotFound();
            }

            return Ok(user);
        }

        [HttpPost]
        public async Task<IActionResult> CreateUser([FromBody] User user)
        {
            if (user == null)
            {
                return BadRequest();
            }

            if (string.IsNullOrEmpty(user.Id))
            {
                user.Id = System.Guid.NewGuid().ToString();
            }
            await _context.SaveAsync(user);

            return CreatedAtAction(nameof(GetUserById), new { id = user.Id }, user);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(string id, [FromBody] User updatedUser)
        {
            if (updatedUser == null || id != updatedUser.Id)
            {
                return BadRequest();
            }

            var existingUser = await _context.LoadAsync<User>(id);
            if (existingUser == null)
            {
                return NotFound();
            }
            await _context.SaveAsync(updatedUser);

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(string id)
        {
            var user = await _context.LoadAsync<User>(id);
            if (user == null)
            {
                return NotFound();
            }
            await _context.DeleteAsync(user);

            return NoContent();
        }
    }
}
