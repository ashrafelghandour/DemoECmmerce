using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AuthenticationApi.Application.Interfaces;
using AuthenticationApi.Infrastructure.Data;
using AuthenticationApi.Infrastructure.Repositories;
using eCommerce.SharedLibrary.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AuthenticationApi.Infrastructure.DependencyInjection
{
    public static class ServicesContanier
    {
        public static IServiceCollection AddInfraStructureService(this IServiceCollection services , IConfiguration config)
        {
            SharedServeceContainer.AddSharedServices<AuthenticationDbContext>(services, config, config["MySerilog:FileName"]!);
            services.AddScoped<IUser, UserRepository>();
            return services;
        }

        public static IApplicationBuilder UseInfrastructurePolicy(this IApplicationBuilder builder)
        {
            builder.UseSharedPolicies();
            return builder;
        }

    }
}
