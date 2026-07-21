using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Work.Models.EntityModels;

namespace Work.Repository.IRepository
{
    public interface ISystemConfigureRepository
    {
        Task<SystemConfigure?> GetAsync(string id);
        Task<SystemConfigure?> allGetAsync();
        Task UpsertAsync(SystemConfigure model, IClientSessionHandle? session = null);

        Task<IClientSessionHandle> StartSessionAsync();
    }
}
