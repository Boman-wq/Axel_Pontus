using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Catalog.Dtos;
using Catalog.Models;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Catalog.Reposotories
{
    public class MongoDbGameRepository : IGameRepository
    {
        private const string databaseName = "catalog";
        private const string collectionName = "games";
        private readonly IMongoCollection<Game> formsCollection;
        private readonly FilterDefinitionBuilder<Game> filterBuilder = Builders<Game>.Filter;
        public MongoDbGameRepository(IMongoClient mongoClient)
        {
            IMongoDatabase database = mongoClient.GetDatabase(databaseName);
            formsCollection = database.GetCollection<Game>(collectionName);
        }
       
        public async Task CreateGame(Game form)
        {
            await formsCollection.InsertOneAsync(form);
        }
        
        public async Task DeleteGame(Guid id)
        {
            var filter = filterBuilder.Eq(form => form.Id, id);
            await formsCollection.DeleteOneAsync(filter);
        }

        public async Task<Game> GetGame(Guid id)
        {
            var filter = filterBuilder.Eq(form => form.Id, id);
            return await formsCollection.Find(filter).SingleOrDefaultAsync();
        }
        
        public async Task<IEnumerable<Game>> GetGames()
        {
            return await formsCollection.Find(new BsonDocument()).ToListAsync();
        }
        
        public async Task<IEnumerable<Game>> Search(string name)
        {
            var filter = filterBuilder.Eq(game => game.Name, name);
            
            return await formsCollection.Find(filter).ToListAsync();
        }
        
        public async Task UpdateGame(Game game)
        {
            var filter = filterBuilder.Eq(exsistingForm => exsistingForm.Id, game.Id);
            await formsCollection.ReplaceOneAsync(filter, game);
        }
    }
} 
// docker run -d --rm --name mongo -p 27017:27017 -v mongodbata:/data/db -e MONGO_INITDB_ROOT_USERNAME=admin -e MONGO_INITDB_ROOT_PASSWORD=admin123 mongo
// docker run -d --rm --name mongo -p 27017:27017 -v mongodbata:/data/db mongo