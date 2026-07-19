using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Work.Models.CommonData;
using Work.Repository.IRepository;
using Work.Repository.Repository;
using Work.Service.IService;
using Work.Service.Services;
namespace Work.Configuration.Dependency
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
        {
            var jwtSettings = configuration.GetSection("JwtSettings").Get<JwtSettings>();
            if (jwtSettings == null)
            {
                throw new InvalidOperationException("JwtSettings section is missing in appsettings.json");
            }
            services.AddSingleton(jwtSettings);
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IAuthService, AuthService>();
            return services;
        }
    }
}
