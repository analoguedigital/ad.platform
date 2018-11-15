using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Results;

namespace WebApi.Results
{
    public class CachedResult<T> : OkNegotiatedContentResult<T>
    {
        public TimeSpan CacheDuration { get; private set; }

        public CachedResult(T content, TimeSpan duration, ApiController controller) : base(content, controller)
        {
            CacheDuration = duration;
        }

        public CachedResult(T content, IContentNegotiator contentNegotiator,
            HttpRequestMessage request, IEnumerable<MediaTypeFormatter> formatters)
            : base(content, contentNegotiator, request, formatters) { }

        public override async Task<HttpResponseMessage> ExecuteAsync(CancellationToken cancellationToken)
        {
            var response = await base.ExecuteAsync(cancellationToken);
            response.Headers.CacheControl = new CacheControlHeaderValue()
            {
                Public = false,
                MaxAge = CacheDuration,
                NoStore = true
            };

            return response;
        }
    }
}