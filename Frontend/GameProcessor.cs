using ProjektWPF.Models;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace ProjektWPF
{
    public class GameProcessor
    {
        private readonly string Url;
        public GameProcessor(string Url)
        {
            this.Url = Url;
        }

        public async Task<List<Game>> GetAllGames()
        {
            using (HttpClient client = new())
            {
                return await client.GetFromJsonAsync<List<Game>>(Url);
            }
        }

        public async Task<Game> GetGame()
        {
            using (HttpClient client = new())
            {
                return await client.GetFromJsonAsync<Game>(Url);
            }
        }

        public async Task AddGame(Game game)
        {
            using (HttpClient client = new())
            {
                await client.PostAsJsonAsync(Url, game);
            }
        }

        public async Task UpdateGame(Game game)
        {
            using (HttpClient client = new())
            {
                await client.PutAsJsonAsync(Url, game);
            }
        }

        public async Task DeleteGame()
        {
            using (HttpClient client = new())
            {
                await client.DeleteAsync(Url);
            }
        }
    }
}
