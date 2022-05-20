using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Catalog.Models;

namespace Catalog.Reposotories
{
    public interface IGameRepository
    {
        Task <IEnumerable<Game>> Search(string lärare);
        Task<Game> GetGame(Guid id);
        Task <IEnumerable<Game>> GetGames();
        Task CreateGame(Game form);
        Task UpdateGame(Game form);
        Task DeleteGame(Guid id);
    }
}