namespace TaskCat.Common.Owin
{
    using Microsoft.Owin;
    using System.Net;
    using System.Net.Http;
    using System.Threading.Tasks;

    public class PreflightRequestsHandler : OwinMiddleware
    {
        public PreflightRequestsHandler(OwinMiddleware next) : base(next)
        {
        }

        public async override Task Invoke(IOwinContext context)
        {
            if (context.Request.Headers.ContainsKey("Origin") && context.Request.Method == "OPTIONS")
            {
                var response = new HttpResponseMessage { StatusCode = HttpStatusCode.OK };
                // Define and add values to variables: origins, headers, methods (can be global)               
                context.Response.Headers.Add("Access-Control-Allow-Origin", new string[] { "*" });
                context.Response.Headers.Add("Access-Control-Allow-Headers", new string[] { "authorization", "content-type" });
                context.Response.Headers.Add("Access-Control-Allow-Methods", new string[] { "GET", "POST", "PUT", "DELETE", "PATCH" });
            }
            else
            {
                await Next.Invoke(context);
            }
        }
    }
}