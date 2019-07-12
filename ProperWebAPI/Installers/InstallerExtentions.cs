using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProperWebAPI.Installers
{
    public static class InstallerExtentions
    {
        public static void InstallServicesInAssembly(this IServiceCollection services, IConfiguration Configuration, ILogger Logger)
        {
            Logger.LogInformation("Started InstallerExtentions");
            var installers = typeof(Startup).Assembly.ExportedTypes.Where(x =>
            typeof(IInstaller).IsAssignableFrom(x) && !x.IsInterface && !x.IsAbstract)
                .Select(Activator.CreateInstance).Cast<IInstaller>().ToList();

            installers.ForEach(installer => installer.InstallServices(services, Configuration, Logger));
        }
    }
}
