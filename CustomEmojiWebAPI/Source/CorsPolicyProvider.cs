namespace CustomEmojiWebAPI.Source
{
    using System;
    using System.Collections.Immutable;
    using System.Linq;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Cors.Infrastructure;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.Primitives;

    public class CorsPolicyProvider : ICorsPolicyProvider
    {
        public Task<CorsPolicy> GetPolicyAsync(HttpContext context, string policyName)
        {
            context.Response.Headers.Append("Access-Control-Allow-Origin", "*");

            var policyBuilder = new CorsPolicyBuilder();

            policyBuilder.AllowAnyHeader();
            policyBuilder.AllowAnyMethod();
            policyBuilder.AllowCredentials();
            policyBuilder.AllowAnyOrigin();

            return Task.FromResult(policyBuilder.Build());
        }
    }
}
