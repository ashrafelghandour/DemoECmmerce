using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using eCommerce.SharedLibrary.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OrderApi.Application.DependencyIngection;
using OrderApi.Application.Interfaces;
using OrderApi.Application.Serveces;
using OrderApi.Infrastructure.Data;
using OrderApi.Infrastructure.Repositories;

namespace OrderApi.Infrastructure.DependencyInjection
{
    public static class ServiceContanier
    {
        public static IServiceCollection AddInfastructureService(this IServiceCollection services, IConfiguration config)
        {
           SharedServeceContainer.AddSharedServices<OrderDbContext>(services, config, config["MySerilog:FileName"]!);
            services.AddApplicationServices(config);

            services.AddScoped<IOrder, OrderRepository>();

            return services;
        }

        public static IApplicationBuilder UseInfastructurePolicy(this IApplicationBuilder app) {

            //Register middlewary such as:
            //Global Wxception 
            // ListenToApiGateWary Only

            SharedServeceContainer.UseSharedPolicies(app);

                return app;
        }
    }
}
