
using eCommerce.SharedLibrary.MiddleWare;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace eCommerce.SharedLibrary.DependencyInjection
{
    public static class SharedServeceContainer
    {
        public static IServiceCollection AddSharedServices<TContext>(this IServiceCollection services
            ,IConfiguration configuration ,string fileName) where TContext : DbContext
        {

            // 1) Add DbContext فقط
            services.AddDbContext<TContext>(options =>
            {
                options.UseSqlServer(
                    configuration.GetConnectionString("eCommerceConnection"),
                    sqlserveroption => sqlserveroption.EnableRetryOnFailure());
            });

            // add srial logger

            Log.Logger = new LoggerConfiguration()
              .MinimumLevel.Information()
              .WriteTo.Debug()
              .WriteTo.Console()
              .WriteTo.File(
                  path: $"{fileName}-.txt",
                  restrictedToMinimumLevel: Serilog.Events.LogEventLevel.Information,
                  outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}{Exception}",
                  rollingInterval: RollingInterval.Day )
              .CreateLogger();

                JWTAuthScheme.AddJWTAuthenticationScheme(services, configuration);

            
            return services;
        }

        public static IApplicationBuilder UseSharedPolicies(this IApplicationBuilder application)
        {
            application.UseMiddleware<GlobalException>();
           // application.UseMiddleware<ListenToOnlyApiGateway>();
            return application;
        }
        
    }
}
