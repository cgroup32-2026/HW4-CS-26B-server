using Microsoft.AspNetCore.Mvc;
using tar1.Models;
using tar1.Services;

namespace tar1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GamesController : ControllerBase
    {
        DBServices db = new DBServices();

        [HttpGet]
        public IActionResult Get([FromQuery] string? name)
        {
            if (!string.IsNullOrEmpty(name))
                return Ok(db.GetGamesByName(name));
            return Ok(db.GetAllGames());
        }

        [HttpPost]
        public IActionResult Post([FromBody] Game game)
        {
            try
            {
                int newId = db.InsertGame(game);
                game.Id = newId;
                return Ok(game);
            }
            catch
            {
                return BadRequest("Failed to insert game.");
            }
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, [FromBody] Game game)
        {
            bool updated = db.UpdateGame(id, game);
            if (updated) return Ok("Game updated successfully.");
            return NotFound("Game not found.");
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            bool deleted = db.DeleteGame(id);
            if (deleted) return Ok("Game deleted.");
            return NotFound("Game not found.");
        }

        [HttpGet("Tags")]
        public IActionResult GetAllTags()
        {
            var tags = db.GetAllTags();
            return Ok(tags);
        }

        [HttpGet("ByTag")]
        public IActionResult GetByTag([FromQuery] string tag)
        {
            var games = db.GetGamesByTag(tag);
            return Ok(games);
        }

        [HttpPost("AddToUser")]
        public IActionResult AddToUser([FromQuery] int userId, [FromQuery] int gameId)
        {
            bool added = db.AddUserGame(userId, gameId);
            if (added) return Ok("Game added to user.");
            return BadRequest("Failed to add game.");
        }

        [HttpGet("Recommended/{userId}")]
        public IActionResult GetRecommended(int userId)
        {
            var games = db.GetRecommendedGames(userId);
            return Ok(games);
        }
    }
}