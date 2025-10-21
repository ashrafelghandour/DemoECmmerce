using Microsoft.AspNetCore.Http;

namespace eCommerce.SharedLibrary.MiddleWare
{
    public class ListenToOnlyApiGateway(RequestDelegate next)
    {
       public async Task InvokeAsynk(HttpContext context)
        {

            var signedHeader = context.Request.Headers["Api-Gateway"];

            if(signedHeader.FirstOrDefault() is null)
            {
                context.Response.StatusCode = StatusCodes.Status503ServiceUnavailable;
                await context.Response.WriteAsync("Sorry  service is unavilable");
                return;

            }
            else
            {
                await next(context);    
            }
        }
    }
}
