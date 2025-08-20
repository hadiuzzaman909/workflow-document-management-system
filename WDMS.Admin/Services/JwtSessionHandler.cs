using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace WDMS.Admin.Services
{
    public class JwtSessionHandler : DelegatingHandler
    {
        private readonly IHttpContextAccessor _ctx;
        public JwtSessionHandler(IHttpContextAccessor ctx) => _ctx = ctx;
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage r, CancellationToken ct)
        {
            var token = _ctx.HttpContext?.Session.GetString("JwtToken");
            if (!string.IsNullOrEmpty(token))
                r.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            return base.SendAsync(r, ct);
        }
    }
}
