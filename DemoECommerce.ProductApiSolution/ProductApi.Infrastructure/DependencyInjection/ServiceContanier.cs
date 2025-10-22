using eCommerce.SharedLibrary.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ProductApi.Infrastructure.Data;
using ProductApi.Infrastructure.Repositories;
using ProductApiApplication.Interfaces;

namespace ProductApi.Infrastructure.DependencyInjection;
    public static class ServiceContanier
    {
        public static IServiceCollection AddInfrastuctuerService(this IServiceCollection services, IConfiguration config)
        {

        // Add databse Connectivity
        //add authentication scheme
        SharedServeceContainer.AddSharedServices<ProductDbContext>(services, config, config["MySerilog:FileName"]!);

          services.AddScoped<IProduct, ProductRepository>();
       
        return services;

        }

       public static IApplicationBuilder UseInfrastructurePolicy(this IApplicationBuilder app)
       {
        SharedServeceContainer.UseSharedPolicies(app);
        return app;
       }  
    }
