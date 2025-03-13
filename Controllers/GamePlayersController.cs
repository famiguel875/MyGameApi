using Microsoft.AspNetCore.Mvc;
using MyGameApi.Model;
using MyGameApi.Service;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MyGameApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GamePlayersController : ControllerBase
    {
        private readonly GamePlayerService _playerService;

        public GamePlayersController(GamePlayerService playerService)
        {
            _playerService = playerService;
        }

        // GET: api/GamePlayers
        [HttpGet]
        public async Task<ActionResult<IEnumerable<GamePlayer>>> GetPlayers()
        {
            var players = await _playerService.GetAsync();
            return Ok(players);
        }

        // GET: api/GamePlayers/5
        [HttpGet("{id}")]
        public async Task<ActionResult<GamePlayer>> GetPlayer(long id)
        {
            var player = await _playerService.GetAsync(id);
            if (player == null)
            {
                return NotFound();
            }
            return Ok(player);
        }

        // PUT: api/GamePlayers/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePlayer(long id, GamePlayer player)
        {
            if (id != player.Id)
            {
                return BadRequest("Player ID mismatch");
            }

            var existingPlayer = await _playerService.GetAsync(id);
            if (existingPlayer == null)
            {
                return NotFound();
            }

            await _playerService.UpdateAsync(id, player);
            return NoContent();
        }

        // POST: api/GamePlayers
        [HttpPost]
        public async Task<ActionResult<GamePlayer>> CreatePlayer(GamePlayer player)
        {
            await _playerService.CreateAsync(player);
            return CreatedAtAction(nameof(GetPlayer), new { id = player.Id }, player);
        }

        // DELETE: api/GamePlayers/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePlayer(long id)
        {
            var player = await _playerService.GetAsync(id);
            if (player == null)
            {
                return NotFound();
            }

            await _playerService.RemoveAsync(id);
            return NoContent();
        }
    }
}
