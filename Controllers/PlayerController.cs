using System.Security.Cryptography;
using Microsoft.AspNetCore.Mvc;
using MyGameApi.Models;
using MyGameApi.Services;

namespace MyGameApi.Controllers
{
    [ApiController]
    [Route("api/player")]
    public class PlayerController : ControllerBase
    {
        private readonly MongoDbService _mongoDbService;

        public PlayerController(MongoDbService mongoDbService)
        {
            _mongoDbService = mongoDbService;
        }

        // POST: api/player/register
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] Player player)
        {
            // Generar hash para la contraseña
            var hashedPassword = HashPassword(player.PasswordHash);
            player.PasswordHash = hashedPassword;

            var success = await _mongoDbService.CreatePlayer(player);
            if (!success)
                return BadRequest("Username already exists");

            return Ok("Player registered successfully");
        }

        // POST: api/player/login
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] Player player)
        {
            var existingPlayer = await _mongoDbService.GetPlayer(player.Username);
            if (existingPlayer == null || !VerifyPassword(player.PasswordHash, existingPlayer.PasswordHash))
                return Unauthorized("Invalid credentials");

            return Ok("Login successful");
        }

        // POST: api/player/submit-score
        [HttpPost("submit-score")]
        public async Task<IActionResult> SubmitScore([FromBody] Player player)
        {
            var success = await _mongoDbService.UpdateScore(player.Username, player.HighestScore);
            return Ok(success
                ? "Score updated successfully"
                : "Score not updated (either player not found or score is not higher)");
        }

        // GET: api/player/leaderboard
        [HttpGet("leaderboard")]
        public async Task<IActionResult> GetLeaderboard()
        {
            var leaderboard = await _mongoDbService.GetLeaderboard();
            return Ok(leaderboard);
        }

        // Método para generar el hash de la contraseña (PBKDF2)
        private static string HashPassword(string password)
        {
            using var rng = RandomNumberGenerator.Create();
            var salt = new byte[16];
            rng.GetBytes(salt);

            using var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 100000, HashAlgorithmName.SHA256);
            var hash = pbkdf2.GetBytes(32);

            return Convert.ToBase64String(salt) + "." + Convert.ToBase64String(hash);
        }

        // Método para verificar la contraseña comparando el hash almacenado
        private static bool VerifyPassword(string password, string storedHash)
        {
            try
            {
                var parts = storedHash.Split('.');
                if (parts.Length != 2)
                    return false;

                var salt = Convert.FromBase64String(parts[0]);
                var storedHashBytes = Convert.FromBase64String(parts[1]);

                using var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 100000, HashAlgorithmName.SHA256);
                var computedHash = pbkdf2.GetBytes(32);

                return CryptographicOperations.FixedTimeEquals(computedHash, storedHashBytes);
            }
            catch
            {
                return false;
            }
        }
    }
}
