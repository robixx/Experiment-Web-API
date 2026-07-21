using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Work.Models.EntityModels;
using Work.Repository.IRepository;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Work.Repository.Repository
{
    public class SystemConfigureRepository : ISystemConfigureRepository
    {
        private readonly IMongoCollection<SystemConfigure> _collection;
        private readonly IMongoClient _mongoClient;

        public SystemConfigureRepository(IMongoDatabase database)
        {
            _collection = database.GetCollection<SystemConfigure>("SystemConfigure");
            _mongoClient = database.Client;
        }

        public async Task<SystemConfigure?> GetAsync(string id)
        {
            if (!ObjectId.TryParse(id, out ObjectId objectId))
            {
                return null;
            }
            return await _collection.Find(x => x.Id == objectId).FirstOrDefaultAsync();
        }

        public async Task UpsertAsync(SystemConfigure model, IClientSessionHandle? session = null)
        {
            var filter = Builders<SystemConfigure>.Filter.Eq(x => x.Id, model.Id);

            var options = new ReplaceOptions
            {
                IsUpsert = true
            };

            if (session == null)
            {
                await _collection.ReplaceOneAsync(filter, model, options);
            }
            else
            {
                await _collection.ReplaceOneAsync(session, filter, model, options);
            }
        }

        public async Task<IClientSessionHandle> StartSessionAsync()
        {
            return await _mongoClient.StartSessionAsync();
        }

        public async Task<SystemConfigure?> allGetAsync()
        {
            return await _collection.Find(_ => true).FirstOrDefaultAsync();
        }
    }
}
