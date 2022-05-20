using Catalog.Dtos;
using Catalog.Models;

namespace Catalog
{
    public static class Extenstions
    {
        public static GameDto AsDto(this Game game)
        {
            return new GameDto(game.Id, game.Name, game.Description, game.Grade, game.Image);
        }
    }
}