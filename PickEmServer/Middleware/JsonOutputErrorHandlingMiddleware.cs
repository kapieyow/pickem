using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Net;
using System.Threading.Tasks;

namespace PickEmServer.Middleware
{
    public class JsonOutputErrorHandlingMiddleware
    {
        private readonly RequestDelegate _nextMiddleware;
        private readonly ILogger<JsonOutputErrorHandlingMiddleware> _logger;

        public JsonOutputErrorHandlingMiddleware(RequestDelegate next, ILogger<JsonOutputErrorHandlingMiddleware> logger)
        {
            this._logger = logger;
            this._nextMiddleware = next;
        }

        public async Task Invoke(HttpContext context /* other dependencies */)
        {
            try
            {
                await _nextMiddleware(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            // default is five hundo
            var code = HttpStatusCode.InternalServerError;


            // TODO : this is only good in dev mode, more specific filters useful maybe in comments

            //if (exception is MyNotFoundException) code = HttpStatusCode.NotFound;
            //else if (exception is MyUnauthorizedException) code = HttpStatusCode.Unauthorized;
            //else if (exception is MyException) code = HttpStatusCode.BadRequest;

            // var result = JsonConvert.SerializeObject(new { error = exception.Message });

            var result = JsonConvert.SerializeObject(exception);

            _logger.LogError("Unhandled Exception ({0}) json of all ({1})", exception.Message, result);

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)code;
            return context.Response.WriteAsync(result);
        }
    }
}
