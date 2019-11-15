using System.Net.Http;

namespace K1.HttpHelpers
{
    public class HttpResponse<TModel>
    {
        public HttpResponseMessage HttpResponseMessage { get; set; }
        public TModel Data { get; set; }
    }
}
