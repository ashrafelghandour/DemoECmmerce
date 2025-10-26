using eCommerce.SharedLibrary;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OrderApi.Application.Serveces;
using Polly;
using Polly.Retry;

namespace OrderApi.Application.DependencyIngection
{
    public static class ServiceContainer
    {

        public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
        {

            
            services.AddHttpClient<IOrderService, OrderService>(option =>
            {
                option.BaseAddress = new Uri(configuration["ApiGateway:BaseAddress"]!);
                option.Timeout = TimeSpan.FromSeconds(1);
            });

            var retryStrategy = new RetryStrategyOptions
            {
                ShouldHandle = new PredicateBuilder().Handle<TaskCanceledException>(),
                 BackoffType = DelayBackoffType.Constant,
                 UseJitter = true
                 ,MaxRetryAttempts = 3,
                  Delay = TimeSpan.FromSeconds(500),
                   OnRetry = args =>
                   {
                       string message = $"OnRetry Attmept: {args.AttemptNumber} Outcome {args.Outcome}";
                       LogException.LogToDebugger(message);
                       LogException.LogToConsole(message);

                       return ValueTask.CompletedTask;
                   }
            };

            services.AddResiliencePipeline("my-retry-pipeline", bulder =>
            {
                bulder.AddRetry(retryStrategy);
            });
            return services;
        }
        

    }
}
