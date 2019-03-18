using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace BankStatementDownloader.Services
{
    public interface IBankApiClient
    {
        Task SetDefaultRequestHeadersAsync(Dictionary<string,string> headers);
        Task<HttpResponseMessage> GetAsync(string requestUri);
        Task<HttpResponseMessage> PostAsync(string requestUri, HttpContent httpContent);
    }
}
