namespace TaskCat.Lib.Utility.ActionFilter
{
    using Exceptions;
    using System;
    using System.Net;
    using System.Net.Http;
    using System.Web.Http.Filters;

    internal class ErrorDocumentFilter : ActionFilterAttribute
    {
        // TODO: Need to add proper debug and other informations, if debug method is needed then we'd add the debug information, or else
        // Not going to add debug informations in production server :)

        public override void OnActionExecuted(HttpActionExecutedContext actionExecutedContext)
        {
            if (actionExecutedContext.Exception != null && actionExecutedContext.Request != null)
            {
                HttpResponseMessage response;
                Exception error;
                HttpStatusCode httpStatusCode = HttpStatusCode.InternalServerError;

                if (actionExecutedContext.Exception is NotImplementedException)
                    httpStatusCode = HttpStatusCode.NotImplemented;
                else if (actionExecutedContext.Exception is EntityNotFoundException)
                    httpStatusCode = HttpStatusCode.NotFound;
                else if (actionExecutedContext.Exception is InvalidOperationException)
                    httpStatusCode = HttpStatusCode.Forbidden;
                else if (actionExecutedContext.Exception is ArgumentException)
                    httpStatusCode = HttpStatusCode.BadRequest;
                else if(actionExecutedContext.Exception is FormatException)
                    httpStatusCode = HttpStatusCode.BadRequest;

                error = actionExecutedContext.Exception;
                response = actionExecutedContext.Request.CreateResponse(httpStatusCode, error, "application/json");
                actionExecutedContext.Response = response;
            }
        }
    }
}