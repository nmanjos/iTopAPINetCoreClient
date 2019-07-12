using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using ProperWebAPI.Options;
using ProperWebAPI.Services;
using Swashbuckle.AspNetCore.Swagger;

namespace ProperWebAPI.Installers
{
    public class MvcInstaller : IInstaller
    {
        public void InstallServices(IServiceCollection services, IConfiguration configuration, ILogger logger)
        {
            logger.LogInformation("Started MVC Installer");
            var jwtOptions = new JwtOptions();
            configuration.Bind(key: "JwtOptions", jwtOptions);

            services.AddSingleton(jwtOptions);

            services.AddScoped<IIdentityService, IdentityService>();

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
            services.AddAuthentication(configureOptions: x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
                .AddJwtBearer(x =>
                {
                    x.SaveToken = true;
                    x.TokenValidationParameters = new TokenValidationParameters
                    {
                        
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(key: Encoding.ASCII.GetBytes(jwtOptions.Secret)),
                        ValidateIssuer = false,
                        ValidateAudience = false,
                        RequireExpirationTime = false,
                        ValidateLifetime = true

                    };
                });
            
            services.AddSwaggerGen(x =>
            {
                x.SwaggerDoc(name:"v1", new Info { Title = "Tweetbook API", Version = "v1" });
                var security = new Dictionary<string, IEnumerable<string>>
                {
                    {"Bearer", new string[0] }
                };

                x.AddSecurityDefinition(name: "Bearer", new ApiKeyScheme
                {
                    Description = "JWT Authorization header using the bearer scheme",
                    Name = "Authorization",
                    In = "header",
                    Type = "apiKey"
                });
                x.AddSecurityRequirement(security);
            });
        }
    }
}
