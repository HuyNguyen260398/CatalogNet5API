using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Catalog.Api.Entities;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Catalog.Api.Repositories
{
    public class MongoDbItemsRepository : IItemsRepository
    {
        private const string dbName = "catalog";
        private const string collectionName = "items";
        private readonly IMongoCollection<Item> itemsCollections;
        private readonly FilterDefinitionBuilder<Item> filterBuilder = Builders<Item>.Filter;

        public MongoDbItemsRepository(IMongoClient mongoClient)
        {
            IMongoDatabase db = mongoClient.GetDatabase(dbName);
            itemsCollections = db.GetCollection<Item>(collectionName);    
        }
        public async Task CreateItemAsync(Item item)
        {
            await itemsCollections.InsertOneAsync(item);
        }

        public async Task DeleteItemAsync(Guid id)
        {
            var filter = filterBuilder.Eq(item => item.Id, id);
            await itemsCollections.DeleteOneAsync(filter);
        }

        public async Task<Item> GetItemAsync(Guid id)
        {
            var filter = filterBuilder.Eq(item => item.Id, id);
            return await itemsCollections.Find(filter).SingleOrDefaultAsync();
        }

        public async Task<IEnumerable<Item>> GetItemsAsync()
        {
            return await itemsCollections.Find(new BsonDocument()).ToListAsync();
        }

        public async Task UpdateItemAsync(Item item)
        {
            var filter = filterBuilder.Eq(existingItem => existingItem.Id, item.Id);
            await itemsCollections.ReplaceOneAsync(filter, item);
        }
    }
}