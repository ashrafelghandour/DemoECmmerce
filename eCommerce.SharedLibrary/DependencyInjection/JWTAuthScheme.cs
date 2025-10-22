using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace eCommerce.SharedLibrary.DependencyInjection;

public static class JWTAuthScheme
{
    public static IServiceCollection AddJWTAuthenticationScheme(this IServiceCollection services, IConfiguration configuration)
    {
        var myjwtoptions = configuration.GetSection("Authentication").Get<AuthJWTBerrer>();

        services.AddAuthentication().AddJwtBearer(options =>
        {
            options.SaveToken = true;
            options.TokenValidationParameters = new TokenValidationParameters
            {

                ValidateIssuer = true,
                ValidIssuer = myjwtoptions!.Issuer,
                ValidateAudience = true,
                ValidAudience = myjwtoptions.Audience,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(myjwtoptions.SigningKey))


            };

        });
        return services;
    }
}
