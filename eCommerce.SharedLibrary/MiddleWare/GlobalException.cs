using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Azure.Core.Serialization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualBasic;

namespace eCommerce.SharedLibrary.MiddleWare
{
    public class GlobalException(RequestDelegate next)
    {
        public async Task InvokeAsync(HttpContext context)
        {
            //Declare Defulat Varibales
            string message = "sorry , intenal server error occurred. Kindly try agin";
            string tital = "error";
            int statusCode = (int)HttpStatusCode.InternalServerError;

            try
            {
                await next(context);

                //check if exception is too many of requst // 429 status code.
                if (context.Response.StatusCode == (int)HttpStatusCode.TooManyRequests)
                {

                    tital = "warning";
                    message = "Too many requst made اهدا شويه.";
                    statusCode = (int)HttpStatusCode.TooManyRequests;
                    await ModifyHeader(context, message, tital, statusCode);
                }
                
                // if respnse is unAuthorize // 401 cod
                if (context.Response.StatusCode == (int)HttpStatusCode.Unauthorized)
                {

                    tital = "Alert";
                    message = "You are not authorized to access {انت مش ليك صلاحيه }";
                    statusCode = (int)HttpStatusCode.Unauthorized;
                    await ModifyHeader(context, message, tital, statusCode);
                }

                //if the respons is forbiden 
                if (context.Response.StatusCode == (int)HttpStatusCode.Forbidden)
                {

                    tital = "not have access on this";
                    message = "You are not allowd ";
                    statusCode = (int)HttpStatusCode.Forbidden;
                    await ModifyHeader(context, message, tital, statusCode);
                }
            }
            catch (Exception ex)
            {
                // log eny exception 
                //log original Excepton /file , debugger ,console
                LogException.LogExceptions(ex); 

                if(ex is TaskCanceledException || ex is TimeoutException)
                {
                    tital = "Out of time";
                    message = "Requst timeout ....try again";
                    statusCode = (int)HttpStatusCode.RequestTimeout;
                }
                await ModifyHeader(context, message, tital, statusCode);

            }


        }
        private static async Task ModifyHeader(HttpContext context, string message, string tital, int statusCode)
        {
            //dispaly scary free message to client.
            context.Request.Headers.ContentType = "application/json";

            await context.Response.WriteAsync(JsonSerializer.Serialize(new ProblemDetails{ 
             Detail = message,
             Status = statusCode
             ,Title = tital
            }),CancellationToken.None);
        }
    }
}
