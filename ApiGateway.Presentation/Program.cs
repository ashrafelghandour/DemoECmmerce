using ApiGateway.Presentation.Middleware;
using eCommerce.SharedLibrary.DependencyInjection;
using Ocelot.Cache.CacheManager;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;

var builder = WebApplication.CreateBuilder(args);



builder.Configuration.AddJsonFile("Ocelot.json",optional:false,reloadOnChange:true);
 builder.Services.AddOcelot().AddCacheManager(x => x.WithDictionaryHandle());
JWTAuthScheme.AddJWTAuthenticationScheme(builder.Services, builder.Configuration);

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(builder =>
    {
        builder.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin();
    });
});


var app = builder.Build();

app.UseHttpsRedirection();
app.UseCors();
app.UseMiddleware<AttachSignatureToRequst>();
app.UseOcelot().Wait();
app.Run();
