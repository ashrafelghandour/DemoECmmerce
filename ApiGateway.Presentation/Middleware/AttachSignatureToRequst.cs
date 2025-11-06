namespace ApiGateway.Presentation.Middleware
{
    public class AttachSignatureToRequst(RequestDelegate next)
    {
        public async Task InvokeAsync(HttpContext con)
        {
            con.Request.Headers["Api-Gateway"] = "Signed";
            await next(con);
        }
    }
}
