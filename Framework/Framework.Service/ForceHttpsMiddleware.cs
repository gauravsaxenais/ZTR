namespace ZTR.Framework.Service
{
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Http;

    public class ForceHttpsMiddleware
    {
        private readonly RequestDelegate _next;

        public ForceHttpsMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            var request = context.Request;

            request.Scheme = "https";

            await _next(context).ConfigureAwait(false);
        }
    }
}
