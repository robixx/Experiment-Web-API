using WorkExperianceAPI.Models;

namespace WorkExperianceAPI.IService
{
    public interface ICompanyRepository
    {
        Task CreateAsync(CompanyInfo company);

        Task<List<CompanyInfo>> GetAllAsync();
    }
}
