namespace K1.HttpHelpers
{
    using System;
    using System.Linq;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.Net.Http;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using CommonInterfaces;

    public class HttpHelper : HttpClient
    {
        private async Task<HttpResponseMessage> SendAsync
            (string url,
            HttpMethod method, 
            Guid? traceLogID = null, 
            ILogger logger = null,
            KeyValuePair<string,string>[] query = null, 
            KeyValuePair<string, string>[] headers = null,
            string body = null,
            string contentType = null)
        {
            if (string.IsNullOrWhiteSpace(url))
                throw new ArgumentNullException(nameof(url));

            traceLogID = traceLogID ?? Guid.NewGuid();

            try
            {

                var uri = CreateUri(url, query);
                var request = new HttpRequestMessage(method, uri);

                AddBody(request, body, contentType);
                AddHeaders(request, headers);

                logger?.Info($"Sending Request: {traceLogID.Value} {Environment.NewLine} {request.ToString()} {Environment.NewLine} Body:{body ?? ""}");

                var sendingTime = DateTime.UtcNow;
                var response = await SendAsync(request);
                var receivedTime = DateTime.UtcNow;

                logger?.Info($"Received Response: {traceLogID.Value}; ResponseTime: {(receivedTime - sendingTime).TotalMilliseconds}ms {Environment.NewLine} {response.ToString()} Body: {response.Content?.ReadAsStringAsync().Result}");

                return response;
            }
            catch (Exception ex)
            {
                logger?.Error($"Request {traceLogID} failed",ex);
                throw;
            }
        }

        private static Uri CreateUri(string url, KeyValuePair<string, string>[] query = null)
        {
            var uriBuilder = new UriBuilder(url);

            if(query != null && query.Length > 0)
            {
                uriBuilder.Query = string.Join("&", query.Select(q => q.Key + "=" + q.Value));
            }

            return uriBuilder.Uri;
        }
        private static void AddBody(HttpRequestMessage httpRequestMessage,string body,string contentType)
        {
            if (!string.IsNullOrWhiteSpace(body))
            {
                httpRequestMessage.Content = new StringContent(body, Encoding.UTF8, contentType);
            }
        }
        private static void AddHeaders(HttpRequestMessage httpRequestMessage,KeyValuePair<string,string>[] headers = null)
        {
            if(headers != null && headers.Length > 0)
            {
                for (int i = 0; i < headers.Length; i++)
                {
                    httpRequestMessage.Headers.Add(headers[i].Key, headers[i].Value);
                }
            }
        }
    }
}
