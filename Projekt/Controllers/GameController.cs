using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Catalog.Dtos;
using Catalog.Models;
using Catalog.Reposotories;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;

namespace Catalog.Controllers
{
    [ApiController]
    [Route("game")]
    public class GameController : ControllerBase
    {
        private readonly IGameRepository repository;
        private readonly ILogger<GameController> logger;
        public GameController(IGameRepository repository, ILogger<GameController> logger) // 
        {
            this.repository = repository;
            this.logger = logger;
        }
        
        [HttpGet]
        public async Task<IEnumerable<GameDto>> GetGames(string name = null)
        {
            var games = (await repository.GetGames()).Select(game => game.AsDto());
            if(!string.IsNullOrEmpty(name))
                games = games.Where(game => game.Name.Contains(name, StringComparison.OrdinalIgnoreCase));

            logger.LogInformation($"{DateTime.UtcNow.ToString("hh:mm:ss")}: Retrived {games.Count()} games");
            return games;
        }

        
        [HttpGet("{id}")]
        public async Task<ActionResult<GameDto>> GetGame(Guid id)
        {
            var game = await repository.GetGame(id);
            if (game is null){
                logger.LogInformation($"{DateTime.UtcNow.ToString("hh:mm:ss")} GetRequest: {NotFound()}");
                return NotFound();
            }
            logger.LogInformation($"{DateTime.UtcNow.ToString("hh:mm:ss")} returned {game.Id}");
            return game.AsDto();
        }

        
        [HttpPost]
        public async Task<ActionResult<GameDto>> CreateGame (CreateGameDto GameDto)
        {
            Game game = new Game()
            {
                Id = Guid.NewGuid(),
                Name = GameDto.Name,
                Description = GameDto.Description,
                Grade = GameDto.Grade,
                Image = GameDto.Image
            };
            await repository.CreateGame(game);
            logger.LogInformation($"{DateTime.UtcNow.ToString("hh:mm:ss")} Game {game.Id} created");
            return CreatedAtAction(nameof(GetGame), new { id = game.Id}, game.AsDto());
        }

        
        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateGame(Guid id, UpdateGameDto GameDto)
        {
            var existingGame = await repository.GetGame(id);
            if (existingGame is null)
                return NotFound();

            existingGame.Name = GameDto.Name;
            existingGame.Description = GameDto.Description;
            existingGame.Grade = GameDto.Grade;
            existingGame.Image = GameDto.Image;
            
            await repository.UpdateGame(existingGame);
            return NoContent();
        }
        
        
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteGame(Guid id)
        {
            var existingGame = await repository.GetGame(id);
            if (existingGame is null){
                logger.LogInformation($"{DateTime.UtcNow.ToString("hh:mm:ss")} DeleteRequest {NotFound()}");
                return NotFound();
            }
            await repository.DeleteGame(id);
            logger.LogInformation($"{DateTime.UtcNow.ToString("hh:mm:ss")} Game {id} deleted");
            return NoContent();
        }

        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<Game>>> Search(string name)
        {
            var games = await repository.Search(name);
            if (games is null){
                logger.LogInformation($"{DateTime.UtcNow.ToString("hh:mm:ss")} SearchRequest:{NotFound()}");
                return NotFound();
            }

            logger.LogInformation($"{DateTime.UtcNow.ToString("hh:mm:ss")} Retrived {games.Count()} SearchRequests");
            var gamesReturned = games.Select(game => game.AsDto());
            return Ok(gamesReturned);
        }
    }
}