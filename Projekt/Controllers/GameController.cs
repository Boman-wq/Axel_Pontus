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

            logger.LogInformation($"{DateTime.UtcNow.ToString("hh:mm:ss")}: Retrived {games.Count()} forms");
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
            logger.LogInformation($"{DateTime.UtcNow.ToString("hh:mm:ss")} Form {game.Id} created");
            return CreatedAtAction(nameof(GetGame), new { id = game.Id}, game.AsDto());
        }

        
        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateForm(Guid id, UpdateGameDto GameDto)
        {
            var existingForm = await repository.GetGame(id);
            if (existingForm is null)
                return NotFound();

            existingForm.Name = GameDto.Name;
            existingForm.Description = GameDto.Description;
            existingForm.Grade = GameDto.Grade;
            existingForm.Image = GameDto.Image;
            
            await repository.UpdateGame(existingForm);
            return NoContent();
        }
        
        
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteForm(Guid id)
        {
            var existingForm = await repository.GetGame(id);
            if (existingForm is null){
                logger.LogInformation($"{DateTime.UtcNow.ToString("hh:mm:ss")} DeleteRequest {NotFound()}");
                return NotFound();
            }
            await repository.DeleteGame(id);
            logger.LogInformation($"{DateTime.UtcNow.ToString("hh:mm:ss")} Form {id} deleted");
            return NoContent();
        }

        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<Game>>> Search(string name)
        {
           var forms = (await repository.Search(name)).Select(game => game.AsDto());
            if (forms is null){
                logger.LogInformation($"{DateTime.UtcNow.ToString("hh:mm:ss")} SearchRequest:{NotFound()}");
                return NotFound();
            }
            logger.LogInformation($"{DateTime.UtcNow.ToString("hh:mm:ss")} Retrived {forms.Count()} SearchRequests");
            return Ok(forms);
        }
    }
}