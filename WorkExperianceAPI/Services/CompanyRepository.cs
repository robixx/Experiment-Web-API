using Microsoft.Extensions.Options;
using MongoDB.Driver;
using WorkExperianceAPI.IService;
using WorkExperianceAPI.Models;

namespace WorkExperianceAPI.Services
{
    public class CompanyRepository : ICompanyRepository
    {
        private readonly IMongoCollection<CompanyInfo> _collection;

        public CompanyRepository(IOptions<MongoDbSettings> options)
        {
            var client = new MongoClient(options.Value.ConnectionString);

            var database = client.GetDatabase(options.Value.DatabaseName);

            _collection = database.GetCollection<CompanyInfo>("CompanyInfo");
        }

        public async Task CreateAsync(CompanyInfo company)
        {
            await _collection.InsertOneAsync(company);
        }

        public async Task<List<CompanyInfo>> GetAllAsync()
        {
            return await _collection.Find(_ => true).ToListAsync();
        }
    }
}
