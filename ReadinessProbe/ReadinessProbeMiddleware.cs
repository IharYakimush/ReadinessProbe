using Microsoft.AspNetCore.Http;
using System;
using System.Threading.Tasks;

namespace ReadinessProbe
{
    public class ReadinessProbeMiddleware : IMiddleware
    {
        public Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            throw new NotImplementedException();
        }
    }
}
