using System;
using System.Data.Entity.Core;
using System.Net;
using System.Net.Http;
using System.Web.Http.Filters;

namespace WebApp.Filters
{
    public class GlobalExceptionFilterAttribute : ExceptionFilterAttribute 
    {
        public override void OnException(HttpActionExecutedContext context)
        {
            if (context.Exception is ObjectNotFoundException)
            {
                context.Response = context.Request.CreateErrorResponse(HttpStatusCode.NotFound, context.Exception.Message);
            } else if (
                context.Exception is ArgumentException ||
                context.Exception is NotSupportedException
            )
            {
                context.Response = context.Request.CreateErrorResponse(HttpStatusCode.BadRequest, context.Exception.Message);
            }
            else
            {
                // TODO: Implement logging

                context.Response = context.Request.CreateErrorResponse(
                HttpStatusCode.InternalServerError,
                    "Sorry, we are facing some issues. Try again in a few moments"
                );
            }
        }
    }
}