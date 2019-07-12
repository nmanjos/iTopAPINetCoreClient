using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ProperWebAPI.Data;
using ProperWebAPI.Services;



namespace ProperWebAPI.Installers
{
    public class DBInstaller : IInstaller
    {
        public void InstallServices(IServiceCollection services, IConfiguration configuration, ILogger Logger)
        {
            Logger.LogInformation("Started DBInstaller");
            services.AddDbContext<DataContext>(options =>
                options.UseMySQL(
                    configuration.GetConnectionString("DefaultConnection")));
            services.AddDefaultIdentity<IdentityUser>()

                .AddEntityFrameworkStores<DataContext>();
            services.AddScoped<IPostService, PostService>();
        }
    }
}
