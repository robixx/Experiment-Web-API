using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Work.Models.RequestModels;
using Work.Models.ResponseModels;

namespace Work.Service.IService
{
    public interface ISystemConfigureService
    {

        Task<SystemConfigureResponse> GetAsync();

        Task SaveAsync(SaveSystemConfigurationRequest request);
    }
}
