using Microsoft.AspNetCore.Mvc;
using tar1.Models;
using tar1.Services;

namespace tar1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        DBServices db = new DBServices();

        [HttpGet]
        public IActionResult Get()
        {
            var users = db.GetAllUsers();
            return Ok(users);
        }

        [HttpPost("Register")]
        public IActionResult Register([FromBody] User user)
        {
            try
            {
                int newId = db.InsertUser(user.Name, user.Email, user.Password, user.Active);
                user.Id = newId;
                return Ok(user);
            }
            catch
            {
                return BadRequest("User already exists.");
            }
        }

        [HttpPost("Login")]
        public IActionResult Login([FromBody] User user)
        {
            var loggedIn = db.LoginUser(user.Email, user.Password);
            if (loggedIn == null) return NotFound("Invalid email or password.");
            return Ok(loggedIn);
        }

        [HttpPut("{id}")]
        public IActionResult UpdateUser(int id, [FromBody] User user)
        {
            bool updated = db.UpdateUser(id, user.Name, user.Password);
            if (updated) return Ok("User updated successfully.");
            return NotFound("User not found.");
        }

        [HttpGet("{userId}/Games")]
        public IActionResult GetUserGames(int userId)
        {
            var games = db.GetUserGames(userId);
            return Ok(games);
        }
    }
}