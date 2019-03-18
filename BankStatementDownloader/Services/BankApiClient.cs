using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace BankStatementDownloader.Services
{
    public class BankApiClient : IBankApiClient
    {
        private readonly HttpClient _httpClient;

        public BankApiClient(IConfiguration configuration, HttpClient httpClient)
        {
            httpClient.BaseAddress = new Uri(configuration["BankApi:Url"]);
            _httpClient = httpClient;
        }

        public async Task SetDefaultRequestHeadersAsync(Dictionary<string, string> headers)
        {
            await Task.Run(() =>
            {
                foreach (var header in headers)
                    _httpClient.DefaultRequestHeaders.Add(header.Key, header.Value);
            });
        }

        public Task<HttpResponseMessage> GetAsync(string requestUri)
        {
            return _httpClient.GetAsync(requestUri);
        }

        public Task<HttpResponseMessage> PostAsync(string requestUri, HttpContent httpContent)
        {
            return _httpClient.PostAsync(requestUri, httpContent);
        }
    }
}
