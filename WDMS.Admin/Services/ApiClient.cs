using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WDMS.Admin.Services
{
    public class ApiClient
    {
        private readonly HttpClient _http;
        public ApiClient(IHttpClientFactory f) => _http = f.CreateClient("Api");

        public Task<T?> GetAsync<T>(string url) => _http.GetFromJsonAsync<T>(url);

        public async Task<TOut?> PostAsync<TIn, TOut>(string url, TIn body)
        {
            var res = await _http.PostAsJsonAsync(url, body);
            if (!res.IsSuccessStatusCode) return default;
            return await res.Content.ReadFromJsonAsync<TOut>();
        }

        public async Task<bool> DeleteAsync(string url)
        {
            var res = await _http.DeleteAsync(url);
            return res.IsSuccessStatusCode;
        }
    }
}
